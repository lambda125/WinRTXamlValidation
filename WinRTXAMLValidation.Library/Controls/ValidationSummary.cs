// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationSummary.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using WinRTXAMLValidation.Library.Core;
using WinRTXAMLValidation.Library.Core.Extensions;
using WinRTXAMLValidation.Library.Core.Helpers;

namespace WinRTXAMLValidation.Library.Controls
{
    /// <summary>
    /// Validation summary control, which offers the opportunity to show all validation messages of an entity.
    /// </summary>
    public class ValidationSummary : Control
    {
        #region Dependency Properties

        /// <summary>
        /// Dependency property for the <see cref="ValidationSource"/> property.
        /// The property type is object because WinRT doesn't support XAML binding to generic dependency properties.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty ValidationSourceProperty =
            DependencyProperty.Register(
                "ValidationSource",
                typeof(object),
                typeof(ValidationSummary),
                new PropertyMetadata(BindableValidator.EmptyMessagesDictionary, HandleValidationSourceChanged));

        /// <summary>
        /// Dependency property for the <see cref="Errors"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.Register(
                "Errors",
                typeof(IList<ValidationMessage>),
                typeof(ValidationSummary),
                null);

        /// <summary>
        /// Dependency property for the <see cref="Warnings"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty WarningsProperty =
            DependencyProperty.Register(
                "Warnings",
                typeof(IList<ValidationMessage>),
                typeof(ValidationSummary),
                null);

        /// <summary>
        /// Dependency property for the <see cref="ShowPropertyValidationMessages"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty ShowPropertyValidationMessagesProperty =
            DependencyProperty.Register(
                "ShowPropertyValidationMessages",
                typeof(bool),
                typeof(ValidationSummary),
                new PropertyMetadata(true, HandleShowPropertyValidationMessagesChanged));

        /// <summary>
        /// Dependency property for the <see cref="ShowGroupValidationMessages"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty ShowGroupValidationMessagesProperty =
            DependencyProperty.Register(
                "ShowGroupValidationMessages",
                typeof(bool),
                typeof(ValidationSummary),
                new PropertyMetadata(true, HandleShowGroupValidationMessagesChanged));

        /// <summary>
        /// Dependency property for the <see cref="AffectedPropertyNames"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty AffectedPropertyNamesProperty =
            DependencyProperty.Register(
                "AffectedPropertyNames",
                typeof(IEnumerable<Binding>),
                typeof(ValidationSummary),
                new PropertyMetadata(new List<Binding>(), HandleAffectedPropertyNamesChanged));

        /// <summary>
        /// Dependency property for the <see cref="Header"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof(string),
                typeof(ValidationSummary),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Dependency property for the <see cref="ShowHeader"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty ShowHeaderProperty =
            DependencyProperty.Register(
                "ShowHeader",
                typeof(bool),
                typeof(ValidationSummary),
                new PropertyMetadata(true));

