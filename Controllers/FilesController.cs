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
            using (var zipStream = new System.IO.Compression.HttpZipStream("https://siasky.net/" + hash))
            {
                System.Console.WriteLine("ZZZ: " + "https://siasky.net/" + hash);
                string content = "";
                var entryList = await zipStream.GetEntriesAsync();
                foreach (var e in entryList) {
                    if (e.FileName != path) continue;
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
                return View(content);
            }
        }
    }
}
