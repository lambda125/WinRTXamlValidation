// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyValidationTextBlock.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinRTXAMLValidation.Library.Core;
using WinRTXAMLValidation.Library.Core.Helpers;

namespace WinRTXAMLValidation.Library.Controls
{
    /// <summary>
    /// Simple text block control, which shows validation messages for a single property.
    /// </summary>
    public class PropertyValidationTextBlock : Control
    {
        #region Dependency Properties

        /// <summary>
        /// Dependency property for the <see cref="ValidationSource"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty ValidationSourceProperty =
            DependencyProperty.Register(
                "ValidationSource",
                typeof(ReadOnlyCollection<ValidationMessage>),
                typeof(PropertyValidationTextBlock),
                new PropertyMetadata(BindableValidator.EmptyMessagesCollection, HandleValidationSourceChanged));

        /// <summary>
        /// Dependency property for the <see cref="Errors"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.Register(
                "Errors",
                typeof(IList<ValidationMessage>),
                typeof(PropertyValidationTextBlock),
                null);

        /// <summary>
        /// Dependency property for the <see cref="Warnings"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty WarningsProperty =
            DependencyProperty.Register(
                "Warnings",
                typeof(IList<ValidationMessage>),
                typeof(PropertyValidationTextBlock),
                null);

        /// <summary>
        /// Dependency property for the <see cref="ShowValidationMessages"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty ShowValidationMessagesProperty =
            DependencyProperty.Register(
                "ShowValidationMessages",
                typeof(bool),
                typeof(PropertyValidationTextBlock),
                new PropertyMetadata(true, HandleShowValidationMessagesChanged));

        /// <summary>
        /// Dependency property for the <see cref="ShowPropertyValidationMessages"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty ShowPropertyValidationMessagesProperty =
            DependencyProperty.Register(
                "ShowPropertyValidationMessages",
                typeof(bool),
                typeof(PropertyValidationTextBlock),
                new PropertyMetadata(true, HandleShowPropertyValidationMessagesChanged));

        /// <summary>
        /// Dependency property for the <see cref="ShowGroupValidationMessages"/> property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This field and its type is immutable.")]
        public static readonly DependencyProperty ShowGroupValidationMessagesProperty =
            DependencyProperty.Register(
                "ShowGroupValidationMessages",
                typeof(bool),
                typeof(PropertyValidationTextBlock),
                new PropertyMetadata(true, HandleShowGroupValidationMessagesChanged));

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValidationTextBlock"/> class.
        /// </summary>
        public PropertyValidationTextBlock()
        {
            this.DefaultStyleKey = typeof(PropertyValidationTextBlock);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to show any validation messages in the text block.
        /// </summary>
        public bool ShowValidationMessages
        {
            get { return (bool)this.GetValue(ShowValidationMessagesProperty); }
            set { this.SetValue(ShowValidationMessagesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the source of validation messages.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Due this collection is of the type read only collection, it can have a setter.")]
        public ReadOnlyCollection<ValidationMessage> ValidationSource
        {
            get { return (ReadOnlyCollection<ValidationMessage>)this.GetValue(ValidationSourceProperty); }
            set { this.SetValue(ValidationSourceProperty, value); }
        }

        /// <summary>
        /// Gets the current validation error messages.
        /// </summary>
        public IList<ValidationMessage> Errors
        {
            get { return (IList<ValidationMessage>)this.GetValue(ErrorsProperty); }
            private set { this.SetValue(ErrorsProperty, value); }
        }

        /// <summary>
        /// Gets the current validation warning messages.
        /// </summary>
        public IList<ValidationMessage> Warnings
        {
            get { return (IList<ValidationMessage>)this.GetValue(WarningsProperty); }
            private set { this.SetValue(WarningsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show property-based validation messages.
        /// </summary>
        public bool ShowPropertyValidationMessages
        {
            get { return (bool)this.GetValue(ShowPropertyValidationMessagesProperty); }
            set { this.SetValue(ShowPropertyValidationMessagesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show group property based validation messages.
        /// </summary>
        public bool ShowGroupValidationMessages
        {
            get { return (bool)this.GetValue(ShowGroupValidationMessagesProperty); }
            set { this.SetValue(ShowGroupValidationMessagesProperty, value); }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Invoked when the validation source dependency property changes.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event arguments.</param>
        private static void HandleValidationSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PropertyValidationTextBlock;
            if (control != null)
            {
                SetValidationMessages(control, (ReadOnlyCollection<ValidationMessage>)e.NewValue, control.ShowValidationMessages, control.ShowPropertyValidationMessages, control.ShowGroupValidationMessages);
            }
        }

        /// <summary>
        /// Invoked when the show validation messages dependency property changes.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event arguments.</param>
        private static void HandleShowValidationMessagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PropertyValidationTextBlock;
            if (control != null)
            {
                SetValidationMessages(control, control.ValidationSource, (bool)e.NewValue, control.ShowPropertyValidationMessages, control.ShowGroupValidationMessages);
            }
        }

        /// <summary>
        /// Invoked when the show property validation messages dependency property changes.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event arguments.</param>
        private static void HandleShowPropertyValidationMessagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PropertyValidationTextBlock;
            if (control != null)
            {
                SetValidationMessages(control, control.ValidationSource, control.ShowValidationMessages, (bool)e.NewValue, control.ShowGroupValidationMessages);
            }
        }

        /// <summary>
        /// Invoked when the show group validation messages dependency property changes.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event arguments.</param>
        private static void HandleShowGroupValidationMessagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PropertyValidationTextBlock;
            if (control != null)
            {
                SetValidationMessages(control, control.ValidationSource, control.ShowValidationMessages, control.ShowPropertyValidationMessages, (bool)e.NewValue);
            }
        }

        /// <summary>
        /// Pushes the validation messages into the text block.
        /// </summary>
        /// <param name="control">The control to set the collections to. </param>
        /// <param name="validationSource">The errors enumeration. </param>
        /// <param name="showValidationMessages">The value indicating whether to show any validation messages.</param>
        /// <param name="showPropertyValidationMessages">The value indicating whether to show property-based messages.</param>
        /// <param name="showGroupValidationMessages">The value indicating whether to show group property based messages.</param>
        private static void SetValidationMessages(
            PropertyValidationTextBlock control,
            ReadOnlyCollection<ValidationMessage> validationSource,
            bool showValidationMessages,
            bool showPropertyValidationMessages,
            bool showGroupValidationMessages)
        {
            Guard.AssertNotNull(control, "control");
            Guard.AssertNotNull(validationSource, "validationSource");

            if (!showValidationMessages)
            {
                showPropertyValidationMessages = false;
                showGroupValidationMessages = false;
            }

            // find errors and show the appropriate messages
            control.Errors = 
                validationSource.Where(msg =>
                    msg.ShowMessageOnProperty &&
                    (msg.ValidationLevel == ValidationLevel.Error) &&
                    (showPropertyValidationMessages || msg.ValidationMessageKind != ValidationMessageKind.Property) &&
                    (showGroupValidationMessages || msg.ValidationMessageKind != ValidationMessageKind.Overall)).ToList();

            // find warnings and show the appropriate messages
            control.Warnings =
                validationSource.Where(msg =>
                    msg.ShowMessageOnProperty &&
                    (msg.ValidationLevel == ValidationLevel.Warning) &&
                    (showPropertyValidationMessages || msg.ValidationMessageKind != ValidationMessageKind.Property) &&
                    (showGroupValidationMessages || msg.ValidationMessageKind != ValidationMessageKind.Overall)).ToList();
        }

        #endregion
    }
}
