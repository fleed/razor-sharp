// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RazorSharpException.cs" company="RazorSharp Team">
//   Copyright © 2016 RazorSharp Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the RazorSharpException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RazorSharp.Core
{
    using System;

    /// <summary>
    /// Defines an exception thrown by the RazorSharp library.
    /// </summary>
    public class RazorSharpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RazorSharpException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public RazorSharpException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorSharpException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public RazorSharpException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}