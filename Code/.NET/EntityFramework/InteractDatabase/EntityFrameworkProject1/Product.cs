using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkProject1
{
    // Ánh xạ bảng Products của database vào class Product
    [Table("Products")]
    public class Product
    {
        [Key]
        public int ProductId { set; get; }

        [Required]
        [StringLength(50)]
        public string Name { set; get; }

        [StringLength(50)]
        public string Provider { set; get; }
    }
}
