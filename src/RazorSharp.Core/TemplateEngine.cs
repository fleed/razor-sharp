// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateEngine.cs" company="RazorSharp Team">
//   Copyright © 2016 RazorSharp Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the TemplateEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RazorSharp.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis;
    using Microsoft.DotNet.ProjectModel.Workspaces;

    using Models;

    using Newtonsoft.Json;

    using Serilog;

    /// <summary>
    /// Defines the engine that processes a <see cref="TemplateManifest"/> and generates the corresponding output.
    /// </summary>
    public class TemplateEngine
    {
        /// <summary>
        /// Processes a <see cref="TemplateManifest"/> provided as JSON serialized file at the given path producing the
        /// corresponding output.
        /// </summary>
        /// <param name="path">
        /// The path to the JSON serialized manifest.
        /// </param>
        /// <exception cref="RazorSharpException">An error occurred during the execution.</exception>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The path is null, empty or not corresponding to an existing file.
        /// </exception>
        public async Task ProcessAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentOutOfRangeException("path", "Path must be an existing json file");
            }

            var info = new FileInfo(path);
            if (!info.Exists)
            {
                throw new ArgumentOutOfRangeException("path", "Path must be an existing json file");
            }

            try
            {
                var assemblies = await this.BuildAssembliesAsync(path);
                TemplateManifest converted;
                using (var file = File.OpenText(path))
                {
                    converted = JsonConvert.DeserializeObject<TemplateManifest>(
                        await file.ReadToEndAsync(),
                        new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                }

                await this.ProcessAsync(converted, assemblies);
            }
            catch (Exception exception)
            {
                throw new RazorSharpException(
                    "An error occurred during the execution",
                    exception);
            }
        }

        /// <summary>
        /// Processes the given <see cref="TemplateManifest"/> producing the corresponding output.
        /// </summary>
        /// <param name="manifest">
        /// The manifest.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        /// <exception cref="ArgumentNullException">The provided <paramref name="manifest"/> is null.</exception>
        public async Task ProcessAsync(TemplateManifest manifest)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException("manifest");
            }

            var applicationName = manifest.ApplicationName ?? Assembly.GetEntryAssembly().GetName().FullName;

            Log.Verbose(
                "Should render manifest {@manifest} with application name '{name}'",
                manifest,
                applicationName);
            var assemblies = new List<Stream>();
            if (manifest.Projects != null)
            {
                assemblies = (await LoadProjectsAsync(manifest.Projects)).ToList();
            }

            await this.ProcessAsync(manifest, assemblies);
        }

        private static async Task<IEnumerable<Stream>> LoadProjectsAsync(IEnumerable<string> items)
        {
            var binaries = new List<Stream>();
            foreach (var item in items)
            {
                using (var projectWorkspace = new ProjectJsonWorkspace(item))
                {
                    foreach (var project in projectWorkspace.CurrentSolution.Projects)
                    {
                        var compilation = await project.GetCompilationAsync();
                        var memoryStream = new MemoryStream();
                        var result = compilation.Emit(memoryStream);
                        if (result.Success)
                        {
                            Log.Debug("Project successfully compiled");
                        }
                        else
                        {
                            Log.Error("Project not compiled");
                            foreach (var error in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
                            {
                                Log.Error("Error while compiling: {@error}", error.GetMessage());
                            }
                        }

                        memoryStream.Seek(0, SeekOrigin.Begin);
                        try
                        {
                            var loadFromStream = AssemblyLoadContext.Default.LoadFromStream(memoryStream);
                            Log.Verbose("Assembly loaded {assembly}", loadFromStream.FullName);
                            binaries.Add(memoryStream);
                        }
                        catch (FileLoadException exception)
                        {
                            Log.Warning(exception, "Couldn't load the specified stream as assembly");
                        }
                        finally
                        {
                            memoryStream.Seek(0, SeekOrigin.Begin);
                        }
                    }
                }
            }

            return binaries;
        }

        private async Task ProcessAsync(
            TemplateManifest manifest,
            IEnumerable<Stream> assemblies,
            IEnumerable<MetadataReference> additionalMetadataReferences = null)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException("manifest");
            }

            var applicationName = manifest.ApplicationName ?? Assembly.GetEntryAssembly().GetName().FullName;

            Log.Verbose(
                "Should render manifest {@manifest} with application name '{name}'",
                manifest,
                applicationName);
            var assembliesArray = assemblies.SafelyEnumerate();
            var additionalMetadataReferencesArray = additionalMetadataReferences.SafelyEnumerate();
            if (manifest.Items != null)
            {
                foreach (var templateItem in manifest.Items)
                {
                    await
                        this.ProcessAsync(
                            applicationName,
                            manifest,
                            templateItem,
                            assembliesArray,
                            additionalMetadataReferencesArray,
                            manifest.TargetRoot);
                }
            }

            if (manifest.Paths != null)
            {
                foreach (var templatePath in manifest.Paths)
                {
                    await
                        this.ProcessAsync(
                            applicationName,
                            manifest,
                            templatePath,
                            assembliesArray,
                            additionalMetadataReferencesArray,
                            manifest.TargetRoot);
                }
            }
        }

        private async Task<IEnumerable<Stream>> BuildAssembliesAsync(string projectPath)
        {
            var items = new List<string>();
            using (var file = File.OpenText(projectPath))
            {
                dynamic json = JsonConvert.DeserializeObject(await file.ReadToEndAsync());
                if (json["projects"] != null)
                {
                    foreach (var item in json["projects"])
                    {
                        items.Add((string)item);
                    }
                }
            }

            return await LoadProjectsAsync(items);
        }

        private async Task ProcessAsync(
            string applicationName,
            TemplateManifest manifest,
            TemplatePath path,
            IEnumerable<Stream> assemblies,
            IEnumerable<MetadataReference> additionalMetadataReferences,
            string outputDirectory)
        {
            var customPath = Path.Combine(outputDirectory, path.OutputPath);
            var assembliesArray = assemblies.SafelyEnumerate();
            var additionalMetadataReferencesArray = additionalMetadataReferences.SafelyEnumerate();
            if (path.Paths != null)
            {
                foreach (var subPath in path.Paths)
                {
                    await
                        this.ProcessAsync(
                            applicationName,
                            manifest,
                            subPath,
                            assembliesArray,
                            additionalMetadataReferencesArray,
                            customPath);
                }
            }

            if (path.Items != null)
            {
                foreach (var item in path.Items)
                {
                    await
                        this.ProcessAsync(
                            applicationName,
                            manifest,
                            item,
                            assembliesArray,
                            additionalMetadataReferencesArray,
                            customPath);
                }
            }
        }

        private async Task ProcessAsync(
            string applicationName,
            TemplateManifest manifest,
            TemplateItem item,
            IEnumerable<Stream> assemblies,
            IEnumerable<MetadataReference> additionalMetadataReferences,
            string outputDirectory)
        {
            Log.Verbose("Processing manifest {@manifest}, item {@item}", manifest, item);
            var options = new TemplateProcessorOptions
                              {
                                  ApplicationName = applicationName,
                                  TemplatesFolder = manifest.TemplatesPath
                              };
            if (assemblies != null)
            {
                options.ModelAssemblies = assemblies.ToList();
            }

            if (additionalMetadataReferences != null)
            {
                options.AdditionalMetadataReferences = additionalMetadataReferences.ToList();
            }

            var generator = new ContentGenerator();
            var directory = new DirectoryInfo(outputDirectory);
            if (!directory.Exists)
            {
                directory.Create();
            }

            var path = Path.Combine(directory.FullName, item.OutputName);
            if (item.ReferencedModel != null)
            {
                var model = manifest.Models.Single(i => i.Name == item.ReferencedModel).Value;
                Log.Debug("Generating item {item} with referenced model {@model}", item.Name, model);
                await generator.GenerateAsync(options, item.Name, model, path);
                return;
            }

            if (item.Model != null)
            {
                Log.Debug("Generating item {item} with model {@model}", item.Name, item.Model);
                await generator.GenerateAsync(options, item.Name, item.Model, path);
                return;
            }

            Log.Debug("Generating item {item} witout model", item.Name);
            await generator.GenerateAsync(options, item.Name, path);
        }
    }
}