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
        public async Task<IActionResult> Get(String hash, String path)
        {
            using (var zipStream = new System.IO.Compression.HttpZipStream("http://remoteArchive.zip"))
            {
                string content = "";
                var entryList = await zipStream.GetEntriesAsync();
                foreach (var e in entryList) {
                    await zipStream.ExtractAsync(e, async (entryStream) =>
                    {
                        if (e.FileName != path) return;
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
