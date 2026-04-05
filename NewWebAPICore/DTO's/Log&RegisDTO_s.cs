namespace NewWebAPICore.DTO_s
{
    public class Log_RegisDTO_s<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        public Log_RegisDTO_s(bool success, string message, T? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}
