namespace PetRadar.API.Infrastructure.Validation.Animals;

internal sealed class DeleteAnimalPostValidator : IDeleteAnimalPostValidator
{
    public void Validate(string animalPostId)
    {
        if (string.IsNullOrWhiteSpace(animalPostId))
            throw new AnimalPostIdMissingValidationException();
    }
}
