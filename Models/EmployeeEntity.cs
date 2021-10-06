using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace csharp_lend_pc.Models
{
    public class EmployeeEntity
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "資産番号は必須です。")]
        public string Id { get; set; }

        [Column("first_name")]
        [Required(ErrorMessage = "名は必須です。")]
        public string FirstName { get; set; }

        [Column("first_name_kana")]
        [Required(ErrorMessage = "メイは必須です。")]
        public string FirstNameKana { get; set; }


        [Column("last_name")]
        [Required(ErrorMessage = "姓は必須です。")]
        public string LastName { get; set; }


        [Column("last_name_kana")]
        [Required(ErrorMessage = "セイは必須です。")]
        public string LastNameKana { get; set; }


        [Column("department")]
        [Required(ErrorMessage = "所属は必須です。")]
        public string Department { get; set; }


        [Column("tel_number")]
        [Required]
        public string TelNumber { get; set; }


        [Column("email")]
        [Required]
        public string Email { get; set; }


        [Column("age")]
        [Required(ErrorMessage = "{0}は必須です。")]
        public int Age { get; set; }


        [Column("gender")]
        [Required]
        public string Gender { get; set; }


        [Column("position")]
        [Required]
        public string Position { get; set; }


        [Column("privilege")]
        [Required]
        public string Privilege { get; set; }


        [Column("retired_at")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? RetiredAt { get; set; }


        [Column("created_at")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; set; }


        [Column("updated_at")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedAt { get; set; }


        [Column("is_deleted")]
        public bool IsDeleted { get; set; }
    }
}