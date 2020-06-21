using CoTEC_Server.Database;
using CoTEC_Server.Logic.GraphQL;
using CoTEC_Server.Logic.GraphQL.Types;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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

            services
                .AddDataLoaderRegistry()
                .AddGraphQL(SchemaBuilder
                    .New()
                    .BindClrType<DateTime, DateType>()
                    // Here, we add the LocationQueryType as a QueryType
                    .AddQueryType<QueryType>()
                    .AddMutationType<Mutation>()
                    .Create());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();
            app.UseGraphQLHttpPost(new HttpPostMiddlewareOptions { Path = "/graphql" });
            app.UseGraphQLHttpGetSchema(new HttpGetSchemaMiddlewareOptions { Path = "/graphql/schema" });
            app.UseGraphQL();
            app.UsePlayground();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
