using PersonService.Api.Builders;
using PersonService.Api.Mappers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.OpenApi;

namespace PersonService.Api
{
    public static class Bootstrapper
    {
        public static void AddApi(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddCors(options =>
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
            service.AddAutoMapper(c =>
            {
                c.AddProfile(typeof(PersonProfile));
            });

            // LinkBuiler
            service.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            service.AddScoped<ILinkBuilder, LinkBuilder>();

            // Swagger
            ConfigureSwagger(service, configuration);
        }

        private static void ConfigureSwagger(IServiceCollection service, IConfiguration configuration)
        {
            service.AddSwaggerGen(options =>
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
