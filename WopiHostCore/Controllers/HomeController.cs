using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using WopiHostCore.Models;

namespace WopiHostCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var testLinks = GetTestLinks();
            var model = new FileRequest
            {
                name = _configuration["appSampleLink"],
                Items = testLinks
            };

            ViewBag.RootPath = "";

            return View(model);
        }

        public ActionResult FramedView()
        {
            var testLinks = GetTestLinks();
            var model = new FileRequest
            {
                name = _configuration["appSampleLink"],
                Items = testLinks
            };

            ViewBag.RootPath = "";

            return View(model);
        }

        public ActionResult Upload()
        {
            ViewBag.RootPath = "";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IEnumerable<SelectListItem> GetTestLinks()
        {
            var appDataPath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            var files = System.IO.Directory.GetFiles(appDataPath, "*.*")
                .Where(s => s.EndsWith(".docx") ||
                    s.EndsWith(".xlsx") ||
                    s.EndsWith(".pptx") ||
                    s.EndsWith(".pdf"));

            var rv = new List<SelectListItem>();
            foreach (string item in files)
            {
                rv.Add(new SelectListItem
                {
                    Selected = false,
                    Text = System.IO.Path.GetFileName(item),
                    Value = System.IO.Path.GetFileName(item)
                });
            }
            return rv;
        }
    }
}
