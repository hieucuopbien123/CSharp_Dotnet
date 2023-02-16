using System;
using System.IO;
using System.Text;

namespace SecondIntermediateProject
{
    class Program
    {
        // # Thao tác với System.IO
        // DriveInfo
        public static void GetDrivesInfo()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                Console.WriteLine("Drive {0}", d.Name);
                Console.WriteLine("  Drive type: {0}", d.DriveType);
                //Class DriveType là kiểu ổ đĩa(System.IO.DriveType): CDRom, Fixed, Network, NoRootDirectory, Ram, Removable, Unknown.
                if (d.IsReady == true)
                {
                    Console.WriteLine("  Volume label: {0}", d.VolumeLabel);
                    Console.WriteLine("  File system: {0}", d.DriveFormat);
                    Console.WriteLine("  Available space to current user:{0, 15} bytes", d.AvailableFreeSpace);
                    Console.WriteLine("  Total available space:          {0, 15} bytes", d.TotalFreeSpace);
                    Console.WriteLine("  Total size of drive:            {0, 15} bytes ", d.TotalSize);
                }
            }
        }

        // Path
        // File
        static void testAppendAllText()
        {
            var filename = "test.txt";
            string contentfile = "\nXin chào! xuanthulab.net - " + DateTime.Now.ToString();

            // Thư mục Documents mà mọi máy win đều có
            var directory_mydoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fullpath = Path.Combine(directory_mydoc, filename);

            if (File.Exists(fullpath))
            {
                File.AppendAllText(fullpath, contentfile); // File đã tồn tại - nối thêm nội dung
            }
            else
            {
                File.WriteAllText(fullpath, contentfile);
                // Hàm WriteAllText có đầy đủ tạo file nếu chưa tồn tại, mở file nếu đã tồn tại, r ghi vào, rồi tự đóng file. Chẳng qua AppenAllText k tự check điều đó nên mới phải check ở đây
            }
            Console.WriteLine($"Save at {directory_mydoc}{Path.DirectorySeparatorChar}{filename}");
            string s = File.ReadAllText(fullpath); // Đọc tất cả, cũng tự động
            Console.WriteLine(s);
        }

        // Directory
        public static void getAllOfDirectory()
        {
            var directory_mydoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            String[] files = System.IO.Directory.GetFiles(directory_mydoc);
            String[] directories = System.IO.Directory.GetDirectories(directory_mydoc);

            foreach (var file in files)
            {
                Console.WriteLine(file);
            }

            foreach (var directory in directories)
            {
                Console.WriteLine(directory);
            }
        }
        static void ListFileRecursiveInDirectory(string path)
        {
            String[] directories = System.IO.Directory.GetDirectories(path);
            String[] files = System.IO.Directory.GetFiles(path);
            foreach (var file in files)
            {
                Console.WriteLine(file);
            }
            foreach (var directory in directories)
            {
                Console.WriteLine(directory);
                ListFileRecursiveInDirectory(directory); // Đệ quy
            }
        }
        static void Main(string[] args)
        {
            GetDrivesInfo();
            //testAppendAllText();
            getAllOfDirectory();
            //ListFileRecursiveInDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)); // Phải cấp quyền

            // # Dùng Stream / FileStream
            string filepath = "../../../../test.txt"; // Tương đối từ trong thư mục có file exe trong debug đổ ra
            using (var stream = new FileStream(path: filepath, mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.Read))
            {
                Console.WriteLine(stream.Name);
                Console.WriteLine($"Stream size {stream.Length} bytes / Position {stream.Position}");
                Console.WriteLine($"Stream can : Read {stream.CanRead} -  Ghi {stream.CanWrite} - Seek {stream.CanSeek} - Timeout {stream.CanTimeout} ");

                Console.WriteLine(UtilsEncoding.GetEncoding(stream));
            }

            // # Dùng Stream / FileStream / Ghi file
            // Ghi file kèm BOM - Preamble
            string filepath2 = "../../../../test2.txt";
            using (var stream = new FileStream(path: filepath2, mode: FileMode.Create, access: FileAccess.Write, share: FileShare.None))
            {
                // Write BOM - UTF8
                Encoding encoding = Encoding.UTF8;
                byte[] bom = encoding.GetPreamble();
                stream.Write(bom, 0, bom.Length);

                string s1 = "Xuanthulab.net -  Xin chào các bạn! \n";
                string s2 = "Ví dụ - ghi file text bằng stream";

                // Encode chuỗi - lưu vào mảng bytes
                byte[] buffer = encoding.GetBytes(s1); // Từ string sang byte[]
                stream.Write(buffer, 0, buffer.Length); 

                buffer = encoding.GetBytes(s2);
                stream.Write(buffer, 0, buffer.Length); 
            }

            // Đọc file kèm BOM
            string filepath3 = "../../../../test2.txt";
            int SIZEBUFFER = 256;
            using (var stream = new FileStream(path: filepath3, mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.Read))
            {
                Encoding encoding = UtilsEncoding.GetEncoding(stream);
                Console.WriteLine(encoding.ToString());
                byte[] buffer = new byte[SIZEBUFFER];
                bool endread = false;
                do
                {
                    int numberRead = stream.Read(buffer, 0, SIZEBUFFER);
                    if (numberRead == 0) endread = true;
                    if (numberRead < SIZEBUFFER)
                    {
                        Array.Clear(buffer, numberRead, SIZEBUFFER - numberRead);
                        // Ta đọc cả lượng 256 bytes là thừa, có thể ra ký tự rác. Cần xóa phần thừa
                    }
                    string s = encoding.GetString(buffer, 0, numberRead); // byte sang string
                    Console.WriteLine(s);

                } while (!endread);
            }

            // # Dùng Collection 
            UseCollections test = new UseCollections();
            Console.WriteLine(test.ToString("N", null));
            Console.WriteLine(test);
        }
    }
    public class UtilsEncoding
    {
        // # Dùng Stream / FileStream
        // Xác định kiểu encoding dựa vào BOM
        public static Encoding GetEncoding(Stream stream)
        {
            byte[] BOMBytes = new byte[4]; // Chỉ cần max 4 bytes đầu là biêt
            int offset = 0; 
            int count = 4; 
            int numberbyte = stream.Read(BOMBytes, offset, count); // đọc 4 bytes lưu buffer

            // Theo wiki
            if (BOMBytes[0] == 0xfe && BOMBytes[1] == 0xff)
            {
                stream.Seek(2, SeekOrigin.Begin); // Di chuyển về vị trí bắt đầu của dữ liệu (đã trừ BOM)
                return Encoding.BigEndianUnicode;
            }
            if (BOMBytes[0] == 0xff && BOMBytes[1] == 0xfe)
            {
                stream.Seek(2, SeekOrigin.Begin); // Di chuyển về vị trí bắt đầu của dữ liệu (đã trừ BOM)
                return Encoding.Unicode;
            }

            if (BOMBytes[0] == 0xef && BOMBytes[1] == 0xbb && BOMBytes[2] == 0xbf)
            {
                stream.Seek(3, SeekOrigin.Begin);
                return Encoding.UTF8;
            }
            if (BOMBytes[0] == 0x2b && BOMBytes[1] == 0x2f && BOMBytes[2] == 0x76)
            {
                stream.Seek(3, SeekOrigin.Begin);
                return Encoding.UTF7;
            }
            if (BOMBytes[0] == 0xff && BOMBytes[1] == 0xfe && BOMBytes[2] == 0 && BOMBytes[3] == 0)
            {
                stream.Seek(4, SeekOrigin.Begin);
                return Encoding.UTF32;
            }
            if (BOMBytes[0] == 0 && BOMBytes[1] == 0 && BOMBytes[2] == 0xfe && BOMBytes[3] == 0xff)
            {
                stream.Seek(4, SeekOrigin.Begin);
                return Encoding.GetEncoding(12001);
            }

            // Nếu không vào type nào tức k có BOM - Preamble, mặc định coi là UTF8
            stream.Seek(0, SeekOrigin.Begin);
            return Encoding.Default;
        }
    }
}
