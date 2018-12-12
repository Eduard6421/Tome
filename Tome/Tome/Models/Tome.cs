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
        public string Name { get; set; }
        
        public DateTime CreationDate { get; set; }

        public bool IsPrivate { get; set; } = false;
        

        //[MaxLength(128), ForeignKey("ApplicationUser")]
        //public virtual string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }


    }
    





}