using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WopiHostCore.Filters;
using WopiHostCore.Helpers;
using WopiHostCore.Models;

namespace WopiHostCore.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        string _rootStoragePath;
        private readonly IConfiguration _configuration;

        public UploadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Provides multiple file upload from an HTTP client
        /// </summary>
        /// <returns>array of files uploaded and links on OWA</returns>
        [HttpPost]
        [Route("PostFile")]
        [DisableFormValueModelBinding]
        public async Task<List<Link>> PostFile()
        {
            // Check if the request contains multipart/form-data.
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
                return await Task<List<Link>>.FromResult<List<Link>>(null);
            }

            _rootStoragePath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            var provider = new MultipartFormDataStreamProvider(_rootStoragePath);

            try
            {
                StringBuilder sb = new StringBuilder(); // Holds the response body 

                var files = RenameFiles(provider);
                var rv = BuildLinks(files);

                await Request.StreamFiles(_rootStoragePath, files);

                foreach (var file in files)
                {
                    sb.Append(string.Format("Uploaded file: {0}\n", file));
                    // Read the form data and return an async task. 
                }

                return rv;
            }
            catch (Exception e)
            {
                throw new ApplicationException("failed to accept files", e);
            }
        }

        List<Link> BuildLinks(List<Link> files)
        {
            var appDataPath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            var xml = _configuration["appDiscoveryXml"];
            var wopiServer = _configuration["appWopiServer"];
            WopiAppHelper wopiHelper = new WopiAppHelper(Path.Combine(appDataPath, xml));

            foreach (Link link in files)
            {
                try
                {
                    var tlink = wopiHelper.GetDocumentLink(wopiServer + link.FileName);
                    link.Url = tlink;
                }
                catch (ArgumentException ex)
                {
                    link.Url = "bad file ext: " + ex.Message;
                }
            }

            return files;
        }

        List<Link> RenameFiles(MultipartFormDataStreamProvider provider)
        {
            List<Link> rv = new List<Link>();
            foreach (MultipartFileData fileData in provider.FileData)
            {
                if (string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName))
                {
                    throw new ApplicationException("invalid request format, must set content disposition header");
                }
                string fileName = fileData.Headers.ContentDisposition.FileName;
                if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                {
                    fileName = fileName.Trim('"');
                }
                if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                {
                    fileName = Path.GetFileName(fileName);
                }

                //fileName = fileName.Replace(" ", string.Empty);

                string targetFile = Path.Combine(_rootStoragePath, fileName);
                if (System.IO.File.Exists(targetFile))
                {
                    System.IO.File.Delete(targetFile);
                }

                System.IO.File.Move(fileData.LocalFileName, targetFile);
                rv.Add(new Link { FileName = fileName });
            }

            return rv;
        }
    }
}