        /// <summary>
        /// Dependency property for the <see cref="HeaderVisibility"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty HeaderVisibilityProperty =
            DependencyProperty.Register(
                "HeaderVisibility",
                typeof(Visibility),
                typeof(ValidationSummary),
                new PropertyMetadata(Visibility.Collapsed));

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationSummary"/> class.
        /// </summary>
        public ValidationSummary()
        {
            this.DefaultStyleKey = typeof(ValidationSummary);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the validation messages source.
        /// The property type is object because WinRT doesn't support XAML binding to generic dependency properties.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Nesting is required here to specify the specific type of collection that is used here. Inheritance is no alternative.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Due this collection is of the type read only collection, it can have a setter.")]
        public object ValidationSource
        {
            get { return this.GetValue(ValidationSourceProperty); }
            set { this.SetValue(ValidationSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the typed validation messages source.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Nesting is required here to specify the specific type of collection that is used here. Inheritance is no alternative.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Due this collection is of the type read only collection, it can have a setter.")]
        public ReadOnlyDictionary<string, ReadOnlyCollection<ValidationMessage>> TypedValidationSource
        {
            get { return (ReadOnlyDictionary<string, ReadOnlyCollection<ValidationMessage>>)this.GetValue(ValidationSourceProperty); }
            set { this.SetValue(ValidationSourceProperty, value); }
        }

        /// <summary>
        /// Gets the validation error messages.
        /// </summary>
        public IList<ValidationMessage> Errors
        {
            get { return (IList<ValidationMessage>)this.GetValue(ErrorsProperty); }
            private set { this.SetValue(ErrorsProperty, value); }
        }

        /// <summary>
        /// Gets the validation warning messages.
        /// </summary>
        public IList<ValidationMessage> Warnings
        {
            get { return (IList<ValidationMessage>)this.GetValue(WarningsProperty); }
            private set { this.SetValue(WarningsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show property-based validation messages in the validation summary.
        /// </summary>
        public bool ShowPropertyValidationMessages
        {
            get { return (bool)this.GetValue(ShowPropertyValidationMessagesProperty); }
            set { this.SetValue(ShowPropertyValidationMessagesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show group property based validation messages in the validation summary.
        /// </summary>
        public bool ShowGroupValidationMessages
        {
            get { return (bool)this.GetValue(ShowGroupValidationMessagesProperty); }
            set { this.SetValue(ShowGroupValidationMessagesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a collection of properties for which validation messages should be shown in the validation summary.
        /// If the property value is <c>null</c>, messages for all properties of an entity will be shown.
        /// </summary>
        public IEnumerable<Binding> AffectedPropertyNames
        {
            get { return (IEnumerable<Binding>)this.GetValue(AffectedPropertyNamesProperty); }
            set { this.SetValue(AffectedPropertyNamesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the header, that should be shown for the validation summary.
        /// </summary>
        public string Header
        {
            get { return (string)this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a validation summary header should be shown.
        /// </summary>
        public bool ShowHeader
        {
            get { return (bool)this.GetValue(ShowHeaderProperty); }
            set { this.SetValue(ShowHeaderProperty, value); }
        }

        /// <summary>
        /// Gets a value for the visibility of the validation summary header.
        /// </summary>
        public Visibility HeaderVisibility
        {
            get { return (Visibility)this.GetValue(HeaderVisibilityProperty); }
            private set { this.SetValue(HeaderVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the summary has validation messages or not.
        /// </summary>
        public bool HasValidationMessages
        {
            get { return this.TypedValidationSource != null && this.TypedValidationSource.Count > 0; }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Invoked when the validation source dependency property changes.
        /// </summary>
        /// <param name="d"> The dependency object. </param>
        /// <param name="e"> The event arguments. </param>
        private static void HandleValidationSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ValidationSummary;
            if (control != null)
            {
                SetValidationMessages(
                    control,
                    (ReadOnlyDictionary<string, ReadOnlyCollection<ValidationMessage>>)e.NewValue,
                    control.AffectedPropertyNames,
                    control.ShowPropertyValidationMessages,
                    control.ShowGroupValidationMessages);
            }
        }

        /// <summary>
        /// Invoked when the show property validation messages dependency property changes.
        /// </summary>
        /// <param name="d"> The dependency object. </param>
        /// <param name="e"> The event arguments. </param>
        private static void HandleShowPropertyValidationMessagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ValidationSummary;
            if (control != null)
            {
                SetValidationMessages(
                    control,
                    control.TypedValidationSource,
                    control.AffectedPropertyNames,
                    (bool)e.NewValue,
                    control.ShowGroupValidationMessages);
            }
        }

        /// <summary>
        /// Invoked when the show group validation messages dependency property changes.
        /// </summary>
        /// <param name="d"> The dependency object. </param>
        /// <param name="e"> The event arguments. </param>
        private static void HandleShowGroupValidationMessagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ValidationSummary;
            if (control != null)
            {
                SetValidationMessages(
                    control,
                    control.TypedValidationSource,
                    control.AffectedPropertyNames,
                    control.ShowPropertyValidationMessages,
                    (bool)e.NewValue);
            }
        }

        /// <summary>
        /// Invoked when the affected property names dependency property changes.
        /// </summary>
        /// <param name="d"> The dependency object. </param>
        /// <param name="e"> The event arguments. </param>
        private static void HandleAffectedPropertyNamesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ValidationSummary;
            if (control != null)
            {
                SetValidationMessages(
                    control,
                    control.TypedValidationSource,
                    (IEnumerable<Binding>)e.NewValue,
                    control.ShowPropertyValidationMessages,
                    control.ShowGroupValidationMessages);
            }
        }

        /// <summary>
        /// Sets the correctly filtered validation messages into the validation summary.
        /// </summary>
        /// <param name="control"> The control to set the collections to. </param>
        /// <param name="validationSource"> The validation messages dictionary. </param>
        /// <param name="affectedProperties"> The properties to show the messages for. </param>
        /// <param name="showPropertyValidationMessages"> The value indicating whether to show property-based messages. </param>
        /// <param name="showGroupValidationMessages"> The value indicating whether to show group property based messages. </param>
        private static void SetValidationMessages(
            ValidationSummary control,
            ReadOnlyDictionary<string, ReadOnlyCollection<ValidationMessage>> validationSource,
            IEnumerable<Binding> affectedProperties,
            bool showPropertyValidationMessages,
            bool showGroupValidationMessages)
        {
            Guard.AssertNotNull(control, "control");
            Guard.AssertNotNull(validationSource, "validationSource");
            Guard.AssertNotNull(affectedProperties, "affectedProperties");

            IEnumerable<KeyValuePair<string, ReadOnlyCollection<ValidationMessage>>> validationMessages = validationSource;

            // if properties are defined, filter for those properties; else, take all properties (no filtering)
            var affectedPropertiesList = affectedProperties.ToList();
            if (affectedPropertiesList != null && affectedPropertiesList.Count > 0)
            {
                validationMessages = validationMessages
                    .Where(kvp => affectedPropertiesList.Any(binding =>
                        binding.Path.Path.EndsWith("." + kvp.Key, StringComparison.Ordinal) ||
                        binding.Path.Path == kvp.Key));
            }

            // filter for all property- or entity-based errors
            control.Errors = validationMessages
                .SelectMany(kvp => kvp.Value)
                .Where(msg =>
                    msg.ShowMessageInSummary &&
                    (msg.ValidationLevel == ValidationLevel.Error) &&
                    (showPropertyValidationMessages || msg.ValidationMessageKind != ValidationMessageKind.Property) &&
                    (showGroupValidationMessages || msg.ValidationMessageKind != ValidationMessageKind.Overall))
                .Distinct(msg => msg.MessageText)
                .ToList();

            // filter for all property- or entity-based warnings
            control.Warnings = validationMessages
                .SelectMany(kvp => kvp.Value)
                .Where(msg =>
                    msg.ShowMessageInSummary &&
                    (msg.ValidationLevel == ValidationLevel.Warning) &&
                    (showPropertyValidationMessages || msg.ValidationMessageKind != ValidationMessageKind.Property) &&
                    (showGroupValidationMessages || msg.ValidationMessageKind != ValidationMessageKind.Overall))
                .Distinct(msg => msg.MessageText)
                .ToList();

            // display or hide header
            bool showHeader =
                !string.IsNullOrEmpty(control.Header) && control.ShowHeader &&
                ((control.Errors != null && control.Errors.Count > 0) ||
                (control.Warnings != null && control.Warnings.Count > 0));
            control.HeaderVisibility = showHeader ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion
    }
}
