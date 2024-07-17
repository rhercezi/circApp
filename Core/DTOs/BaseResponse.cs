namespace Core.DTOs
{
    public class BaseResponse
    {
        public int ResponseCode { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}