using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tome.Models
{
    public class TomeViewModel
    {
        public Tome ReferredTome { get; set; }
        public TomeContent TomeContent { get; set; }
        public IEnumerable<SelectListItem> TagList { get; set; }
        public int SelectedTag { get; set; }

    }
}