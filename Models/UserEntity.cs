using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace csharp_lend_pc.Models
{
    public class UserEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [Required]
        public int Id { get; set; }

        [Column("user_name")]
        [Required]
        public string UserName { get; set; }

        [Column("hashed_password")]
        [Required]
        public string PasswordHash { get; set; }

        [Column("is_deleted")]
        [Required]
        public bool IsDeleted { get; set; }


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