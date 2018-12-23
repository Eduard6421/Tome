using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Tome.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only alphabets and numbers allowed.")]
        public string TagTitle { get; set; }

    }





}