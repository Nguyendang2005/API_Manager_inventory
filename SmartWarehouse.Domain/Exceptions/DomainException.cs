namespace SmartWarehouse.Domain.Exceptions
{
    /// <summary>Exception gốc cho toàn bộ lỗi domain business logic</summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
        public DomainException(string message, Exception inner) : base(message, inner) { }
    }
}
