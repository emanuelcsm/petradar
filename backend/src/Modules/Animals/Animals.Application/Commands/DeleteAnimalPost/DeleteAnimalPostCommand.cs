using MediatR;

namespace Animals.Application.Commands.DeleteAnimalPost;

public sealed record DeleteAnimalPostCommand(
    string AnimalPostId,
    string RequesterUserId) : IRequest;
