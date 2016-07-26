// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITemplateContainer.cs" company="RazorSharp Team">
//   Copyright © 2016 RazorSharp Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITemplateContainer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RazorSharp.Core.Models
{
    /// <summary>
    /// Defines an item that contain <see cref="TemplateItem"/>s and <see cref="TemplatePath"/>s.
    /// </summary>
    public interface ITemplateContainer
    {
        /// <summary>
        /// Gets the items.
        /// </summary>
        TemplateItem[] Items { get; }

        /// <summary>
        /// Gets the paths.
        /// </summary>
        TemplatePath[] Paths { get; }
    }
}