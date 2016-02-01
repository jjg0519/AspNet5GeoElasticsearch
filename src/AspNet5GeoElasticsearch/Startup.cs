﻿using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AspNet5GeoElasticsearch.ElasticsearchApi;
using Swashbuckle.SwaggerGen;

namespace AspNet5GeoElasticsearch
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var pathToDoc = Configuration["Swagger:Path"];
            // Add framework services.
            services.AddMvc();
            services.AddSwaggerGen();
            services.ConfigureSwaggerDocument(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Geo Search API",
                    Description = "A simple api to search using geo location in Elasticsearch",
                    TermsOfService = "None"
                });
                options.OperationFilter(new Swashbuckle.SwaggerGen.XmlComments.ApplyXmlActionComments(pathToDoc));
            });
            services.ConfigureSwaggerSchema(options =>
            {
                options.DescribeAllEnumsAsStrings = true;
                options.ModelFilter(new Swashbuckle.SwaggerGen.XmlComments.ApplyXmlTypeComments(pathToDoc));
            });

            services.AddScoped<ISearchProvider, SearchProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseBrowserLink();
            app.UseDeveloperExceptionPage();

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // This should be defined just after app.UseMVC as suggested by Muqeet Khan 
            // See commnet in blog. Thanks.
            app.UseSwaggerGen();
            app.UseSwaggerUi();
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}

