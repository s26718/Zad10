namespace Zad10.Exceptions;

[Serializable]
public class TooManyMedicamentsException : Exception
{
    public TooManyMedicamentsException ()
    {}

    public TooManyMedicamentsException (string message) 
        : base(message)
    {}

    public TooManyMedicamentsException (string message, Exception innerException)
        : base (message, innerException)
    {}  
}