﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevoteWebsite.Models.Store
{
    [Table("items", Schema = "store")]
    public class StoreItem
    {
        [Key]
        [Column("item_id")]
        public int Id { get; set; }

        [Column("name")]
        public required string Name { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("thumbnail_url")]
        public required string ThumbnailUrl { get; set; }

        [Column("description")]
        public required string Description { get; set; }

        [Column("uid")]
        public Guid Uid { get; set; }

        public StoreItemSaleInfo? StoreItemSaleInfo { get; set; }
    }

}
