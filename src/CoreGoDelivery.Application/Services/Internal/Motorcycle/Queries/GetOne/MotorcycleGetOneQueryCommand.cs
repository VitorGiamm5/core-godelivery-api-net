using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;
using System.Text.Json.Serialization;

namespace CoreGoDelivery.Application.Services.Internal.Motorcycle.Queries.GetOne;

public class MotorcycleGetOneQueryCommand : IRequest<ActionResult>
{
    public MotorcycleGetOneQueryCommand(string id)
    {
        Id = id;
    }

    [JsonIgnore]
    public string Id { get; set; }
}
