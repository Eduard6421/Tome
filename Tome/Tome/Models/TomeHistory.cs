using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Tome.Models
{
    public class TomeHistory
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime ModificationDate { get; set; }

        [ForeignKey("Tome")]
        public int TomeId { get; set; }
        public virtual Tome Tome { get; set; }

        public string FilePath { get; set; }


        [MaxLength(128), ForeignKey("ApplicationUser")]
        public virtual string UserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

    }



}