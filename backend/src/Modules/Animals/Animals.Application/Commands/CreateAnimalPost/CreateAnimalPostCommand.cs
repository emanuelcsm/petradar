using MediatR;
using PetRadar.SharedKernel.ValueObjects;

namespace Animals.Application.Commands.CreateAnimalPost;

public sealed record CreateAnimalPostCommand(
    string UserId,
    string Description,
    GeoLocation Location,
    IReadOnlyList<string>? MediaIds) : IRequest;
