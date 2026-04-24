using MediatR;

namespace Animals.Application.Queries.GetAnimalById;

public sealed record GetAnimalByIdQuery(string AnimalPostId) : IRequest<GetAnimalByIdResult>;
