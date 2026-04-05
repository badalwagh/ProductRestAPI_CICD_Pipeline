using System.ComponentModel.DataAnnotations;

namespace WebAPICore.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public ICollection<Production> Products { get; set; }
    }
}
