using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace SwaggerClientApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;
            services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "Swagger Sample API B2C " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                        Version = "v1"
                    });

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    In = "header",
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", Enumerable.Empty<string>()}
                });

                c.AddSecurityDefinition("oauth2", new OAuth2Scheme()
                {
                    Description = "Client credentials needed.",
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = Configuration.GetValue<string>("GlobalSettings:AuthorizationUrl")
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"oauth2", new string[] { }}
                });

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var basePath = Environment.GetEnvironmentVariable("ASPNETCORE_APPL_PATH");

            basePath = basePath == null ? "/" : basePath.EndsWith("/") ? basePath : $"{basePath}/";
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.OAuthClientId(Configuration.GetValue<string>("GlobalSettings:SwaggerClientId"));
                c.OAuthAppName("Sample API B2C");
                c.OAuthAdditionalQueryStringParams(new Dictionary<string, string>() {
                    { "resource", Configuration.GetValue<string>("GlobalSettings:MyUrl") },
                    { "p", Configuration.GetValue<string>("GlobalSettings:Policy") },
                    { "scope",Configuration.GetValue<string>("GlobalSettings:Scope") }
                });
                c.SwaggerEndpoint($"{basePath}swagger/v1/swagger.json", "Swagger Client Api V1 ");
            });

            app.UseAuthentication();
            app.UseMvc();

            }
    }
}
