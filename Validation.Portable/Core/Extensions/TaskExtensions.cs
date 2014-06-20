// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskExtensions.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Core.Extensions
{
    using System;
    using System.Threading.Tasks;

    using WinRTXAMLValidation.Library.Core.Helpers;

    /// <summary>
    /// Extension methods for the <see cref="Task"/> class.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Continues the execution after finishing this task in any case.
        /// </summary>
        /// <typeparam name="T"> The result type of the task. </typeparam>
        /// <param name="task"> The task to await. </param>
        /// <param name="nextTask"> The function factory for task to continue with. </param>
        /// <param name="ignoreExceptions"> The indicator whether to ignore exceptions or not. </param>
        /// <returns> The <see cref="Task"/> which is waiting and continuing. </returns>
        public static async Task<T> ContinueWithAsync<T>(this Task<T> task, Func<Task<T>> nextTask, bool ignoreExceptions = true)
        {
            Guard.AssertNotNull(task, "task");
            Guard.AssertNotNull(task, "nextTask");

            try
            {
                await task;
            }
            catch
            {
                // Catching any exception so that the next task can continue if intended
                if (!ignoreExceptions)
                {
                    throw;
                }
            }

            return await nextTask();
        }
    }
}
