using PersonService.Api.Builders;
using PersonService.Api.Mappers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.OpenApi;

namespace PersonService.Api
{
    public static class DependencyInjection
    {
        public static void AddApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("DevPolicy", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            // AutoMapper
            services.AddAutoMapper(c =>
            {
                c.AddProfile(typeof(PersonProfile));
            });

            // LinkBuiler
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<ILinkBuilder, LinkBuilder>();

            // Swagger
            ConfigureSwagger(services, configuration);
        }

        private static void ConfigureSwagger(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Person Service API",
                    Version = "v1",
                    Description = "Enterprise - grade REST API for managing person data, exposing CRUD operations designed with Clean Architecture principles, MongoDB persistence, and asynchronous messaging via Google Cloud Pub / Sub",
                    Contact = new OpenApiContact
                    {
                        Name = "Moisés do Espírito Santo Silva",
                        Email = "moisessilvagsp@outlook.com",
                        Url = TryCreateUri(configuration["ContactUrl"])
                    }
                });

                // This groups controllers using EndpointGroupName
                options.TagActionsBy(api =>
                {
                    var groupName = api.GroupName;
                    return [groupName ?? api.ActionDescriptor.RouteValues["controller"]];
                });
                options.DocInclusionPredicate((name, api) => true);
            });
        }

        private static Uri? TryCreateUri(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            return Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri : null;
        }
    }
}
