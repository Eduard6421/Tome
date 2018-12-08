using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Tome.Models
{
    public class TagReference
    {
        [Key]
        public int TagReferenceId { get; set; }

        [ForeignKey("Tome")]
        public int TomeId { get; set; }
        public virtual Tome Tome { get; set; }
    

        [ForeignKey("Tag")]
        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }
    }

    
}