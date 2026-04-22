using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence.Documents;
using MongoDB.Driver;

namespace Identity.Infrastructure.Persistence;

internal sealed class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _collection;

    public MongoUserRepository(IMongoCollection<UserDocument> collection)
    {
        _collection = collection;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var document = await _collection
            .Find(x => x.Email == normalizedEmail)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.ToDomain();
    }

    public async Task SaveAsync(User user, CancellationToken cancellationToken = default)
    {
        var document = UserDocument.FromDomain(user);

        await _collection.ReplaceOneAsync(
            x => x.Id == document.Id,
            document,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }
}
