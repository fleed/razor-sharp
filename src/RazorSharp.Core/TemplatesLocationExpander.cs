// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplatesLocationExpander.cs" company="RazorSharp Team">
//   Copyright © 2016 RazorSharp Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the TemplatesLocationExpander type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RazorSharp.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc.Razor;

    /// <summary>
    /// Custom <see cref="IViewLocationExpander"/> used to define the view locations for templates.
    /// </summary>
    public class TemplatesLocationExpander : IViewLocationExpander
    {
        /// <inheritdoc />
        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            return viewLocations.Select(f => f.Replace("/Views/", "/Templates/"));
        }

        /// <inheritdoc />
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            // nothing to do here
        }
    }
}