using CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create.MessageValidators;
using System.Text;

namespace CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create;

public class DeliverierCreateValidator
{
    public readonly DeliverierBuildMessageDeliverierCreate _buildMessageCreate;
    public readonly DeliverierBuildMessageCnh _buildMessageCnh;
    public readonly DeliverierBuildMessageCnpj _buildMessageCnpj;

    public DeliverierCreateValidator(
        DeliverierBuildMessageDeliverierCreate buildMessageCreate,
        DeliverierBuildMessageCnh buildMessageCnh,
        DeliverierBuildMessageCnpj buildMessageCnpj)
    {
        _buildMessageCreate = buildMessageCreate;
        _buildMessageCnh = buildMessageCnh;
        _buildMessageCnpj = buildMessageCnpj;
    }

    public async Task<StringBuilder> Validate(DeliverierCreateCommand data)
    {
        var message = new StringBuilder();

        await _buildMessageCnpj.Build(message, data.Cnpj);

        DeliverierBuildMessageFullName.Build(data, message);

        DeliverierBuildMessageBirthDate.Build(data, message);
        DeliverierBuildMessageLicenseDates.Build(data, message);

        DeliverierBuildMessageLicenseType.Build(data, message);

        await _buildMessageCreate.Build(data, message);

        await _buildMessageCnh.Build(data, message);

        return message;
    }
}
