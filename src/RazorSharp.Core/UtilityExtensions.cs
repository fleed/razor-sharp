// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilityExtensions.cs" company="RazorSharp Team">
//   Copyright © 2016 RazorSharp Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace RazorSharp.Core
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Utility extension methods.
    /// </summary>
    public static class UtilityExtensions
    {
        /// <summary>
        /// The enumerate.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <returns>
        /// An array corresponding to the given enumerable, if the given values enumerable is not <c>null</c>;
        /// otherwise, <c>null</c>.
        /// </returns>
        public static T[] SafelyEnumerate<T>(this IEnumerable<T> values)
        {
            if (values == null)
            {
                return null;
            }

            if (values is T[])
            {
                return values as T[];
            }

            return values.ToArray();
        }
    }
}