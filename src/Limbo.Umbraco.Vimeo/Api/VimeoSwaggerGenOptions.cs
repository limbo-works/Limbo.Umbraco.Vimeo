using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Limbo.Umbraco.Vimeo.Api;

#pragma warning disable CS1591

public class VimeoSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions> {

    public void Configure(SwaggerGenOptions options) {

        options.SwaggerDoc(VimeoApiConstants.Alias, new OpenApiInfo {
            Title = VimeoApiConstants.Name,
            Version = VimeoApiConstants.Version
        });

        options.OperationFilter<VimeoSecurityFilter>();
        options.SchemaFilter<VimeoSchemaFilter>();
        options.DocumentFilter<VimeoDocumentFilter>();

    }

}