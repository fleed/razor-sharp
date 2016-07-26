// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateProcessor.cs" company="RazorSharp Team">
//   Copyright © 2016 RazorSharp Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the TemplateProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RazorSharp.Core
{
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Internal;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.ObjectPool;
    using Microsoft.Extensions.PlatformAbstractions;

    using Serilog;

    /// <summary>
    /// Defines the component that can generates content out of a template and a model.
    /// </summary>
    internal class TemplateProcessor
    {
        private readonly TemplateProcessorOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateProcessor"/> class.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        public TemplateProcessor(TemplateProcessorOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// Generates the content.
        /// </summary>
        /// <param name="templateName">
        /// The name of the template.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <typeparam name="TModel">
        /// The type of the model.
        /// </typeparam>
        /// <returns>
        /// The generated content.
        /// </returns>
        public async Task<string> ProcessAsync<TModel>(string templateName, TModel model)
        {
            var services = new ServiceCollection();
            this.ConfigureDefaultServicesAsync(services);
            var provider = services.BuildServiceProvider();

            var renderer = provider.GetRequiredService<RazorViewToStringRenderer>();
            return await renderer.RenderViewToStringAsync(templateName, model);
        }

        private void ConfigureDefaultServicesAsync(IServiceCollection services)
        {
            var applicationEnvironment = PlatformServices.Default.Application;
            services.AddSingleton(applicationEnvironment);

            var appDirectory = this.GetTemplatesDirectory();

            var environment = new HostingEnvironment
            {
                WebRootFileProvider = new PhysicalFileProvider(appDirectory),
                ApplicationName = this.options.ApplicationName
            };
            services.AddSingleton<IHostingEnvironment>(environment);

            services.Configure<RazorViewEngineOptions>(
                razorViewEngineOptions =>
                    {
                        if (this.options.ModelAssemblies != null)
                        {
                            foreach (var modelAssembly in this.options.ModelAssemblies)
                            {
                                var copy = new MemoryStream();
                                modelAssembly.CopyTo(copy);
                                copy.Seek(0, SeekOrigin.Begin);
                                modelAssembly.Seek(0, SeekOrigin.Begin);
                                razorViewEngineOptions.AdditionalCompilationReferences.Add(
                                    MetadataReference.CreateFromStream(copy));
                            }
                        }

                        if (this.options.AdditionalMetadataReferences != null)
                        {
                            foreach (var additionalReference in this.options.AdditionalMetadataReferences)
                            {
                                Log.Debug("Additional reference: {additionalReference}", additionalReference.Display);
                                razorViewEngineOptions.AdditionalCompilationReferences.Add(additionalReference);
                            }
                        }

                        razorViewEngineOptions.FileProviders.Clear();
                        razorViewEngineOptions.FileProviders.Add(new PhysicalFileProvider(appDirectory));
                        razorViewEngineOptions.ViewLocationExpanders.Add(new TemplatesLocationExpander());
                    });

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);

            services.AddLogging();
            services.AddMvc();
            services.AddSingleton<RazorViewToStringRenderer>();
        }

        private string GetTemplatesDirectory()
        {
            if (string.IsNullOrEmpty(this.options.TemplatesFolder))
            {
                return Path.Combine(Directory.GetCurrentDirectory());
            }

            return this.options.TemplatesFolder;
        }
    }
}