using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using MainWeb.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WopiHostCore.Filters;
using WopiHostCore.Helpers;
using WopiHostCore.Models;

namespace WopiHostCore.Controllers.Api
{
    [Route("api/wopi/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        IFileHelper _fileHelper;
        IKeyGen _keyGen;

        /// <summary>
        /// Constructor for DI
        /// </summary>
        /// <param name="fileHelper">File helper implementation</param>
        /// <param name="keyGen">KeyGen helper implementation</param>
        public FilesController(IFileHelper fileHelper, IKeyGen keyGen, IConfiguration configuration)
        {
            _fileHelper = fileHelper;
            _keyGen = keyGen;
            _configuration = configuration;

        }

        /// <summary>
        /// Returns the metadata about an office document
        /// </summary>
        /// <param name="name">filename</param>
        /// <param name="access_token">access token generated for this server</param>
        /// <returns></returns>
        [HttpGet(@"{name}/")]
        public CheckFileInfo GetFileInfo(string name, [FromQuery] string access_token)
        {
            Validate(name, access_token);

            var fileInfo = _fileHelper.GetFileInfo(name);

            bool updateEnabled = false;
            if (bool.TryParse(_configuration["updateEnabled"], out updateEnabled))
            {
                fileInfo.SupportsUpdate = updateEnabled;
                fileInfo.UserCanWrite = updateEnabled;
                fileInfo.SupportsLocks = updateEnabled;
            }

            return fileInfo;
        }

        // GET api/<controller>/5
        /// <summary>
        /// Get a single file contents
        /// </summary>
        /// <param name="name">filename</param>
        /// <returns>a file stream</returns>
        [HttpGet(@"{name}/contents")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None)]
        public FileStreamResult Get(string name, [FromQuery] string access_token)
        {
            try
            {
                var appDataPath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
                Validate(name, access_token);
                    //Response.StatusCode = 200;
                    //Response.ContentType = "application/octet-stream";
                    ////Response.Headers.Add("Content-disposition", $"attachment; filename={name}");
                    //file.CopyTo(Response.Body);
                return File(System.IO.File.OpenRead(Path.Combine(appDataPath, name)), "application/octet-stream");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return null;
            }
        }

        // POST api/<controller>
        /// <summary>
        /// Not implemented, but will provide editing (simple)
        /// </summary>
        /// <param name="access_token"></param>
        [HttpPost(@"{name}/contents")]
        [DisableFormValueModelBinding]
        public async void Post(string name, [FromQuery] string access_token)
        {
            var appData = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            var fileExt = name.Substring(name.LastIndexOf('.') + 1);

            var outFile = Path.Combine(
                appData, name);
            //Guid.NewGuid().ToString() +
            //"_" +
            //name);

            //var fi = new FileInfo(outFile);
            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                await System.IO.File.WriteAllBytesAsync(outFile, ms.ToArray());
            }

        }

        // Action saves the request’s content into an Azure blob 
        public Task PostUploadfile(string destinationBlobName, [FromBody] byte[] body)
        {
            // string should come from URL, we’ll read content body ourselves.
            Stream azureStream = System.IO.File.Open("", FileMode.OpenOrCreate); ;// OpenAzureStorage(destinationBlobName); // stream to write to azure
            return azureStream.WriteAsync(body, 0, body.Length); // upload body contents to azure. 
        }

        void Validate(string name, string access_token)
        {
            //var decoded = access_token);
            var isValid = _keyGen.Validate(name, access_token);
            if (!isValid)
                throw new SecurityException("Access token doesn't match calculated token");
        }

    }
}