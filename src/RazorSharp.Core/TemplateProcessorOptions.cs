// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateProcessorOptions.cs" company="RazorSharp Team">
//   Copyright © 2016 RazorSharp Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the TemplateProcessorOptions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RazorSharp.Core
{
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Defines the options for the template processor.
    /// </summary>
    public class TemplateProcessorOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateProcessorOptions"/> class.
        /// </summary>
        public TemplateProcessorOptions()
        {
            this.ModelAssemblies = new List<Stream>();
            this.AdditionalMetadataReferences = new List<MetadataReference>();
        }

        /// <summary>
        /// Gets or sets the additional metadata references to be passed to the Razor view engine.
        /// </summary>
        public ICollection<MetadataReference> AdditionalMetadataReferences { get; set; }

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the model assemblies.
        /// </summary>
        /// <remarks>Streams must be sought to Origin.</remarks>
        public ICollection<Stream> ModelAssemblies { get; set; }

        /// <summary>
        /// Gets or sets the folder where the engine will search templates.
        /// </summary>
        public string TemplatesFolder { get; set; }
    }
}