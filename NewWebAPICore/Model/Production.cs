using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace WebAPICore.Model
{
    public class Production
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
