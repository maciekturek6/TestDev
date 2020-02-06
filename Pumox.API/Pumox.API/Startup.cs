using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pumox.Domain;
using Pumox.Services;
using Pumox.Services.Mapping;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;

namespace Pumox.API
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
            services.AddMvc()
                 .AddJsonOptions(options =>
                 {
                     options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                 });
            //.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var connectionString = Configuration.GetConnectionString("PumoxDbContext");
            services.AddEntityFrameworkNpgsql().AddDbContext<PumoxDbContext>(options => options.
                UseNpgsql(connectionString));

            services.AddDbContext<PumoxDbContext>(options => options.UseNpgsql(connectionString)).AddUnitOfWork<PumoxDbContext>();
            services.AddAutoMapper(typeof(CompanyMapping).GetTypeInfo().Assembly);

            services
                .AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasicAuthentication(
                    options =>
                    {
                        options.Realm = "Pumox Application";
                        options.Events = new BasicAuthenticationEvents
                        {
                            OnValidatePrincipal = context =>
                            {
                                if ((context.UserName == Configuration.GetValue<string>("Authentication:UserName")) && (context.Password == Configuration.GetValue<string>("Authentication:Password")))
                                {
                                    var claims = new List<Claim>
                                    {
                                        new Claim(ClaimTypes.Name, context.UserName, context.Options.ClaimsIssuer)
                                    };

                                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                                    context.Principal = principal;
                        }
                        else
                        {
                            // optional with following default.
                             context.AuthenticationFailMessage = "Authentication failed."; 
                        }

                        return Task.CompletedTask;
                    }
                };
            });
            services.AddScoped(typeof(ICompanyService), typeof(CompanyServices));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
