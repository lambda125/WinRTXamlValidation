// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionExtensions.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Core.Extensions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    using WinRTXAMLValidation.Library.Core.Helpers;

    /// <summary>
    /// Extension methods for Expressions.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Extracts the property name from the expression.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="propertySelector">The property selector.</param>
        /// <returns>Name of the property.</returns>
        public static string ExtractPropertyName<T>(this Expression<Func<T>> propertySelector)
        {
            Guard.AssertNotNull(propertySelector, "propertySelector");

            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = propertySelector.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("The expression is not a member access expression.", "propertySelector");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property.", "propertySelector");
            }

            return memberExpression.Member.Name;
        }
    }
}
