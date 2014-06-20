// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinqExtensions.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using WinRTXAMLValidation.Library.Core.Helpers;

    /// <summary>
    /// The class that provides custom LINQ extensions.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Determines the distinct enumeration by selecting a element member to compare.
        /// </summary>
        /// <typeparam name="TElement"> The enumeration element type. </typeparam>
        /// <typeparam name="TEqualityMember"> The type of the member to compare. </typeparam>
        /// <param name="enumerable"> The enumerable of elements to get as distinct enumeration. </param>
        /// <param name="selector"> The function which selects the member to compare. </param>
        /// <returns> The distinct <see cref="IEnumerable{TElement}"/>. </returns>
        public static IEnumerable<TElement> Distinct<TElement, TEqualityMember>(
            this IEnumerable<TElement> enumerable, Func<TElement, TEqualityMember> selector)
        {
            return enumerable.Distinct(new MemberComparer<TElement, TEqualityMember>(selector));
        }
    }
}
