namespace Zad10.Exceptions;

[Serializable]
public class NoSuchMedicamentException : Exception
{
    public NoSuchMedicamentException ()
    {}

    public NoSuchMedicamentException (string message) 
        : base(message)
    {}

    public NoSuchMedicamentException (string message, Exception innerException)
        : base (message, innerException)
    {}  
}