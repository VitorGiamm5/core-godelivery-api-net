using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create.Common;
using CoreGoDelivery.Domain.Consts;
using CoreGoDelivery.Domain.Entities.GoDelivery.Deliverier;
using CoreGoDelivery.Domain.Entities.GoDelivery.LicenceDriver;

namespace CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create;

public static class DeliverierCreateMappers
{
    public static DeliverierEntity MapCreateToEntity(DeliverierCreateCommand command)
    {
        var result = new DeliverierEntity()
        {
            Id = command.Id,
            FullName = command.FullName,
            Cnpj = command.Cnpj.NormalizeCnpj(),
            BirthDate = command.BirthDate,
            LicenceDriver = new LicenceDriverEntity()
            {
                Id = command.LicenseNumber,
                Type = DeliverierParseLicenseType.Parse(command),
                ImageUrlReference = LicenseImageConst.PENDING_IMAGE_LICENSE,
                ExpiryDate = command.ExpiryDate,
                IssueDate = command.IssueDate
            }
        };

        return result;
    }
}
