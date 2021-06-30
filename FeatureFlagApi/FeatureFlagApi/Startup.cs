﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlagApi.Services;
using FeatureFlagApi.SwaggerUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace FeatureFlagApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            ConfigureSwaggerDocument(services);
            ConfigureDI(services);
        }

        public void ConfigureDI(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IRulesEngineService, RulesEngineService>();
            services.AddSingleton<IFeatureRepository, YamlFileFeatureService>();
            services.AddSingleton<IRequestHeaderService, RequestHeaderService>();
            services.AddScoped<IHttpRequestHeaderMatchInListRuleService, HttpRequestHeaderMatchInListRuleService>();
            services.AddScoped<IJwtPayloadParseMatchInListRuleService, JwtPayloadParseMatchInListRuleService>();
            
        }

        public void ConfigureSwaggerDocument(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FeatureFlag API",
                    Version = "v1",
                    Description = "Simple Development Level FeatureFlag API."
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Use bearer token to authorize",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.OperationFilter<AuthorizationHeader>();
                c.OperationFilter<EnvironmentHeader>();
                var filePath = Path.Combine(AppContext.BaseDirectory, "FeatureFlagApi.xml"); 
                c.IncludeXmlComments(filePath);
            });
        }
            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Local V1");
                c.SwaggerEndpoint("/Prod/swagger/v1/swagger.json", "Prod API Gateway V1");
                c.SwaggerEndpoint("/Stage/swagger/v1/swagger.json", "Stage API Gateway V1");

                // To serve SwaggerUI at application's root page, set the RoutePrefix property to an empty string.
                c.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
                });
            });
        }
    }
}