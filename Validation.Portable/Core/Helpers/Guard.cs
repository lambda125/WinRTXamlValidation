// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Guard.cs" company="-" year="2013">
//   Alexander Jung (alexander.jung@sdx-ag.de)
//   Matthias Jauernig (matthias.jauernig@live.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Core.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Helper class to provide parameter checks.
    /// </summary>
    /// <remarks>
    /// Authors: (SDX AG, http://www.sdx-ag.de)
    ///  * Alexander Jung, http://ajdotnet.wordpress.com
    ///  * Matthias Jauernig, http://www.minddriven.de
    /// </remarks>
    public static class Guard
    {
        /// <summary>
        /// Value for output if the param name is null.
        /// </summary>
        private const string NullParam = "(no param)";

        /// <summary>
        /// Value for output if the param value is null.
        /// </summary>
        private const string NullValue = "(null)";

        /// <summary>
        /// Checks if a given value is not null and throws a respective exception otherwise.
        /// </summary>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        [DebuggerStepThrough]
        public static void AssertNotNull(object arg, string paramName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(
                    paramName, "Argument ‘" + (paramName ?? NullParam) + "’ should not be NULL!");
            }
        }

        /// <summary>
        /// Checks if a given value is not null and throws a respective exception otherwise.
        /// </summary>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">A message that describes the error. </param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        [DebuggerStepThrough]
        public static void AssertNotNull(object arg, string paramName, string message)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(paramName, message);
            }
        }

        /// <summary>
        /// Checks if a given value is not null and throws a respective exception otherwise.
        /// </summary>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An Object array containing zero or more objects to format. </param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        [DebuggerStepThrough]
        public static void AssertNotNull(object arg, string paramName, string format, params object[] args)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(paramName, SafeFormat(format, args));
            }
        }

        /// <summary>
        /// Checks if a given value is not null and throws a respective exception otherwise.
        /// </summary>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is empty.</exception>
        [DebuggerStepThrough]
        public static void AssertNotEmpty(string arg, string paramName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(
                    paramName, "Argument ‘" + (paramName ?? NullParam) + "’ should not be NULL!");
            }

            if (arg.Length == 0)
            {
                throw new ArgumentOutOfRangeException(
                    paramName, arg, "Argument ‘" + (paramName ?? NullParam) + "’ should not be empty!");
            }
        }

        /// <summary>
        /// Checks if a given value is neither null nor empty and throws a respective exception otherwise.
        /// </summary>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">A message that describes the error. </param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is empty.</exception>
        [DebuggerStepThrough]
        public static void AssertNotEmpty(string arg, string paramName, string message)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(paramName, message);
            }

            if (arg.Length == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, arg, message);
            }
        }

        /// <summary>
        /// Checks if a given value is neither null nor empty and throws a respective exception otherwise.
        /// </summary>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An Object array containing zero or more objects to format. </param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is empty.</exception>
        [DebuggerStepThrough]
        public static void AssertNotEmpty(string arg, string paramName, string format, params object[] args)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(paramName, SafeFormat(format, args));
            }

            if (arg.Length == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, arg, SafeFormat(format, args));
            }
        }

        /// <summary>
        /// Checks if a given value is neither null nor empty and throws a respective exception otherwise.
        /// </summary>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is empty.</exception>
        [DebuggerStepThrough]
        public static void AssertNotEmpty(ICollection arg, string paramName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(
                    paramName, "Argument ‘" + (paramName ?? NullParam) + "’ should not be NULL!");
            }

            if (arg.Count == 0)
            {
                throw new ArgumentOutOfRangeException(
                    paramName, arg, "Argument ‘" + (paramName ?? NullParam) + "’ should not be empty!");
            }
        }

        /// <summary>
        /// Checks if a given value is neither null nor empty and throws a respective exception otherwise.
        /// </summary>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <typeparam name="T">Collection element type.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is empty.</exception>
        [DebuggerStepThrough]
        public static void AssertNotEmpty<T>(IEnumerable<T> arg, string paramName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(
                    paramName, "Argument ‘" + (paramName ?? NullParam) + "’ should not be NULL!");
            }

            if (!arg.Any())
            {
                throw new ArgumentOutOfRangeException(
                    paramName, arg, "Argument ‘" + (paramName ?? NullParam) + "’ should not be empty!");
            }
        }

        /// <summary>
        /// Checks if a given value is neither null nor empty and throws a respective exception otherwise.
        /// </summary>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">A message that describes the error. </param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is empty.</exception>
        [DebuggerStepThrough]
        public static void AssertNotEmpty(ICollection arg, string paramName, string message)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(paramName, message);
            }

            if (arg.Count == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, arg, message);
            }
        }

        /// <summary>
        /// Checks if a given value is neither null nor empty and throws a respective exception otherwise.
        /// </summary>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An Object array containing zero or more objects to format. </param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is empty.</exception>
        [DebuggerStepThrough]
        public static void AssertNotEmpty(ICollection arg, string paramName, string format, params object[] args)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(paramName, SafeFormat(format, args));
            }

            if (arg.Count == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, arg, SafeFormat(format, args));
            }
        }

        /// <summary>
        /// Checks if a given value is neither null nor empty and throws a respective exception otherwise.
        /// </summary>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is empty.</exception>
        [DebuggerStepThrough]
        public static void AssertNotEmpty(Guid arg, string paramName)
        {
            if (arg == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(
                    paramName, arg, "Argument ‘" + (paramName ?? NullParam) + "’ should not be empty!");
            }
        }

        /// <summary>
        /// Checks if a given value is neither null nor empty and throws a respective exception otherwise.
        /// </summary>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">A message that describes the error. </param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is empty.</exception>
        [DebuggerStepThrough]
        public static void AssertNotEmpty(Guid arg, string paramName, string message)
        {
            if (arg == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(paramName, arg, message);
            }
        }

        /// <summary>
        /// Checks if a given value is neither null nor empty and throws a respective exception otherwise.
        /// </summary>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An Object array containing zero or more objects to format. </param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is empty.</exception>
        [DebuggerStepThrough]
        public static void AssertNotEmpty(Guid arg, string paramName, string format, params object[] args)
        {
            if (arg == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(paramName, arg, SafeFormat(format, args));
            }
        }

        /// <summary>
        /// Checks if a given condition is met and throws a respective exception otherwise.
        /// </summary>
        /// <param name="condition">The condition that has to be true, otherwise an exception is thrown.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An Object array containing zero or more objects to format. </param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is empty.</exception>
        [DebuggerStepThrough]
        public static void AssertCondition(
            bool condition, string paramName, object arg, string format, params object[] args)
        {
            if (!condition)
            {
                throw new ArgumentOutOfRangeException(paramName, arg ?? NullValue, SafeFormat(format, args));
            }
        }

        /// <summary>
        /// Checks if a given condition is met and throws a respective exception otherwise.
        /// </summary>
        /// <param name="condition">The condition that has to be true, otherwise an exception is thrown.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <param name="message">A message that describes the error. </param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is empty.</exception>
        [DebuggerStepThrough]
        public static void AssertCondition(bool condition, string paramName, object arg, string message)
        {
            if (!condition)
            {
                throw new ArgumentOutOfRangeException(paramName, arg ?? NullValue, message);
            }
        }

        /// <summary>
        /// Checks if a given condition is met and throws a respective exception otherwise.
        /// </summary>
        /// <param name="condition">The condition that has to be true, otherwise an exception is thrown.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="arg">The value of the parameter that should be checked.</param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is empty.</exception>
        [DebuggerStepThrough]
        public static void AssertCondition(bool condition, string paramName, object arg)
        {
            if (!condition)
            {
                throw new ArgumentOutOfRangeException(paramName, arg ?? NullValue, "Condition not met!");
            }
        }

        /// <summary>
        /// Helper method: Replaces <see cref="String.Format"/>.
        /// Replaces each format item in a specified String with the text equivalent of
        /// a corresponding Object instance in a specified array.
        /// </summary>
        /// <remarks>
        /// Contrary to <see cref="String.Format"/>, this method does not throw an exception.
        /// Rather in case of an error it returns a descriptive string describing that error.
        /// This is done to avoid having a subsequent string format error obscure the original
        /// error that caused this call in the first place.
        /// </remarks>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An Object array containing zero or more objects to format. </param>
        /// <returns>A copy of format in which the format items have been replaced by the String
        /// equivalent of the corresponding instances of Object in args. </returns>
        private static string SafeFormat(string format, params object[] args)
        {
            try
            {
                return string.Format(format, args);
            }
            catch (Exception ex)
            {
                return "Format failure: " + ex.Message + Environment.NewLine +
                       format + " with " + args.Length + " arguments.";
            }
        }
    }
}
