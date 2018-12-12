using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tome.Models
{
    public class TomeContent
    {
        public Tome requestTome { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        
    }
}