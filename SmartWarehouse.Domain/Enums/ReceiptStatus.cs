namespace SmartWarehouse.Domain.Enums
{
    /// <summary>Trạng thái phiếu nhập kho</summary>
    public enum ReceiptStatus
    {
        Draft     = 0,   // Bản nháp
        Confirmed = 1,   // Đã xác nhận
        Cancelled = 2    // Đã huỷ
    }
}
