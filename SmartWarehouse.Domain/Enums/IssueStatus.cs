namespace SmartWarehouse.Domain.Enums
{
    /// <summary>Trạng thái phiếu xuất kho</summary>
    public enum IssueStatus
    {
        Draft      = 0,  // Bản nháp
        Approved   = 1,  // Đã duyệt
        Dispatched = 2   // Đã xuất hàng
    }
}
