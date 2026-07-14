namespace Warehouse.Domain.Exceptions;

public class BusinessRuleException : Exception
{
    public string ErrorCode { get; }

    public BusinessRuleException(
        string message,
        string errorCode = "business_rule_violation")
        : base(message)
    {
        ErrorCode = errorCode;
    }
}
