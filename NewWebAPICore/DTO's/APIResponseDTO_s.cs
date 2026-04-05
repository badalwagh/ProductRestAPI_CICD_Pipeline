namespace NewWebAPICore.DTO_s
{
    public class APIResponseDTO_s<T>
    {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T? Data { get; set; }
            public APIResponseDTO_s(bool success, string message, T? data)
            {
                Success = success;
                Message = message;
                Data = data;
            }
    }
}
