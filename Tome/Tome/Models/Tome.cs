using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Tome.Models
{
    public class Tome
    {
        [Key]
        public int TomeId { get; set; }
        [Required(ErrorMessage = "Required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Tome name must have at least 3 characters")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only alphanumeric and space allowed.")]
        public string Name { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public bool IsPrivate { get; set; } = false;
        

        //[MaxLength(128), ForeignKey("ApplicationUser")]
        //public virtual string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }


    }
    





}