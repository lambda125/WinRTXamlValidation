// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationMessage.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Core
{
    /// <summary>
    /// The validation message class, which represents a message occurred during validation.
    /// </summary>
    public class ValidationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationMessage"/> class.
        /// </summary>
        /// <param name="validationLevel">
        /// The validation level (Error, Warning).
        /// </param>
        /// <param name="messageText">
        /// Validation message text.
        /// </param>
        /// <param name="showMessageOnProperty">
        /// Indicates whether the validation message will be shown on the property at the UI.
        /// </param>
        /// <param name="showMessageInSummary">
        /// Indicates whether the validation message will be shown in the validation summary at the UI.
        /// </param>
        /// <param name="validationMessageKind">
        /// The validation message kind which indicates what kind of validation 
        /// has been performed (property or entity based).
        /// </param>
        public ValidationMessage(
            ValidationLevel validationLevel,
            string messageText,
            bool showMessageOnProperty = true,
            bool showMessageInSummary = true,
            ValidationMessageKind? validationMessageKind = null)
        {
            this.ValidationLevel = validationLevel;
            this.MessageText = messageText;
            this.ShowMessageOnProperty = showMessageOnProperty;
            this.ShowMessageInSummary = showMessageInSummary;
            this.ValidationMessageKind = validationMessageKind;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationMessage"/> class.
        /// </summary>
        /// <param name="validationLevel">
        /// The validation level (Error, Warning).
        /// </param>
        /// <param name="messageText">
        /// Validation message text.
        /// </param>
        /// <param name="validationMessageKind">
        /// The validation message kind which indicates what kind of validation 
        /// has been performed (property or entity based).
        /// </param>
        public ValidationMessage(ValidationLevel validationLevel, string messageText, ValidationMessageKind validationMessageKind)
        {
            this.ValidationLevel = validationLevel;
            this.MessageText = messageText;
            this.ValidationMessageKind = validationMessageKind;
            this.ShowMessageOnProperty = true;
            this.ShowMessageInSummary = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationMessage"/> class using an existing validation message.
        /// </summary>
        /// <param name="message">
        /// Existing validation message, whose properties should be taken.
        /// </param>
        /// <param name="validationMessageKind">
        /// The validation message kind which indicates what kind of validation has been performed (property or entity based).
        /// If no value is provided, <c>message.ValidationMessageKind</c> will be taken.
        /// </param>
        public ValidationMessage(ValidationMessage message, ValidationMessageKind? validationMessageKind = null)
            : this(
                message.ValidationLevel,
                message.MessageText,
                message.ShowMessageOnProperty,
                message.ShowMessageInSummary,
                validationMessageKind ?? message.ValidationMessageKind)
        {
        }

        /// <summary>
        /// Gets the validation level (Error, Warning).
        /// </summary>
        public ValidationLevel ValidationLevel { get; private set; }

        /// <summary>
        /// Gets the validation message text.
        /// </summary>
        public string MessageText { get; private set; }

        /// <summary>
        /// Gets the validation message kind (property or entity based).
        /// </summary>
        public ValidationMessageKind? ValidationMessageKind { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the validation message 
        /// should be shown on the property field at the UI (corresponding ValidationPanel).
        /// </summary>
        public bool ShowMessageOnProperty { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the validation message 
        /// should be shown in the validation summary at the UI (corresponding ValidationSummary).
        /// </summary>
        public bool ShowMessageInSummary { get; private set; }
    }
}