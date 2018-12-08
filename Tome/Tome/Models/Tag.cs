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
        public string TagTitle { get; set; }

    }





}