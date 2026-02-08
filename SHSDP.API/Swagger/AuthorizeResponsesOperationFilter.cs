using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SHSDP.API.Swagger;

public class AuthorizeResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        bool hasAuthorize = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                                .OfType<AuthorizeAttribute>().Any() == true
                            || context.MethodInfo.GetCustomAttributes(true)
                                .OfType<AuthorizeAttribute>().Any();

        if (hasAuthorize)
        {
            operation.Responses?.TryAdd("401", new OpenApiResponse
            {
                Description = "Unauthorized - You are not authorized to access this resource without a valid token."
            });
        }
    }
}