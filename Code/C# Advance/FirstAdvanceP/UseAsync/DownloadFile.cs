using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UseAsync
{
    class DownloadFile
    {
        public static async Task Download(string url)
        {
            Action downloadaction = () => {
                using (var client = new WebClient())
                {
                    Console.Write("Starting download ..." + url);
                    
                    // # Module Network C# / Tải data từ remote
                    // Tải về từ remote luôn ra byte[]
                    byte[] data = client.DownloadData(new Uri(url));

                    // Lấy tên file để lưu, mặc định truyền vào cuối url là 1 tên 1 file
                    string filename = System.IO.Path.GetFileName(url);
                    System.IO.File.WriteAllBytes(filename, data);
                }
            };

            Task task = new Task(downloadaction);
            task.Start();

            await task;
            Console.WriteLine("Download file finish");
        }
    }
}
