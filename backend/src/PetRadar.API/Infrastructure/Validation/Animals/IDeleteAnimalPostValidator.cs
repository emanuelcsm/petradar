namespace PetRadar.API.Infrastructure.Validation.Animals;

public interface IDeleteAnimalPostValidator
{
    void Validate(string animalPostId);
}
