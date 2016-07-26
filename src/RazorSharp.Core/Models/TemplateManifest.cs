// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateManifest.cs" company="RazorSharp Team">
//   Copyright © 2016 RazorSharp Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the TemplateManifest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RazorSharp.Core.Models
{
    using Microsoft.CodeAnalysis;

    using Newtonsoft.Json;

    /// <summary>
    /// Defines a template manifest.
    /// </summary>
    public class TemplateManifest : ITemplateContainer
    {
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        [JsonProperty("templatesPath")]
        public string TemplatesPath { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path to projects that the system should build and pass to the template processor.
        /// </summary>
        [JsonProperty("projects")]
        public string[] Projects { get; set; }

        /// <summary>
        /// Gets or sets the models.
        /// </summary>
        [JsonProperty("models")]
        public TemplateModel[] Models { get; set; }

        /// <summary>
        /// Gets or sets the target root.
        /// </summary>
        [JsonProperty("targetRoot")]
        public string TargetRoot { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        [JsonProperty("items")]
        public TemplateItem[] Items { get; set; }

        /// <summary>
        /// Gets or sets the paths.
        /// </summary>
        [JsonProperty("paths")]
        public TemplatePath[] Paths { get; set; }

        /// <summary>
        /// Gets or sets the additional metadata references that the system should pass to the template processor.
        /// </summary>
        [JsonProperty("additionalMetadataReferences")]
        public MetadataReference[] AdditionalMetadataReferences { get; set; }

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        [JsonProperty("applicationName")]
        public string ApplicationName { get; set; }
    }
}