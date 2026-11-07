using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Domain.Enums.ServiceErrorMessage;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using DocumentValidator;
using System.Text;

namespace CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create.MessageValidators;

public class DeliverierBuildMessageCnpj
{
    private readonly IDeliverierRepository _repositoryDeliverier;

    public DeliverierBuildMessageCnpj(IDeliverierRepository repositoryDeliverier)
    {
        _repositoryDeliverier = repositoryDeliverier;
    }

    public async Task Build(StringBuilder message, string cnpj)
    {
        var paramName = nameof(cnpj);

        if (string.IsNullOrWhiteSpace(cnpj))
        {
            message.Append(paramName.AppendError());
        }
        else
        {
            if (!CnpjValidation.Validate(cnpj))
            {
                message.Append(paramName.AppendError(AdditionalMessageEnum.InvalidFormat));
            }
            else if (!await _repositoryDeliverier.CheckIsUnicByCnpj(cnpj.NormalizeCnpj()))
            {
                message.Append(paramName.AppendError(AdditionalMessageEnum.AlreadyExist));
            }
        }
    }
}
