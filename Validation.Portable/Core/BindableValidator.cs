// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindableValidator.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    using WinRTXAMLValidation.Library.Attributes;
    using WinRTXAMLValidation.Library.Core.Extensions;
    using WinRTXAMLValidation.Library.Core.Helpers;

    /// <summary>
    /// The <see cref="BindableValidator" /> class evaluates validation rules of an entity and stores a collection of messages 
    /// of the properties that did not pass validation. The validation is run on each property change or whenever 
    /// the <see cref="ValidateAsync" /> method is called. Note that the validate async method is not synchronized 
    /// and may overrides the current collection of errors. It also provides and indexer property, that uses the property names 
    /// as keys and returns the message list for the specified property.
    /// </summary>
    public class BindableValidator : BindableBase
    {
        #region Fields

        /// <summary>
        /// Represents a collection of empty message values.
        /// </summary>
        public static readonly ReadOnlyCollection<ValidationMessage> EmptyMessagesCollection =
            new ReadOnlyCollection<ValidationMessage>(new List<ValidationMessage>());

        /// <summary>
        /// Represents a dictionary of empty message values.
        /// </summary>
        public static readonly ReadOnlyDictionary<string, ReadOnlyCollection<ValidationMessage>> EmptyMessagesDictionary =
            new ReadOnlyDictionary<string, ReadOnlyCollection<ValidationMessage>>(new Dictionary<string, ReadOnlyCollection<ValidationMessage>>());

        /// <summary>
        /// The generic entity validation key. This static object is needed to identify validation messages
        /// in the validation dictionary that belong to the entire entity instead of single properties.
        /// </summary>
        private static readonly IGroupValidationAttribute GenericEntityValidationKey = new GenericEntityKeyValidationAttribute();
        
        /// <summary>
        /// The locking object.
        /// </summary>
        private readonly object lockingObject = new object();

        /// <summary>
        /// The entity to validate.
        /// </summary>
        private readonly INotifyPropertyChanged entityToValidate;

        /// <summary>
        /// The validation messages collection (by property name).
        /// </summary>
        private readonly IDictionary<string, ReadOnlyCollection<ValidationMessage>> propertyMessages =
            new Dictionary<string, ReadOnlyCollection<ValidationMessage>>();

        /// <summary>
        /// The group validation messages collection (by group validation attribute).
        /// </summary>
        private readonly IDictionary<IGroupValidationAttribute, ValidationMessage> groupPropertyMessages =
            new Dictionary<IGroupValidationAttribute, ValidationMessage>();

        /// <summary>
        /// The overall property validation messages dictionary which combines the property validation messages 
        /// and the overall messages for a whole entity.
        /// </summary>
        private ReadOnlyDictionary<string, ReadOnlyCollection<ValidationMessage>> allMessages;

        /// <summary>
        /// The running validations task which contains the scheduled validation chain.
        /// </summary>
        private Task<bool> runningValidations;

        /// <summary>
        /// Indicates if a validation is currently executing.
        /// </summary>
        private bool isValidating;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableValidator" /> class with the entity to validate.
        /// </summary>
        /// <param name="entityToValidate">The entity to validate.</param>
        public BindableValidator(INotifyPropertyChanged entityToValidate)
        {
            Guard.AssertNotNull(entityToValidate, "entityToValidate");

            this.entityToValidate = entityToValidate;
            this.IsImplicitValidationEnabled = true;
        }

        #endregion

        #region Events

        /// <summary>
        /// Multicast event for property messages change notifications.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> PropertyMessagesChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the overall property validation messages dictionary which combines the property validation messages 
        /// and the overall messages for multiple properties.
        /// </summary>
        public ReadOnlyDictionary<string, ReadOnlyCollection<ValidationMessage>> AllMessages
        {
            get
            {
                lock (this.lockingObject)
                {
                    var propertyMessagesDictionary = this.propertyMessages.ToArray();
                    var multiPropertyMessagesDictionary = this.groupPropertyMessages.ToArray();

                    // add property messages to result
                    var dictionary = propertyMessagesDictionary.ToDictionary(
                        keyValuePair => keyValuePair.Key,
                        keyValuePair =>
                        keyValuePair.Value.Select(
                            message => new ValidationMessage(message, ValidationMessageKind.Property)).ToList());

                    // add group messages to result
                    foreach (var keyValuePair in multiPropertyMessagesDictionary)
                    {
                        var message = keyValuePair.Value;
                        var affectedPropertyNames = keyValuePair.Key.AffectedPropertyNames.ToList();
                        if (affectedPropertyNames == null || affectedPropertyNames.Count == 0)
                        {
                            // validation message regards the whole entity
                            this.AddPropertyValidationMessage(dictionary, "::ENTITY", message, ValidationMessageKind.Overall);
                        }
                        else
                        {
                            // validation message regards single properties
                            foreach (var propertyName in keyValuePair.Key.AffectedPropertyNames)
                            {
                                this.AddPropertyValidationMessage(dictionary, propertyName, message, ValidationMessageKind.Overall);
                            }
                        }
                    }

                    this.allMessages = new ReadOnlyDictionary<string, ReadOnlyCollection<ValidationMessage>>(
                        dictionary.ToDictionary(
                            keyValuePair => keyValuePair.Key,
                            keyValuePair => new ReadOnlyCollection<ValidationMessage>(keyValuePair.Value)));

                    return this.allMessages;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether implicit property setter validation is enabled or not.
        /// </summary>
        public bool IsImplicitValidationEnabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether a validation is currently in progress or not.
        /// </summary>
        public bool IsValidating
        {
            get { return this.isValidating; }
            private set { this.SetProperty(ref this.isValidating, value); }
        }

        /// <summary>
        /// Indexer that returns the validation messages of a specific property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns> The messages of the property, if it has messages. Otherwise, the <see cref="EmptyMessagesCollection" />.</returns>
        public ReadOnlyCollection<ValidationMessage> this[string propertyName]
        {
            get
            {
                lock (this.lockingObject)
                {
                    // get all regarding property messages
                    var propertyMessagesDictionary = this.propertyMessages.ToArray();
                    var groupPropertyMessagesDictionary = this.groupPropertyMessages.ToArray();

                    var list = propertyMessagesDictionary.Where(kvp => kvp.Key == propertyName)
                            .Select(kvp => kvp.Value)
                            .SelectMany(s => s)
                            .Select(msg => new ValidationMessage(msg, ValidationMessageKind.Property))
                            .Concat(groupPropertyMessagesDictionary
                                .SelectMany(
                                    keyValuePair => keyValuePair.Key.AffectedPropertyNames,
                                    (keyValuePair, propName) => new { keyValuePair.Value, propName })
                                .Where(tuple => tuple.propName == propertyName)
                                .Select(tuple => new ValidationMessage(tuple.Value, ValidationMessageKind.Overall)));

                    return new ReadOnlyCollection<ValidationMessage>(list.ToList());
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates all the properties decorated with the <see cref="ValidationAttribute"/> or <see cref="AsyncValidationAttribute"/> attribute.
        /// It updates each property messages collection with the new validation results (notifying if necessary). 
        /// </summary>
        /// <returns> The <see cref="Task"/> which determines the <see cref="bool"/> value which is True if the properties are valid. Otherwise, false. </returns>
        public async Task<bool> ValidateAsync()
        {
            Task<bool> validations;
            lock (this.lockingObject)
            {
                this.IsValidating = true;
                try
                {
                    this.runningValidations = this.runningValidations == null
                        ? this.ValidateNonconcurrentAsync()
                        : this.runningValidations.ContinueWithAsync(async () => await this.ValidateNonconcurrentAsync());
                    validations = this.runningValidations;
                }
                finally
                {
                    this.IsValidating = false;
                }
            }

            return await validations;
        }

        /// <summary>
        /// Validates the property, based on the rules set of the property <see cref="ValidationAttribute"/> and <see cref="AsyncValidationAttribute"/> attributes as well as the <see cref="IGroupValidationAttribute"/> of its class.
        /// It updates the messages collection with the new validation results (notifying if necessary). 
        /// </summary>
        /// <param name="propertyName"> The name of the property to validate. </param>
        /// <param name="removeOnly"> The indicator whether only to remove messages, but not add new messages. </param>
        /// <returns> The <see cref="Task"/> which determines the <see cref="bool"/> value which is <see langword="true"/> if the property is valid. Otherwise, <see langword="false"/>. </returns>
        /// <exception cref="ArgumentNullException"> When <paramref name="propertyName"/> is <see langword="null" />. </exception>
        /// <exception cref="ArgumentException"> When the <paramref name="propertyName"/> parameter does not match any property name. </exception>
        public async Task<bool> ValidatePropertyAsync(string propertyName, bool removeOnly = false)
        {
            Task<bool> validations;
            lock (this.lockingObject)
            {
                this.IsValidating = true;
                try
                {
                    this.runningValidations = this.runningValidations == null
                        ? this.ValidatePropertyNonconcurrentAsync(propertyName, removeOnly)
                        : this.runningValidations.ContinueWithAsync(
                            async () => await this.ValidatePropertyNonconcurrentAsync(propertyName, removeOnly));
                    validations = this.runningValidations;
                }
                finally
                {
                    this.IsValidating = false;
                }
            }

            return await validations;
        }

        /// <summary>
        /// Clears all former validation messages.
        /// </summary>
        public void ClearMessages()
        {
            var properties = new List<string>();
            lock (this.propertyMessages)
            {
                properties.AddRange(propertyMessages.Select(message => message.Key));
                this.propertyMessages.Clear();
            }

            lock (this.groupPropertyMessages)
            {
                properties.AddRange(groupPropertyMessages.SelectMany(message => message.Key.AffectedPropertyNames));
                this.groupPropertyMessages.Clear();
            }

            foreach (var property in properties)
            {
                this.OnPropertyChanged("Item[" + property + "]");
                this.NotifyPropertyMessagesChanged(property);
            }
            this.OnPropertyChanged("AllMessages");
        }

        /// <summary>
        /// Manually adds a validation message of a certain level to the entire entity.
        /// </summary>
        /// <param name="validationLevel">Level of the validation message (Error, Warning).</param>
        /// <param name="messageText">Validation message text.</param>
        /// <returns><c>true</c>, if the message has been added. <c>false</c>, if it was already there.</returns>
        public bool AddMessage(ValidationLevel validationLevel, string messageText)
        {
            return this.AddMessage(new ValidationMessage(validationLevel, messageText, ValidationMessageKind.Overall));
        }

        /// <summary>
        /// Manually adds a validation message for a given property with a certain level.
        /// </summary>
        /// <param name="propertySelector">Selector of the validated property.</param>
        /// <param name="validationLevel">Level of the validation message.</param>
        /// <param name="messageText">Validation message text.</param>
        /// <param name="showMessageOnProperty">Indicates if the message should be shown on the property at the UI.</param>
        /// <param name="showMessageInSummary">Indicates if the message should be shown in the validation summary at the UI.</param>
        /// <returns><c>true</c>, if the message has been added. <c>false</c>, if it was already there.</returns>
        public bool AddMessage(
            Expression<Func<object>> propertySelector,
            ValidationLevel validationLevel,
            string messageText,
            bool showMessageOnProperty = true,
            bool showMessageInSummary = true)
        {
            Guard.AssertNotNull(propertySelector, "propertySelector");

            string propertyName = propertySelector.ExtractPropertyName();
            var validationMessage = new ValidationMessage(validationLevel, messageText, showMessageOnProperty, showMessageInSummary, ValidationMessageKind.Property);

            return this.AddMessage(propertyName, validationMessage);
        }

        /// <summary>
        /// Manually adds a validation message to the entire entity.
        /// </summary>
        /// <param name="validationMessage">Validation message that should be added.</param>
        /// <returns><c>true</c>, if the message has been added. <c>false</c>, if it was already there.</returns>
        public bool AddMessage(ValidationMessage validationMessage)
        {
            Guard.AssertNotNull(validationMessage, "validationMessage");

            bool messagesChanged = this.SetEntityMessage(validationMessage);
            if (messagesChanged)
            {
                this.OnPropertyChanged("AllMessages");
            }

            return messagesChanged;
        }

        /// <summary>
        /// Manually adds a validation message for a given property.
        /// </summary>
        /// <param name="propertyName">Property, for which the validation message should be added.</param>
        /// <param name="validationMessage">Validation message that should be added.</param>
        /// <returns><c>true</c>, if the message has been added. <c>false</c>, if it was already there.</returns>
        public bool AddMessage(string propertyName, ValidationMessage validationMessage)
        {
            Guard.AssertNotNull(propertyName, "propertyName");
            Guard.AssertNotNull(validationMessage, "validationMessage");

            bool messagesChanged = this.SetPropertyMessage(
                propertyName,
                validationMessage,
                false);
            if (messagesChanged)
            {
                this.OnPropertyChanged("AllMessages");
                this.OnPropertyChanged("Item[" + propertyName + "]");
                this.NotifyPropertyMessagesChanged(propertyName);
            }

            return messagesChanged;
        }

        /// <summary>
        /// Adds a number of validation messages (for properties or the whole entity if the first tupel item is null or empty).
        /// </summary>
        /// <param name="validationMessages">Validation messages that should be added.</param>
        /// <returns><c>true</c>, if at least one message has been added. Otherwise <c>false</c>.</returns>
        public bool AddMessages(IEnumerable<Tuple<string, ValidationMessage>> validationMessages)
        {
            Guard.AssertNotNull(validationMessages, "validationMessages");

            // for each validation message -> add as property or entity-based message
            bool messagesChanged = false;
            foreach (var validationMessage in validationMessages)
            {
                bool messageChanged = !String.IsNullOrEmpty(validationMessage.Item1) 
                    ? this.AddMessage(validationMessage.Item1, validationMessage.Item2) 
                    : this.AddMessage(validationMessage.Item2);
                messagesChanged = messagesChanged || messageChanged;
            }

            return messagesChanged;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Validates all the properties decorated with a validation attribute.
        /// It updates each property messages collection with the new validation results (notifying if necessary). 
        /// </summary>
        /// <returns>The <see cref="Task"/> whose result is <c>true</c> if the properties are valid. Otherwise, <c>false</c>.</returns>
        private async Task<bool> ValidateNonconcurrentAsync()
        {
            var propertiesWithChangedMessages = new List<string>();
            var propertiesToValidate = this.entityToValidate.GetType().GetRuntimeProperties();

            var isValid = true;
            var propertyMessagesChanged = false;
            var groupPropertyMessagesChanged = false;
            var groupPropertyMatchMessages = new Dictionary<IGroupValidationAttribute, ValidationMessage>();
            var validationContextDictionary = new Dictionary<object, object>();
            foreach (var propertyInfo in propertiesToValidate)
            {
                // validate the current property
                var singlePropertyMatchMessages = new List<ValidationMessage>();
                isValid = (await this.TryValidatePropertyAsync(propertyInfo, singlePropertyMatchMessages, groupPropertyMatchMessages, validationContextDictionary, true)) && isValid;

                // evaluate property messages
                lock (this.propertyMessages)
                {
                    // If the messages have changed, save the property name to notify the update at the end of this method
                    if (this.SetPropertyMessages(propertyInfo.Name, singlePropertyMatchMessages) &&
                        !propertiesWithChangedMessages.Contains(propertyInfo.Name))
                    {
                        propertiesWithChangedMessages.Add(propertyInfo.Name);
                        propertyMessagesChanged = true;
                    }
                }

                // evaluate group property messages
                lock (this.groupPropertyMessages)
                {
                    foreach (var group in groupPropertyMatchMessages.Where(group => this.SetPropertyGroupMessage(group.Key, group.Value)))
                    {
                        foreach (var propertyName in group.Key.AffectedPropertyNames.Where(propertyName => !propertiesWithChangedMessages.Contains(propertyName)))
                        {
                            propertiesWithChangedMessages.Add(propertyName);
                        }

                        groupPropertyMessagesChanged = true;
                    }
                }
            }

            if (propertyMessagesChanged || groupPropertyMessagesChanged)
            {
                this.OnPropertyChanged("AllMessages");

                // Notify each property whose set of messages has changed since the last validation.  
                foreach (var propertyName in propertiesWithChangedMessages)
                {
                    this.NotifyPropertyMessagesChanged(propertyName);
                    this.OnPropertyChanged(string.Format(CultureInfo.CurrentCulture, "Item[{0}]", propertyName));
                }
            }

            return isValid;
        }

        /// <summary>
        /// Validates a property with the validation attributes defined on it.
        /// This updates the messages collection with the new validation results (notifying if necessary). 
        /// </summary>
        /// <param name="propertyName"> The name of the property to validate. </param>
        /// <param name="removeOnly"> The indicator whether only to remove obsolete messages, but not add new messages. </param>
        /// <returns> The <see cref="Task"/> whose result is <c>true</c> if the property is valid. Otherwise, <c>false</c>. </returns>
        private async Task<bool> ValidatePropertyNonconcurrentAsync(string propertyName, bool removeOnly = false)
        {
            Guard.AssertNotNull(propertyName, "propertyName");
            Guard.AssertNotEmpty(propertyName, "propertyName", "Invalid property name: " + propertyName);

            var propertyInfo = this.entityToValidate.GetType().GetRuntimeProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentException("Invalid property name: " + propertyName, propertyName);
            }

            var changedProperties = new List<string>();
            var singleMessagesChanged = false;
            var groupMessagesChanged = false;
            var singleMatchMessages = new List<ValidationMessage>();
            var groupMatchMessages = new Dictionary<IGroupValidationAttribute, ValidationMessage>();
            var isValid = await this.TryValidatePropertyAsync(propertyInfo, singleMatchMessages, groupMatchMessages, null, false);
            lock (this.propertyMessages)
            {
                if (removeOnly)
                {
                    // only modify if messages need to be removed
                    if (this.propertyMessages.ContainsKey(propertyInfo.Name))
                    {
                        var singlePropertyMatchMessages = this.propertyMessages[propertyInfo.Name];
                        var clearedMessageList = singlePropertyMatchMessages.ToList();
                        clearedMessageList.RemoveRange(
                            singlePropertyMatchMessages.Where(msg => singleMatchMessages.All(oldMsg => msg.MessageText != oldMsg.MessageText)));
                        singleMessagesChanged = this.SetPropertyMessages(propertyInfo.Name, clearedMessageList);
                    }
                }
                else
                {
                    // replace with new messages list
                    singleMessagesChanged = this.SetPropertyMessages(propertyInfo.Name, singleMatchMessages);
                }

                if (singleMessagesChanged)
                {
                    changedProperties.Add(propertyName);
                }
            }

            lock (this.groupPropertyMessages)
            {
                // This required null to be set for each group if no messages for group occurred.
                var groupMessages = removeOnly
                    ? groupMatchMessages.Where(groupMessage => groupMessage.Value == null && this.groupPropertyMessages.ContainsKey(groupMessage.Key))
                    : groupMatchMessages;

                foreach (var groupMessage in groupMessages.Where(groupMessage => this.SetPropertyGroupMessage(groupMessage.Key, groupMessage.Value)))
                {
                    changedProperties.AddRange(groupMessage.Key.AffectedPropertyNames);
                    groupMessagesChanged = true;
                }
            }

            if (singleMessagesChanged || groupMessagesChanged)
            {
                this.OnPropertyChanged("AllMessages");

                foreach (var changedProperty in changedProperties.Distinct())
                {
                    this.OnPropertyChanged(string.Format(CultureInfo.CurrentCulture, "Item[{0}]", changedProperty));
                    this.NotifyPropertyMessagesChanged(changedProperty);
                }
            }

            return isValid;
        }

        /// <summary>
        /// Core logic for validating a property, adding the results to the <paramref name="singlePropertyMatchMessages"/>
        /// and <paramref name="groupPropertyMatchMessages"/> lists. 
        /// </summary>
        /// <param name="propertyInfo"> The <see cref="PropertyInfo"/> of the property to validate. </param>
        /// <param name="singlePropertyMatchMessages"> A list containing the current messages of the property. </param>
        /// <param name="groupPropertyMatchMessages"> A dictionary containing the current messages of the properties groups. </param>
        /// <param name="validationContextDictionary"> The validation context dictionary which is used to keep state between different validation rules. </param>
        /// <param name="includeAllValidationAttributes"> Indicates whether attributes that should not have been used in setter validation will be validated as well. </param>
        /// <returns><c>true</c> if the property is valid. Otherwise, <c>false</c>.</returns>
        private async Task<bool> TryValidatePropertyAsync(
            PropertyInfo propertyInfo,
            List<ValidationMessage> singlePropertyMatchMessages,
            IDictionary<IGroupValidationAttribute, ValidationMessage> groupPropertyMatchMessages,
            Dictionary<object, object> validationContextDictionary,
            bool includeAllValidationAttributes)
        {
            Guard.AssertNotNull(propertyInfo, "propertyInfo");
            Guard.AssertNotNull(singlePropertyMatchMessages, "singlePropertyMatchMessages");
            Guard.AssertNotNull(groupPropertyMatchMessages, "groupPropertyMatchMessages");

            var context = new ValidationContext(this.entityToValidate, validationContextDictionary ?? new Dictionary<object, object>());
            context.MemberName = propertyInfo.Name;
            
            // gather all validation attributes that have to be evaluated
            var propertyValue = propertyInfo.GetValue(this.entityToValidate);
            var validationAttributes =
                propertyInfo.GetCustomAttributes(typeof(ValidationAttribute))
                            .Cast<ValidationAttribute>()
                            .ToArray();
            var asyncValidationAttributes =
                propertyInfo.GetCustomAttributes(typeof(AsyncValidationAttribute))
                            .Cast<AsyncValidationAttribute>()
                            .Where(att => includeAllValidationAttributes || att.UseInImplicitValidation)
                            .ToArray();
            var groupValidationAttributes =
                this.entityToValidate.GetType()
                    .GetTypeInfo()
                    .GetCustomAttributes()
                    .OfType<IGroupValidationAttribute>()
                    .Where(group => group.CausativePropertyNames.Contains(propertyInfo.Name))
                    .OfType<GroupValidationAttribute>()
                    .Where(att => includeAllValidationAttributes || att.UseInImplicitValidation)
                    .Distinct()
                    .ToArray();
            var asyncGroupValidationAttributes =
                this.entityToValidate.GetType()
                    .GetTypeInfo()
                    .GetCustomAttributes()
                    .OfType<IGroupValidationAttribute>()
                    .Where(group => group.CausativePropertyNames.Contains(propertyInfo.Name))
                    .OfType<AsyncValidationAttribute>()
                    .Where(att => includeAllValidationAttributes || att.UseInImplicitValidation)
                    .Distinct()
                    .ToArray();

            // Validate the property
            var isValid = true;
            foreach (var validationAttribute in validationAttributes)
            {
                var validationResult = validationAttribute.GetValidationResult(propertyValue, context);
                if (validationResult != ValidationResult.Success)
                {
                    ValidationMessage validationMessage = null;
                    var extValidationAttribute = validationAttribute as ExtendedValidationAttribute;
                    if (extValidationAttribute != null)
                    {
                        validationMessage = new ValidationMessage(
                            extValidationAttribute.ValidationLevel, validationResult.ErrorMessage, 
                            extValidationAttribute.ShowMessageOnProperty, extValidationAttribute.ShowMessageInSummary);
                    }
                    else
                    {
                        validationMessage = new ValidationMessage(ValidationLevel.Error, validationResult.ErrorMessage);
                    }
                    singlePropertyMatchMessages.Add(validationMessage);
                    isValid = false;
                }
            }

            var isAsyncValid = true;
            foreach (var asyncValidationAttribute in asyncValidationAttributes)
            {
                var validationResult = await asyncValidationAttribute.GetValidationResultAsync(propertyValue, context);
                if (validationResult != ValidationResult.Success)
                {
                    singlePropertyMatchMessages.Add(new ValidationMessage(asyncValidationAttribute.ValidationLevel, validationResult.ErrorMessage, asyncValidationAttribute.ShowMessageOnProperty, asyncValidationAttribute.ShowMessageInSummary));
                    isAsyncValid = false;
                }
            }

            // Validate the property groups
            var isGroupValid = true;
            foreach (var groupValidationAttribute in groupValidationAttributes)
            {
                // Skip attribute if already checked in higher level validation
                if (!groupPropertyMatchMessages.ContainsKey(groupValidationAttribute))
                {
                    var validationResult = groupValidationAttribute.GetValidationResult(this.entityToValidate, context);
                    if (validationResult != ValidationResult.Success)
                    {
                        groupPropertyMatchMessages[groupValidationAttribute] = new ValidationMessage(
                            groupValidationAttribute.ValidationLevel, validationResult.ErrorMessage,
                            groupValidationAttribute.ShowMessageOnProperty, groupValidationAttribute.ShowMessageInSummary);
                        isGroupValid = false;
                    }
                    else
                    {
                        // must be always set to be reset if required
                        groupPropertyMatchMessages[groupValidationAttribute] = null;
                    }
                }
            }

            var isAsyncGroupValid = true;
            foreach (var asyncGroupValidationAttribute in asyncGroupValidationAttributes)
            {
                var castedGroupAttribute = asyncGroupValidationAttribute as IGroupValidationAttribute;
                // Skip attribute if already checked in higher level validation
                if (!groupPropertyMatchMessages.ContainsKey(castedGroupAttribute))
                {
                    var validationResult = await asyncGroupValidationAttribute.GetValidationResultAsync(this.entityToValidate, context);
                    if (validationResult != ValidationResult.Success)
                    {
                        groupPropertyMatchMessages[castedGroupAttribute] = new ValidationMessage(
                            asyncGroupValidationAttribute.ValidationLevel, validationResult.ErrorMessage,
                            asyncGroupValidationAttribute.ShowMessageOnProperty, asyncGroupValidationAttribute.ShowMessageInSummary);
                        isAsyncGroupValid = false;
                    }
                    else
                    {
                        // must be always set to be reset if required
                        groupPropertyMatchMessages[castedGroupAttribute] = null;
                    }
                }
            }

            return isValid && isAsyncValid && isGroupValid && isAsyncGroupValid;
        }

        /// <summary>
        /// Updates the messages collection of the property.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <param name="newPropertyMessage"> The new property message. </param>
        /// <returns> True if the property messages have changed. Otherwise, false. </returns>
        private bool SetPropertyMessage(string propertyName, ValidationMessage newPropertyMessage)
        {
            return this.SetPropertyMessage(propertyName, newPropertyMessage, true);
        }

        /// <summary>
        /// Updates the messages collection of the property.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <param name="newPropertyMessage"> The new property message. </param>
        /// <param name="replaceMessages"> Indicates if old messages will be replaced. </param>
        /// <returns> True if the property messages have changed. Otherwise, false. </returns>
        private bool SetPropertyMessage(string propertyName, ValidationMessage newPropertyMessage, bool replaceMessages)
        {
            Guard.AssertNotNull(newPropertyMessage, "newPropertyMessage");

            return this.SetPropertyMessages(propertyName, new List<ValidationMessage> { newPropertyMessage }, replaceMessages);
        }

        /// <summary>
        /// Updates the messages collection of the property. New messages will be added to old messages.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <param name="newPropertyMessages"> The new collection of property messages. </param>
        /// <returns> True if the property messages have changed. Otherwise, false. </returns>
        private bool SetPropertyMessages(string propertyName, IList<ValidationMessage> newPropertyMessages)
        {
            return this.SetPropertyMessages(propertyName, newPropertyMessages, true);
        }

        /// <summary>
        /// Updates the messages collection of the property. Old messages will be replaced by new messages or new messages will be added.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <param name="newPropertyMessages"> The new collection of property messages. </param>
        /// <param name="replaceMessages"> Indicates if old messages will be replaced. </param>
        /// <returns> True if the property messages have changed. Otherwise, false. </returns>
        private bool SetPropertyMessages(string propertyName, IList<ValidationMessage> newPropertyMessages, bool replaceMessages)
        {
            Guard.AssertNotNull(newPropertyMessages, "newPropertyMessages");

            lock (this.propertyMessages)
            {
                bool messagesChanged = false;

                // If the property does not have messages, simply add them
                if (!this.propertyMessages.ContainsKey(propertyName))
                {
                    if (newPropertyMessages.Count > 0)
                    {
                        this.propertyMessages.Add(propertyName, new ReadOnlyCollection<ValidationMessage>(newPropertyMessages));
                        messagesChanged = true;
                    }
                }
                else
                {
                    var unionMessages = this.propertyMessages[propertyName].Union(newPropertyMessages).Distinct().ToList();

                    // If the property has messages, check if the messages are different.
                    // If messages are different, add the new ones.
                    if (newPropertyMessages.Count != this.propertyMessages[propertyName].Count ||
                        unionMessages.Count != this.propertyMessages[propertyName].Count)
                    {
                        if (newPropertyMessages.Count > 0)
                        {
                            if (replaceMessages)
                            {
                                this.propertyMessages[propertyName] = new ReadOnlyCollection<ValidationMessage>(newPropertyMessages);
                            }
                            else
                            {
                                this.propertyMessages[propertyName] = new ReadOnlyCollection<ValidationMessage>(unionMessages);
                            }
                        }
                        else
                        {
                            this.propertyMessages.Remove(propertyName);
                        }

                        messagesChanged = true;
                    }
                }

                return messagesChanged;
            }
        }

        /// <summary>
        /// Updates the messages collection of the property group.
        /// </summary>
        /// <param name="propertyGroup"> The property group validation attribute instance. </param>
        /// <param name="newMessage"> The new message to set for this group. </param>
        /// <returns> True if the property group message has changed. Otherwise, false. </returns>
        private bool SetPropertyGroupMessage(IGroupValidationAttribute propertyGroup, ValidationMessage newMessage)
        {
            Guard.AssertNotNull(propertyGroup, "propertyGroup");

            lock (this.groupPropertyMessages)
            {
                var messageChanged = false;

                // If the property does not have a message, simply add them
                if (!this.groupPropertyMessages.ContainsKey(propertyGroup))
                {
                    if (newMessage != null)
                    {
                        this.groupPropertyMessages.Add(propertyGroup, newMessage);
                        messageChanged = true;
                    }
                }
                else
                {
                    // If the group has a message, check if the message differs.
                    if (this.groupPropertyMessages[propertyGroup] != newMessage)
                    {
                        if (newMessage != null)
                        {
                            this.groupPropertyMessages[propertyGroup] = newMessage;
                        }
                        else
                        {
                            this.groupPropertyMessages.Remove(propertyGroup);
                        }

                        messageChanged = true;
                    }
                }

                return messageChanged;
            }
        }

        /// <summary>
        /// Sets a validation message for a whole entity.
        /// </summary>
        /// <param name="message">Validation message.</param>
        /// <returns><c>true</c>, if the message has been added. <c>false</c>, if it was already there.</returns>
        private bool SetEntityMessage(ValidationMessage message)
        {
            Guard.AssertNotNull(message, "message");

            return this.SetPropertyGroupMessage(GenericEntityValidationKey, message);
        }

        /// <summary>
        /// Notifies listeners that the messages of a property have changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.</param>
        private void NotifyPropertyMessagesChanged(string propertyName)
        {
            var eventHandler = this.PropertyMessagesChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Adds validation message for a property to a certain dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary, where the message should be added.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="message">The validation message to add.</param>
        /// <param name="messageKind">The validation message kind (property, entity).</param>
        private void AddPropertyValidationMessage(
            Dictionary<string, List<ValidationMessage>> dictionary, 
            string propertyName, 
            ValidationMessage message, 
            ValidationMessageKind messageKind)
        {
            Guard.AssertNotNull(dictionary, "dictionary");
            Guard.AssertNotNull(message, "message");


            // if the property is already in the dictionary, just add the new message; else, add a new key/value pair
            if (dictionary.ContainsKey(propertyName))
            {
                dictionary[propertyName].Add(new ValidationMessage(message, messageKind));
            }
            else
            {
                dictionary.Add(propertyName, new List<ValidationMessage> { new ValidationMessage(message, messageKind) });
            }
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Private helper class to identify validation messages in the validation dictionary that
        /// belong to the entire entity instead of single properties.
        /// </summary>
        private class GenericEntityKeyValidationAttribute : GroupValidationAttribute
        {
            protected override IEnumerable<Expression<Func<object>>> AffectedProperties { get { return new List<Expression<Func<object>>>(); } }
            protected override IEnumerable<Expression<Func<object>>> CausativeProperties { get { return new List<Expression<Func<object>>>(); } }
            protected override ValidationResult IsValid(object value, ValidationContext validationContext) { return new ValidationResult("Error"); }
        }

        #endregion
    }
}