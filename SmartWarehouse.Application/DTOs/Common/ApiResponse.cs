namespace SmartWarehouse.Application.DTOs.Common
{
    /// <summary>
    /// Bao bì chuẩn hoá cho mọi API response.
    /// Đảm bảo client luôn nhận được cùng một cấu trúc JSON.
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        // Factory methods — tránh new {} rải rác khắp controller
        public static ApiResponse<T> Ok(T data, string? message = null)
            => new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(string message)
            => new() { Success = false, Message = message };
    }
}
