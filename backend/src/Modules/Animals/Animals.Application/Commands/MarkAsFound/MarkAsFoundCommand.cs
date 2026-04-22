using MediatR;

namespace Animals.Application.Commands.MarkAsFound;

public sealed record MarkAsFoundCommand(
    string AnimalPostId,
    string RequesterUserId) : IRequest;
