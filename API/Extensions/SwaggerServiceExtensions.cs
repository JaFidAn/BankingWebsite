using Microsoft.OpenApi.Models;

namespace API.Extensions;

public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Banking Website API",
                Version = "v1",
                Description = "Endpoints for banking website"
            });

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter 'Bearer {your token}'",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            };

            c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    jwtSecurityScheme,
                    new string[] { }
                }
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking Website API v1");
        });

        return app;
    }
}
