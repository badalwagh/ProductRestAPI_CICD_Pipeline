using System.ComponentModel.DataAnnotations;

namespace NewWebAPICore.Model
{
    public class ErrorLogs
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
