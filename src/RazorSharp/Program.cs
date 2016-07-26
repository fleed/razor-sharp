// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="RazorSharp Team">
//   Copyright © 2016 RazorSharp Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RazorSharp
{
    using System;
    using System.IO;
    using System.Linq;

    using Core;

    using Microsoft.Extensions.Configuration;

    using Serilog;

    /// <summary>
    /// Defines the application entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry point for the application.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: razor-sharp target_directory");
                return;
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            var engine = new TemplateEngine();
            try
            {
                engine.ProcessAsync(args.First()).GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Exception");
            }
        }
    }
}