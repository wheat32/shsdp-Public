using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SHSDP.API.Swagger;

public class EnumAsStringSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is OpenApiSchema concreteSchema && context.Type.IsEnum)
        {
            concreteSchema.Type = JsonSchemaType.String;
            concreteSchema.Format = null;
            concreteSchema.Enum = Enum.GetNames(context.Type)
                .Select(name => (JsonNode)JsonValue.Create(name)!)
                .ToList();
        }
    }
}