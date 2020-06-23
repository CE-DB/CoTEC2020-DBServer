using CoTEC_Server.Database;
using CoTEC_Server.Logic;
using CoTEC_Server.Logic.Auth;
using CoTEC_Server.Logic.GraphQL;
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
            services.AddControllers();


            // Add DbContext
            services
              .AddDbContext<SQLServerContext>(options =>
                options.UseSqlServer(SQLServerContext.DbConnectionString));

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
                    .AddQueryType<QueryType>()
                    .AddMutationType<Mutation>()
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
