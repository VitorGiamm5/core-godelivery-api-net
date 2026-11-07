using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;
using System.Text.Json.Serialization;

namespace CoreGoDelivery.Application.Services.Internal.LicenseDriver.Commands.Create;

public class LicenseImageCommand : IRequest<ActionResult>
{
    public byte[] LicenseImageBase64 { get; set; } = Array.Empty<byte>();

    [JsonIgnore]
    public string? IdLicenseNumber { get; set; }
}
