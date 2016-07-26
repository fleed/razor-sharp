// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateItem.cs" company="RazorSharp Team">
//   Copyright © 2016 RazorSharp Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the TemplateItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RazorSharp.Core.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines a template item.
    /// </summary>
    public class TemplateItem
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the output name.
        /// </summary>
        [JsonProperty("outputName")]
        public string OutputName { get; set; }

        /// <summary>
        /// Gets or sets the referenced model.
        /// </summary>
        [JsonProperty("$modelRef")]
        public string ReferencedModel { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <remarks>
        /// This value is ignored if there is a <see cref="ReferencedModel"/> (value not null or empty).
        /// </remarks>
        [JsonProperty("model")]
        public dynamic Model { get; set; }
    }
}