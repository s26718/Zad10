namespace Zad10.Exceptions;

[Serializable]
public class NoSuchPatientException : Exception
{
    public NoSuchPatientException ()
    {}

    public NoSuchPatientException (string message) 
        : base(message)
    {}

    public NoSuchPatientException (string message, Exception innerException)
        : base (message, innerException)
    {}  
}