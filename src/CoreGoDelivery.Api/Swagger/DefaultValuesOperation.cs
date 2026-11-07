using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Nodes;

namespace CoreGoDelivery.Api.Swagger;

public sealed class DefaultValuesOperation : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters is null || context.MethodInfo?.DeclaringType is not { } declaringType)
        {
            return;
        }

        foreach (var parameter in operation.Parameters)
        {
            if (string.IsNullOrWhiteSpace(parameter.Name))
            {
                continue;
            }

            var property = declaringType.GetProperty(
                parameter.Name,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            var defaultValue = property?.GetCustomAttribute<DefaultValueAttribute>()?.Value;
            if (defaultValue is not null && parameter.Schema is OpenApiSchema schema)
            {
                schema.Default = JsonValue.Create(defaultValue.ToString());
            }
        }
    }
}
