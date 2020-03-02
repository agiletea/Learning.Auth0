using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AgileTea.Auth0.Web.Api
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
            services.AddMvc();

            services.AddControllers();
            services.AddLogging();

            // 1. Add Authentication Services
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = "https://agiletea.eu.auth0.com/";
                options.Audience = "http://localhost:8001";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
                        var logger = loggerFactory.CreateLogger("AuthenticationTokenValidationLogger");

                        logger.LogDebug($"Token Issuer: {context.SecurityToken.Issuer}");

                        if (context.SecurityToken is JwtSecurityToken token)
                        {
                            if (context.Principal.Identity is ClaimsIdentity identity)
                            {
                                identity.AddClaim(new Claim("access_token", token.RawData));

                                if (!identity.Claims.Any(x => x.ValueType == "SessionId"))
                                {
                                    identity.AddClaim(new Claim("session_id", Guid.NewGuid().ToString()));
                                }
                            }
                        }

                        context.Success();
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context => Task.CompletedTask
                };
            })
                ;

            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
