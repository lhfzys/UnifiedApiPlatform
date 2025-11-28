namespace UnifiedApiPlatform.Domain.Exceptions;

/// <summary>
/// 领域异常基类
/// </summary>
public class DomainException : Exception
{
    public DomainException() : base()
    {
    }

    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
