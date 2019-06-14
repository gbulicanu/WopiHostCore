using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WopiHostCore.Helpers;
using WopiHostCore.Models;

namespace WopiHostCore.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LinkController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Provides a link that can be used to Open a document in the relative viewer
        /// from the Office Web Apps server
        /// </summary>
        /// <param name="fileRequest">indicates the request type</param>
        /// <returns>A link usable for HREF</returns>
        [Route("GetLink")]
        public Link GetLink([FromQuery] FileRequest fileRequest)
        {
            if (ModelState.IsValid)
            {
                var xml = _configuration["appDiscoveryXml"];
                var wopiServer = _configuration["appWopiServer"];
                bool updateEnabled = false;
                bool.TryParse(_configuration["updateEnabled"], out updateEnabled);
                var appDataPath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
                WopiAppHelper wopiHelper = new WopiAppHelper(System.IO.Path.Combine(appDataPath, xml), updateEnabled, _configuration);

                var result = wopiHelper.GetDocumentLink(wopiServer + fileRequest.name);

                var rv = new Link
                {
                    Url = result
                };
                return rv;
            }

            throw new ApplicationException("Invalid ModelState");
        }
    }
}