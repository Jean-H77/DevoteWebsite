using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevoteWebsite.Models.Store
{
    [Table("sales", Schema = "store")]
    public class StoreItemSaleInfo
    {
        [Key]
        [Column("sale_id")]
        public int Id { get; set; }

        [Column("percentage_off")]
        public int PercentageOff { get; set; }

        [ForeignKey("StoreItem")]
        [Column("item_id")]
        public int ItemId { get; set; }

        public StoreItem StoreItem { get; set; } = null!;
    }
}
