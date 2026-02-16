namespace TestApi.Common.Exceptions;

/// <summary>Thrown when a requested resource does not exist.</summary>
public class NotFoundException(string message) : Exception(message)
{
    public static NotFoundException For<T>(int id) =>
        new($"{typeof(T).Name} with id {id} was not found.");
}
