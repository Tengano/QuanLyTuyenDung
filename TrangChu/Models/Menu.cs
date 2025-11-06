using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrangChu.Models
{
    [Table("Menu")]
    public class Menu
    {
        [Key]
        public int ID { get; set; }
        
        public int? ParentID { get; set; }
        
        public string? Name { get; set; }
        
        public string? Url { get; set; }
        
        public int? OrderIndex { get; set; }
        
        // Navigation properties for hierarchical relationships
        [ForeignKey("ParentID")]
        public Menu? Parent { get; set; }
        
        public ICollection<Menu>? Children { get; set; }
    }
}