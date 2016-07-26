// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RazorViewToStringRenderer.cs" company="Swan Team">
//   Copyright © 2016 Swan Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the RazorViewToStringRenderer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RazorSharp.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Routing;
    using Serilog;

    /// <summary>
    /// Renders a razor view to a string.
    /// </summary>
    internal class RazorViewToStringRenderer
    {
        private readonly IRazorViewEngine viewEngine;

        private readonly ITempDataProvider tempDataProvider;

        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorViewToStringRenderer"/> class.
        /// </summary>
        /// <param name="viewEngine">
        /// The view engine.
        /// </param>
        /// <param name="tempDataProvider">
        /// The temp data provider.
        /// </param>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        public RazorViewToStringRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            this.viewEngine = viewEngine;
            this.tempDataProvider = tempDataProvider;
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Renders a view to a string.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>
        /// The rendered content.
        /// </returns>
        /// <exception cref="InvalidOperationException">The view with the given name wasn't found.</exception>
        public async Task<string> RenderViewToStringAsync<TModel>(string name, TModel model)
        {
            var actionContext = this.GetActionContext();

            ViewEngineResult viewEngineResult;
            try
            {
                viewEngineResult = this.viewEngine.FindView(actionContext, name, false);
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error by view engine");
                throw;
            }

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", name));
            }
            
            Log.Information("View {name} found. Model type: {type}", name, model?.GetType().FullName);
            var view = viewEngineResult.View;

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        this.tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = this.serviceProvider
            };

            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}