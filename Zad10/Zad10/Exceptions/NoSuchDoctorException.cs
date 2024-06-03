namespace Zad10.Exceptions;

[Serializable]
public class NoSuchDoctorException : Exception
{
    public NoSuchDoctorException ()
    {}

    public NoSuchDoctorException (string message) 
        : base(message)
    {}

    public NoSuchDoctorException (string message, Exception innerException)
        : base (message, innerException)
    {}  
}