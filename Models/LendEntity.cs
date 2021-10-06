using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace csharp_lend_pc.Models
{
    public class LendEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [Required]
        public int Id { get; set; }

        [Column("employee_id")]
        [Required]
        public string EmployeeId { get; set; }

        [Column("pc_id")]
        [Required]
        public string PcId { get; set; }

        [Column("remarks")]        
        public string Remarks { get; set; }

        [Column("lend_start_at")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime LendStartAt { get; set; }


        [Column("lend_end_at")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime LendEndAt { get; set; }


        [Column("created_at")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; set; }


        [Column("updated_at")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedAt { get; set; }


        [Column("is_deleted")]
        [Required]
        public bool IsDeleted { get; set; }

        [Column("is_returned")]
        [Required]
        public bool IsReturned { get; set; }
    }
}