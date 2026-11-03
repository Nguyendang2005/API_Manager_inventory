namespace SmartWarehouse.Domain.Exceptions
{
    /// <summary>Ném ra khi SKU sản phẩm đã tồn tại</summary>
    public class DuplicateSkuException : DomainException
    {
        public string SKU { get; }

        public DuplicateSkuException(string sku)
            : base($"SKU '{sku}' đã tồn tại trong hệ thống.")
        {
            SKU = sku;
        }
    }
}
