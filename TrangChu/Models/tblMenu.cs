using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrangChu.Models
{
    [Table("tblMenu")]
    public class tblMenu
    {
        [Key]
        public int id { get; set; }
        
        public int? parent_id { get; set; }
        
        public string? name { get; set; }
        
        public string? url { get; set; }
        
        public int? order_index { get; set; }
        
        // Navigation properties for hierarchical relationships
        [ForeignKey("parent_id")]
        public tblMenu? Parent { get; set; }
        
        public ICollection<tblMenu>? Children { get; set; }
    }
}