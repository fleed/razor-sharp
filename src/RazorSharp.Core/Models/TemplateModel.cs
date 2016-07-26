// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateModel.cs" company="RazorSharp Team">
//   Copyright © 2016 RazorSharp Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the TemplateModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RazorSharp.Core.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// The template model.
    /// </summary>
    public class TemplateModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the model.
        /// </summary>
        [JsonProperty("value")]
        public dynamic Value { get; set; }
    }
}