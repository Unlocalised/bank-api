using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

class TransformerComponentHeaders() : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.Headers.Add("GenericStringHeader", new OpenApiHeader
        {
            Schema = new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "GenericString" } }
        });

        //because of a bug in the spectral OWASP linter, we add the Access-Control-Allow-Origin header to the components and use it
        document.Components.Headers.Add("Access-Control-Allow-Origin", new OpenApiHeader
        {
            Schema = new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "GenericString" } }
        });

        document.Components.Headers.Add("X-RateLimit-Limit", OpenApiFactory.CreateGenericIntHeader($"The maximum number of requests you're permitted to make in a window of {GlobalConfiguration.ApiSettings!.FixedWindowRateLimit.Window.Minutes} minutes."));

        return Task.CompletedTask;
    }
}