using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace SGReservation.Helpers
{
    public static class VersioningExtension
    {
        public static void AddVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(
               config =>
               {
                   config.ReportApiVersions = true;
                   config.AssumeDefaultVersionWhenUnspecified = true;
                   config.DefaultApiVersion = new ApiVersion(1, 0);
                   config.ApiVersionReader = new HeaderApiVersionReader("api-version");
               });
            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });
        }
    }
}
