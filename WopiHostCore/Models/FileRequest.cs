using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WopiHostCore.Models
{
    public class FileRequest
    {
        public string name { get; set; }

        public string SelectedItemId { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
    }
}
