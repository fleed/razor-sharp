// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentGenerator.cs" company="RazorSharp Team">
//   Copyright © 2016 RazorSharp Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the ContentGenerator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RazorSharp.Core
{
    using System.IO;
    using System.Threading.Tasks;

    using Serilog;

    /// <summary>
    /// Defines a component that can generate content.
    /// </summary>
    public class ContentGenerator
    {
        public async Task GenerateAsync(
            TemplateProcessorOptions processorOptions,
            string templateName,
            string targetPath)
        {
            var templateProcessor = new TemplateProcessor(processorOptions);
            var content = await templateProcessor.ProcessAsync<object>(templateName, null);
            await this.WriteFileAsync(targetPath, content);
            Log.Information("Generated file {path}", targetPath);
        }

        public async Task GenerateAsync<T>(
            TemplateProcessorOptions processorOptions,
            string templateName,
            T model,
            string targetPath)
        {
            var templateProcessor = new TemplateProcessor(processorOptions);
            var content = await templateProcessor.ProcessAsync(templateName, model);
            await this.WriteFileAsync(targetPath, content);
            Log.Information("Generated file {path}", targetPath);
        }

        private async Task WriteFileAsync(string path, string content)
        {
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            else if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            using (var file = File.OpenWrite(path))
            {
                using (var streamWriter = new StreamWriter(file))
                {
                    await streamWriter.WriteAsync(content);
                }
            }
        }
    }
}