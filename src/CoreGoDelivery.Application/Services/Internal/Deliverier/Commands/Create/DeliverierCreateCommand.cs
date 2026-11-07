using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create;

public class DeliverierCreateCommand : IRequest<ActionResult>
{
    [JsonIgnore]
    public string Id { get; set; } = new Guid().ToString();

    [DefaultValue("João da Silva")]
    public string FullName { get; set; } = string.Empty;

    [DefaultValue("12345678901234")]
    public string Cnpj { get; set; } = string.Empty;

    [DefaultValue("1990-01-01T00:00:00Z")]
    public DateTime BirthDate { get; set; }

    [DefaultValue("12345678900")]
    public string LicenseNumber { get; set; } = string.Empty;

    [DefaultValue("A")]
    public string LicenseType { get; set; } = string.Empty;

    [DefaultValue("1990-01-01T00:00:00Z")]
    public DateTime ExpiryDate { get; set; }

    [DefaultValue("1990-01-01T00:00:00Z")]
    public DateTime IssueDate { get; set; }
}
