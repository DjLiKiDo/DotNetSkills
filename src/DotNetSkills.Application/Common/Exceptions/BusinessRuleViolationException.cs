namespace DotNetSkills.Application.Common.Exceptions;

/// <summary>
/// Thrown when a business rule is violated in the Application layer (distinct from domain invariants).
/// </summary>
public sealed class BusinessRuleViolationException : ApplicationExceptionBase
{
    public BusinessRuleViolationException(string message) : base(message) { }
    public BusinessRuleViolationException(string ruleName, string detail)
        : base($"Business rule '{ruleName}' violated: {detail}") { }

    public override int StatusCode => 409; // Conflict is typical; adjust to 400 if desired
    public override string ErrorCode => "business_rule_violation";
}
