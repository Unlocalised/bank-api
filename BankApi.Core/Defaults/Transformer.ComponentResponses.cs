using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

class TransformerComponentResponses() : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.Responses.Add("500", new OpenApiResponse
        {
            Description = "Internal server error.",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                { "InternalServerError", new OpenApiMediaType { Schema = new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "GenericString" } } } }
            }
        });

        document.Components.Responses.Add("401", new OpenApiResponse
        {
            Description = "Unauthorized request.",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                { "UnauthorizedRequest", new OpenApiMediaType { Schema = new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "GenericString" } } } }
            },
            Headers = new Dictionary<string, OpenApiHeader>
            {
                { "WWW-Authenticate", new OpenApiHeader { Reference = new OpenApiReference { Type = ReferenceType.Header, Id = "GenericStringHeader" } } }
            }
        });

        document.Components.Responses.Add("429", new OpenApiResponse
        {
            Description = "Too many requests.",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                { "TooManyRequests", new OpenApiMediaType { Schema = new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "GenericString" } } } }
            },
            Headers = new Dictionary<string, OpenApiHeader>
            {
                { "Retry-After", OpenApiFactory.CreateGenericIntHeader("The number of seconds to wait before retrying the request.") }
            }
        });

        AddHeadersToResponses(document.Components);

        return Task.CompletedTask;
    }

    private void AddHeadersToResponses(OpenApiComponents components)
    {
        foreach (var response in components.Responses)
        {
            response.Value.Headers.Add("Access-Control-Allow-Origin", new OpenApiHeader { Reference = new OpenApiReference { Type = ReferenceType.Header, Id = "Access-Control-Allow-Origin" } });
            response.Value.Headers.Add("Access-Control-Expose-Headers", new OpenApiHeader { Reference = new OpenApiReference { Type = ReferenceType.Header, Id = "GenericStringHeader" } });

            if (response.Key[0] is '2' or '4')
            {
                response.Value.Headers.Add("X-RateLimit-Limit", new OpenApiHeader { Reference = new OpenApiReference { Type = ReferenceType.Header, Id = "X-RateLimit-Limit" } });
            }
        }
    }
}