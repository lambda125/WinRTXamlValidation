// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="-" year="2013">
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
    /// Extension methods for collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Removes a range of elements from the collection.
        /// </summary>
        /// <typeparam name="T"> The type of the element. </typeparam>
        /// <param name="collection"> The collection to remove the elements from. </param>
        /// <param name="elementsToRemove"> The enumeration of elements to remove from the collection. </param>
        /// <param name="allOccurrences"> The indicator whether all occurrences of the element instance should be removed. </param>
        /// <returns>true if at least one element instance has been removed. Otherwise false.</returns>
        public static bool RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> elementsToRemove, bool allOccurrences = false)
        {
            Guard.AssertNotNull(collection, "collection");
            Guard.AssertNotNull(elementsToRemove, "elementsToRemove");

            var localElementsToRemove = elementsToRemove.ToArray();

            lock (collection)
            {
                var originalCount = collection.Count;
                foreach (var elementToRemove in localElementsToRemove)
                {
                    if (allOccurrences)
                    {
                        while (collection.Remove(elementToRemove)) { }
                    }
                    else
                    {
                        collection.Remove(elementToRemove);
                    }
                }

                return originalCount > collection.Count;
            }
        }
    }
}
