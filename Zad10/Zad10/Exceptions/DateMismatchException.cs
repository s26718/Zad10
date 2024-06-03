namespace Zad10.Exceptions;

[Serializable]
public class DateMismatchException : Exception
{
    public DateMismatchException ()
    {}

    public DateMismatchException (string message) 
        : base(message)
    {}

    public DateMismatchException (string message, Exception innerException)
        : base (message, innerException)
    {}  
}