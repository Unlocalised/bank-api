using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

class TransformerDocInfo() : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        foreach (var server in GlobalConfiguration.ApiDocument!.Servers)
        {
            server.Extensions["x-internal"] = new OpenApiBoolean(false);
        }

        document.Info = GlobalConfiguration.ApiDocument!.Info;
        document.Servers = GlobalConfiguration.ApiDocument!.Servers;
        document.Components ??= new OpenApiComponents();

        document.Components.Headers.Add("Access-Control-Allow-Origin", CreateStringHeader());
        document.Components.Headers.Add("Access-Control-Expose-Headers", CreateStringHeader());
        document.Components.Headers.Add("X-RateLimit-Limit", CreateIntHeader($"The maximum number of requests you're permitted to make in a window of {GlobalConfiguration.ApiSettings!.FixedWindowRateLimit.Window.Minutes} minutes."));

        document.Components.Responses.Add("500", new OpenApiResponse
        {
            Description = "Internal server error.",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                { "InternalServerError", new OpenApiMediaType { Schema = CreateStringSchema() } }
            }
        });

        document.Components.Responses.Add("401", new OpenApiResponse
        {
            Description = "Unauthorized request.",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                { "UnauthorizedRequest", new OpenApiMediaType { Schema = CreateStringSchema() } }
            },
            Headers = new Dictionary<string, OpenApiHeader>
            {
                { "WWW-Authenticate", CreateStringHeader() }
            }
        });

        document.Components.Responses.Add("429", new OpenApiResponse
        {
            Description = "Too many requests.",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                { "TooManyRequests", new OpenApiMediaType { Schema = CreateStringSchema() } }
            },
            Headers = new Dictionary<string, OpenApiHeader>
            {
                { "Retry-After", CreateIntHeader("The number of seconds to wait before retrying the request.") }
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
            response.Value.Headers.Add("Access-Control-Expose-Headers", new OpenApiHeader { Reference = new OpenApiReference { Type = ReferenceType.Header, Id = "Access-Control-Expose-Headers" } });

            if (response.Key[0] is '2' or '4')
            {
                response.Value.Headers.Add("X-RateLimit-Limit", new OpenApiHeader { Reference = new OpenApiReference { Type = ReferenceType.Header, Id = "X-RateLimit-Limit" } });
            }
        }
    }

    private OpenApiSchema CreateStringSchema() => new OpenApiSchema
    {
        Type = "string",
        Pattern = GlobalConfiguration.ApiSettings!.GenericBoundaries.Regex,
        MaxLength = GlobalConfiguration.ApiSettings!.GenericBoundaries.Maximum
    };

    private OpenApiHeader CreateStringHeader() => new OpenApiHeader
    {
        Schema = CreateStringSchema()
    };

    private OpenApiHeader CreateIntHeader(string? description) => new OpenApiHeader
    {
        Schema = new OpenApiSchema
        {
            Type = "integer",
            Format = "int32",
            Minimum = GlobalConfiguration.ApiSettings!.GenericBoundaries.Minimum,
            Maximum = GlobalConfiguration.ApiSettings!.GenericBoundaries.Maximum,
            Description = description
        }
    };
}