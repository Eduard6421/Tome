using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Tome.Models
{
    public class CurrentVersion
    {
        [Key]
        public int CurrentVersionId { get; set; }


        [ForeignKey("Tome")]
        public int TomeId { get; set; }
        public virtual Tome Tome { get; set; }

        [ForeignKey("TomeHistory")]
        public int TomeHistoryId { get; set; }
        public virtual TomeHistory TomeHistory { get; set; }

    }
}