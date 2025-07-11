namespace CSX.Common.Data.Exceptions;

public class BusinessRuleViolationException : System.Exception
{
    public BusinessRuleViolationException() : base()
    {
    }

    public BusinessRuleViolationException(string message) : base(message)
    {
    }
}