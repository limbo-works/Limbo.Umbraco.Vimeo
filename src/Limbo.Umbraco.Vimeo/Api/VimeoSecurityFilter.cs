using Umbraco.Cms.Api.Management.OpenApi;

namespace Limbo.Umbraco.Vimeo.Api;

internal class VimeoSecurityFilter : BackOfficeSecurityRequirementsOperationFilterBase {

    protected override string ApiName => VimeoApiConstants.Name;

}