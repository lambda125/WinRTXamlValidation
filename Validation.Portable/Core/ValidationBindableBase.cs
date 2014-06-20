// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationBindableBase.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Core
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// This class adds validation support for model classes that contain validation rules.
    /// The class contains the logic to run the validation rules of the instance of a model class 
    /// and return the results of this validation as a list of errors for properties.
    /// </summary>
    public class ValidationBindableBase : BindableBase
    {
        #region Fields

        /// <summary>
        /// The validator, which contains base validation logic and stores validation messages (warnings and errors).
        /// </summary>
        private readonly BindableValidator bindableValidator;

        /// <summary>
        /// Indicates if a validation is currently in progress.
        /// </summary>
        private bool isValidating;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationBindableBase"/> class.
        /// </summary>
        public ValidationBindableBase()
        {
            this.bindableValidator = new BindableValidator(this);
            this.IsImplicitValidationEnabled = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="BindableValidator"/> instance, which has an indexer property
        /// that returns the validation messages for a validated property.
        /// </summary>
        public BindableValidator ValidationMessages
        {
            get { return this.bindableValidator; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is validation enabled.
        /// </summary>
        public bool IsImplicitValidationEnabled
        {
            get
            {
                return this.bindableValidator.IsImplicitValidationEnabled;
            }

            set
            {
                if (this.bindableValidator.IsImplicitValidationEnabled != value)
                {
                    this.bindableValidator.IsImplicitValidationEnabled = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether a validation is currently in progress.
        /// </summary>
        public bool IsValidating
        {
            get { return this.isValidating; }
            private set { base.SetProperty(ref this.isValidating, value); }
        }

        #endregion

        #region Public/Protected Methods

        /// <summary>
        /// Executes the validation for the current entity.
        /// </summary>
        /// <returns> The <see cref="Task"/>, whose result is <c>true</c> if all properties validation attributes 
        /// pass the validation rules; otherwise, the result is <c>false</c>. </returns>
        public async Task<bool> ValidateAsync()
        {
            this.IsValidating = true;
            try
            {
                return await bindableValidator.ValidateAsync();
            }
            finally
            {
                this.IsValidating = false;
            }
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary. We are overriding this property to ensure that the SetProperty and the ValidateProperty methods are fired in a
        /// deterministic way.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>
        /// True if the value was changed, false if the existing value matched the desired value.
        /// </returns>
        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var result = base.SetProperty(ref storage, value, propertyName);
            if (result && !string.IsNullOrEmpty(propertyName))
            {
                if (this.IsImplicitValidationEnabled)
                {
#pragma warning disable 4014
                    // perform async validation
                    this.ValidatePropertyAsync(propertyName);
#pragma warning restore 4014
                }
            }

            return result;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Validates a given property async.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The <see cref="Task"/> as continuation of the validation process.</returns>
        private async Task ValidatePropertyAsync(string propertyName)
        {
            this.IsValidating = true;
            try
            {
                await this.bindableValidator.ValidatePropertyAsync(propertyName);
            }
            finally
            {
                this.IsValidating = false;
            }
        }

        #endregion
    }
}
