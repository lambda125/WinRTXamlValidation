// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberComparer.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Core.Helpers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The member comparer class, which implements <see cref="IEqualityComparer{TElement}"/> 
    /// and compares by using a specific member of this class.
    /// </summary>
    /// <typeparam name="TElement"> The type of the element to compare. </typeparam>
    /// <typeparam name="TEqualityMember"> The type of the element member to compare. </typeparam>
    public class MemberComparer<TElement, TEqualityMember> : IEqualityComparer<TElement>
    {
        /// <summary>
        /// The member selector function.
        /// </summary>
        private readonly Func<TElement, TEqualityMember> selector;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberComparer{TElement,TEqualityMember}"/> class.
        /// </summary>
        /// <param name="selector"> The function which selects a member. </param>
        /// <exception cref="ArgumentNullException"> When <paramref name="selector"/> is null. </exception>
        public MemberComparer(Func<TElement, TEqualityMember> selector)
        {
            Guard.AssertNotNull(selector, "selector");

            this.selector = selector;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x"> The first object of type <see cref="TElement"/> to compare. </param>
        /// <param name="y"> The second object of type <see cref="TElement"/> to compare. </param>
        /// <returns> true if the specified objects are equal; otherwise, false. </returns>
        public bool Equals(TElement x, TElement y)
        {
            return EqualityComparer<TEqualityMember>.Default.Equals(this.selector(x), this.selector(y));
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj"> The <see cref="T:System.Object"/> for which a hash code is to be returned. </param>
        /// <returns> A hash code for the specified object. </returns>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null. </exception>
        public int GetHashCode(TElement obj)
        {
            return EqualityComparer<TEqualityMember>.Default.GetHashCode(this.selector(obj));
        }
    }
}