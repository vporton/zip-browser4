using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using zip_browser4.Models;

namespace zip_browser4.Controllers
{
    public class FilesController : Controller
    {
        public string Test() {
            return "TEST";
        }

        [Route("Files/Zip/{hash}/{path}")]
        [HttpGet]
        public async Task<IActionResult> Zip(string hash, string path)
        {
            var zipStream = new System.IO.Compression.HttpZipStream("https://siasky.net/" + hash);
            if(await zipStream.GetContentLengthAsync() == -1)
                NotFound("No such Sia hash.");
            using (zipStream)
            {
                await zipStream.GetContentLengthAsync();
                string content = "";
                var entryList = await zipStream.GetEntriesAsync();
                bool found = false;
                foreach (var e in entryList) {
                    if (e.FileName != path) continue;
                    found = true;
                    await zipStream.ExtractAsync(e, async (entryStream) =>
                    {
                        byte[] buffer = new byte[4096];
                        while(true) {
                            int result = await entryStream.ReadAsync(buffer, content.Length, 4096);
                            if(result == 0) break;
                            content += buffer.ToString();
                        }
                    });
                }
                if (!found) return NotFound("No such file in archive.");
                return View(content);
            }
        }
    }
}
