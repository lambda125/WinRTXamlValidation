// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateCommand.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Demo.Common
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Delegate command as implementation of <see cref="ICommand"/> for UI binding.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        /// <summary>
        /// The action to perform.
        /// </summary>
        private readonly Action<object> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        public DelegateCommand(Action action)
        {
            this.action = (o => action());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        public DelegateCommand(Action<object> action)
        {
            this.action = action;
        }

        /// <summary>
        /// Fired when the <see cref="CanExecute"/> property changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Indicates if the command can be executed.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        /// <returns><c>true</c>, if the command can be executed.</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        public void Execute(object parameter)
        {
            this.action(parameter);
        }
    }
}
