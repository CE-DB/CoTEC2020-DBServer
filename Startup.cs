using CoTEC_Server.DBModels;
using CoTEC_Server.Logic;
using CoTEC_Server.Logic.Auth;
using CoTEC_Server.Logic.GraphQL;
using DinkToPdf;
using DinkToPdf.Contracts;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;

namespace CoTEC_Server
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
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddCors();
            services.AddControllers();


            // Add DbContext
            services
              .AddDbContext<CoTEC_DBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SQLServerConnection")));

            services.AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", config => 
                {
                    var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Constants.key));

                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Constants.Issuer,
                        ValidateAudience = false,
                        IssuerSigningKey = key
                        

                    };

                    //config.SaveToken = true;
                });

            services.AddAuthorization(x =>
            {
                x.AddPolicy(Constants.AdminPolicyName, builder =>
                    builder
                        .RequireAuthenticatedUser()
                        .Requirements.Add(new IdentificationRoleClaim(Constants.AdminRoleName))
                );

                x.AddPolicy(Constants.HealthCenterPolicyName, builder =>
                    builder
                        .RequireAuthenticatedUser()
                        .Requirements.Add(new IdentificationRoleClaim(Constants.HealthCenterRoleName))
                );
            });

            services.AddScoped<IAuthorizationHandler, IdentificationRoleClaimHandle>();


            services
                .AddDataLoaderRegistry()
                .AddGraphQL(SchemaBuilder
                    .New()
                    .BindClrType<DateTime, DateType>()
                    // Here, we add the LocationQueryType as a QueryType
                    .AddQueryType<Query>()
                    .AddMutationType<Mutation>()
                    //.AddMutationType<Mutation>()
                    .AddAuthorizeDirectiveType()
                    .Create());

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var architectureFolder = (IntPtr.Size == 8) ? "64 bit" : "32 bit";
            var wkHtmlToPdfPath = System.IO.Path.Combine(env.ContentRootPath, $"wkhtmltox\\v0.12.4\\{architectureFolder}\\libwkhtmltox");
            CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(wkHtmlToPdfPath);

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseWebSockets();
            app.UseGraphQLHttpPost(new HttpPostMiddlewareOptions { Path = "/graphql" });
            app.UseGraphQLHttpGetSchema(new HttpGetSchemaMiddlewareOptions { Path = "/graphql/schema" });
            app.UseGraphQL();
            app.UsePlayground();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
