using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace csharp_lend_pc.Models
{
    public class PcEntity
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string Id { get; set; }

        [Column("maker")]
        [Required]
        public string Maker { get; set; }

        [Column("os")]
        [Required]
        public string Os { get; set; }


        [Column("memory")]
        [Required]
        public string Memory { get; set; }


        [Column("capacity")]
        [Required]
        public string Capacity { get; set; }


        [Column("storage_location")]
        [Required]
        public string StorageLocation { get; set; }


        [Column("remarks")]
        public string Remarks { get; set; }

        [Column("has_graphic_board")]
        [Required]
        public bool HasGraphicBoard { get; set; }


        [Column("is_broken")]
        [Required]
        public bool IsBroken { get; set; }

        [Column("is_deleted")]
        [Required]
        public bool IsDeleted { get; set; }


        [Column("lease_start_at")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime LeaseStartAt { get; set; }


        [Column("lease_end_at")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime LeaseEndAt { get; set; }
        
        
        [Column("inventory_at")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? InventoryAt { get; set; }


        [Column("created_at")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; set; }


        [Column("updated_at")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedAt { get; set; }
    }
}