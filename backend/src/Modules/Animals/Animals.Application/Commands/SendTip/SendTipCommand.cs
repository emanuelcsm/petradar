using MediatR;

namespace Animals.Application.Commands.SendTip;

public sealed record SendTipCommand(
    string AnimalId,
    string SenderId,
    string SenderName,
    string Message) : IRequest;
