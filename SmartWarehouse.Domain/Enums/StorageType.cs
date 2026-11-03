namespace SmartWarehouse.Domain.Enums
{
    /// <summary>Loại kho lưu trữ</summary>
    public enum StorageType
    {
        Ambient     = 0,  // Kho thường
        ColdStorage = 1,  // Kho lạnh
        Hazardous   = 2   // Kho hàng nguy hiểm
    }
}
