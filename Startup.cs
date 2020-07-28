using CoTEC_Server.DBModels;
using CoTEC_Server.Logic;
using CoTEC_Server.Logic.Auth;
using CoTEC_Server.Logic.GraphQL;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

            services.AddCors();
            services.AddControllers();


            if (Environment.GetEnvironmentVariable("DigitalOceanDatabaseConn") is null)
            {
                services
              .AddDbContext<CoTEC_DBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SQLServerConnection")));
            }
            else
            {
                services
              .AddDbContext<CoTEC_DBContext>(options =>
                options.UseSqlServer(Environment.GetEnvironmentVariable("DigitalOceanDatabaseConn")));
            }



            if (Environment.GetEnvironmentVariable("DigitalOceanDatabaseConn") is null)
            {
                services
              .AddDbContext<CoTEC_DBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SQLServerConnection")));
            }
            else
            {
                services
              .AddDbContext<CoTEC_DBContext>(options =>
                options.UseSqlServer(Environment.GetEnvironmentVariable("DigitalOceanDatabaseConn")));
            }

            // Add DbContext

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(config => 
                {
                    var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Constants.key));

                    config.RequireHttpsMetadata = false;
                    config.SaveToken = true;

                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Constants.Issuer,
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.Zero
                    };
                    services.AddCors();

                    //config.SaveToken = true;
                });

            services.AddAuthorization(x =>
            {
                x.AddPolicy(Constants.AdminPolicyName, builder =>
                    builder
                        .RequireAuthenticatedUser()
                        .RequireRole(Constants.AdminRoleName)
                        .Build()
                );

                x.AddPolicy(Constants.HealthCenterPolicyName, builder =>
                    builder
                        .RequireAuthenticatedUser()
                        .RequireRole(Constants.HealthCenterRoleName)
                        .Build()
                );
            });

            //services.AddScoped<IAuthorizationHandler, IdentificationRoleClaimHandle>();


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

            app.UseAuthentication();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseWebSockets();
            app.UseGraphQLHttpPost(new HttpPostMiddlewareOptions { Path = "/graphql" });
            //app.UseGraphQLHttpGetSchema(new HttpGetSchemaMiddlewareOptions { Path = "/graphql/schema" });
            app.UseGraphQL();
            app.UsePlayground();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
