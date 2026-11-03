namespace SmartWarehouse.Domain.Exceptions
{
    /// <summary>Ném ra khi xuất kho vượt quá số lượng tồn</summary>
    public class InsufficientStockException : DomainException
    {
        public string SKU { get; }
        public int Available { get; }
        public int Requested { get; }

        public InsufficientStockException(string sku, int available, int requested)
            : base($"SKU '{sku}': yêu cầu {requested} nhưng chỉ còn {available} trong kho.")
        {
            SKU       = sku;
            Available = available;
            Requested = requested;
        }
    }
}
