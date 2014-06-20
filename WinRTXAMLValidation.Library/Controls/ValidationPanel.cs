// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationPanel.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Controls
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    using WinRTXAMLValidation.Library.Core;
    using WinRTXAMLValidation.Library.Core.Helpers;

    /// <summary>
    /// Control which wraps another control to add visual validation information. If a value
    /// becomes invalid (<see cref="Errors"/> or <see cref="Warnings"/> is not empty), a red border will be drawn
    /// around the inner control. Additionally, the list of validation errors will be shown below.
    /// </summary>
    [TemplateVisualState(GroupName = "CommonStates", Name = "Valid")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Error")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Warning")]
    public sealed class ValidationPanel : ContentControl
    {
        #region Dependency Properties

        /// <summary>
        /// Dependency property for the <see cref="ValidationSource"/> property.
        /// </summary>
        public static readonly DependencyProperty ValidationSourceProperty =
            DependencyProperty.Register(
                "ValidationSource",
                typeof(ReadOnlyCollection<ValidationMessage>),
                typeof(ValidationPanel),
                new PropertyMetadata(BindableValidator.EmptyMessagesCollection, HandleValidationSourcePropertyChanged));

        /// <summary>
        /// Dependency property for the <see cref="ShowValidationMessages"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowValidationMessagesProperty =
            DependencyProperty.Register(
                "ShowValidationMessages",
                typeof(bool),
                typeof(ValidationPanel),
                new PropertyMetadata(true));

        /// <summary>
        /// Dependency property for the <see cref="ShowPropertyValidationMessages"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowPropertyValidationMessagesProperty =
            DependencyProperty.Register(
                "ShowPropertyValidationMessages",
                typeof(bool),
                typeof(ValidationPanel),
                new PropertyMetadata(true));

        /// <summary>
        /// Dependency property for the <see cref="ShowGroupValidationMessages"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowGroupValidationMessagesProperty =
            DependencyProperty.Register(
                "ShowGroupValidationMessages",
                typeof(bool),
                typeof(ValidationPanel),
                new PropertyMetadata(true));

        #endregion

        #region Fields

        /// <summary>
        /// The original border brush of the inner control
        /// (will be replaced when a red border is shown because of invalid data).
        /// </summary>
        private Brush originalControlBorderBrush;

        /// <summary>
        /// The original border thickness of the inner control
        /// (will be replaced when a red border is shown because of invalid data).
        /// </summary>
        private Thickness originalControlBorderThickness = new Thickness(0);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationPanel"/> class.
        /// </summary>
        public ValidationPanel()
        {
            this.DefaultStyleKey = typeof(ValidationPanel);
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets validation messages source.
        /// </summary>
        public ReadOnlyCollection<ValidationMessage> ValidationSource
        {
            get { return (ReadOnlyCollection<ValidationMessage>)this.GetValue(ValidationSourceProperty); }
            set { this.SetValue(ValidationSourceProperty, value); }
        }

        /// <summary>
        /// Gets the Error validation messages.
        /// </summary>
        public IList<ValidationMessage> Errors
        {
            get { return this.ValidationSource.Where(msg => msg.ValidationLevel == ValidationLevel.Error).ToList(); }
        }

        /// <summary>
        /// Gets the Warning validation messages.
        /// </summary>
        public IList<ValidationMessage> Warnings
        {
            get { return this.ValidationSource.Where(msg => msg.ValidationLevel == ValidationLevel.Warning).ToList(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether any validation messages should be shown or not.
        /// </summary>
        public bool ShowValidationMessages
        {
            get { return (bool)this.GetValue(ShowValidationMessagesProperty); }
            set { this.SetValue(ShowValidationMessagesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether property-based validation messages should be shown or not.
        /// </summary>
        public bool ShowPropertyValidationMessages
        {
            get { return (bool)this.GetValue(ShowPropertyValidationMessagesProperty); }
            set { this.SetValue(ShowPropertyValidationMessagesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether group property based validation messages should be shown or not.
        /// </summary>
        public bool ShowGroupValidationMessages
        {
            get { return (bool)this.GetValue(ShowGroupValidationMessagesProperty); }
            set { this.SetValue(ShowGroupValidationMessagesProperty, value); }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Invoked when the control has been loaded. In this case the control UI
        /// will update its validation state.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="routedEventArgs">Event arguments.</param>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdateValidationInformation(this);
        }

        /// <summary>
        /// Invoked when the inner content changed. In this case, if the content is a control, 
        /// the control's border information will be stored, hence it could be restored later.
        /// </summary>
        /// <param name="oldContent"> The old content. </param>
        /// <param name="newContent"> The new content. </param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            var control = newContent as Control;
            if (control != null)
            {
                this.originalControlBorderBrush = control.BorderBrush;
                this.originalControlBorderThickness = control.BorderThickness;
            }

            base.OnContentChanged(oldContent, newContent);
        }

        /// <summary>
        /// Invoked when the collection of validation messages changed. In this case, 
        /// the control will change its state to render the validation information.
        /// </summary>
        /// <param name="d"> The dependency object. </param>
        /// <param name="e"> The event arguments. </param>
        private static void HandleValidationSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Guard.AssertNotNull(d, "d");
            Guard.AssertCondition(d is ValidationPanel, "d", d);

            UpdateValidationInformation((ValidationPanel)d);
        }

        /// <summary>
        /// Updates the validation information to show on the control.
        /// </summary>
        /// <param name="panel"><c>ValidationPanel</c> that should be rendered.</param>
        private static void UpdateValidationInformation(ValidationPanel panel)
        {
            // go to the appropriate validation state
            VisualStateManager.GoToState(
              panel,
              ComputeVisualState(panel.Errors.Any(), panel.Warnings.Any()),
              true);

            var control = panel.Content as Control;
            if (control != null)
            {
                // state is invalid: clear the border information (colored border is drawn around);
                // state is valid:   restore the initial border information of the control
                var isValid = !panel.ValidationSource.Any();
                control.BorderBrush = isValid ? panel.originalControlBorderBrush : null;
                control.BorderThickness = isValid ? panel.originalControlBorderThickness : new Thickness(0);
            }
        }

        /// <summary>
        /// Computes the visual state to represent the validation state of the control.
        /// </summary>
        /// <param name="hasErrors"> The indicator whether there are errors. </param>
        /// <param name="hasWarnings"> The indicator whether there are warnings. </param>
        /// <returns> The <see cref="string"/> containing the name of the visual state. </returns>
        private static string ComputeVisualState(bool hasErrors, bool hasWarnings)
        {
            return hasErrors
                ? "Error"
                : (hasWarnings ? "Warning" : "Valid");
        }

        #endregion
    }
}
