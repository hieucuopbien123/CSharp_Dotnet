using System;
using System.Collections.Generic;
using Contract;
using AlexDataAnalyser;
using System.IO;
using System.Text;
using System.Linq;

namespace CSharpBasic
{
    // # Biến và kiểu dữ liệu
    class Program
    {
        static public long[] tempStr = new long[10];
        static public int[] temp = new int[10];
        static public void saveCount(long str, int count)
        {
            for (int i = 0; i < 10; i++)
            {
                if (count > temp[i])
                {
                    for (int j = 9; j > i && j > 0; j--)
                    {
                        temp[j] = temp[j - 1];
                        tempStr[j] = tempStr[j - 1];
                    }
                    temp[i] = count;
                    tempStr[i] = str;
                    break;
                }
            }
        }
        static public long[] SortArray(long[] array, int leftIndex, int rightIndex)
        {
            var i = leftIndex;
            var j = rightIndex;
            var pivot = array[leftIndex];
            while (i <= j)
            {
                while (array[i] < pivot)
                {
                    i++;
                }

                while (array[j] > pivot)
                {
                    j--;
                }
                if (i <= j)
                {
                    long temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                    i++;
                    j--;
                }
            }

            if (leftIndex < j)
                SortArray(array, leftIndex, j);
            if (i < rightIndex)
                SortArray(array, i, rightIndex);
            return array;
        }
        static void Main(string[] args)
        {
            // Bài toán là tìm kiểu dữ liệu nhỏ nhất sao cho lưu được 4000000 string 10 ký tự. Cả yêu cầu về tốc độ xử lý

            // Dùng Dictionay bth nó cũng tốn nhiều bộ nhớ vì các bộ nhớ có sẵn của C# nó luôn thêm hàng đống property và method vào. Khi cần xử lý lượng data lớn thì nên cân nhắc.
            // List tốn hơn array thuần 1 tẹo thôi nhưng cũng tương tự k search được nhưng Sort của List nhanh hơn quick sort khi implement trên array thuần, dùng nào cũng được. HashTable > Dictionary vì non generic. SortedMap add lâu hơn 10p và cũng tốn nhiều bộ nhớ hơn Dictionary vì mỗi lần thêm nó sort lại, dictionary ngang hashmap.

            // Khi khai báo mặc định thì capacity của collection sẽ tự mở rộng theo cấp số mũ khi thêm các phần tử vào nên có thể mở rộng chiếm 1 lượng bộ nhớ rất lớn. Với bộ nhớ lớn nên ước lượng để fix capacity như dưới. 
            // Dictionary<long, int> strings = new Dictionary<long, int>(38362866);
            // List<long> longs = new List<long>(4000000);
            // HashSet<long> stringsSet = new HashSet<long>(38362866);
            // => Ta đã tính trước chuẩn cần lưu bao nhiêu phần tử vào rồi nhưng vẫn rất lâu

            // 3 thứ: Tốc độ tìm + tốc độ add + bộ nhớ.
            // Hay: forloop nhanh hơn foreach loop nếu các phần tử trong loop đó của array ta chỉ access 1 lần. Nếu access nhiều lần thì dùng foreach nhanh hơn vì foreach nó tách thành 1 biến riêng và truy xuất đến biến đó. Còn forloop lại tìm index để truy xuất

            // Khi code, các biến scope k cần lo bộ nhớ vì ra khỏi là mất, nhưng bộ nhớ sẽ rất tốn nếu trong 1 scope mà dùng quá nhiều
            // Nếu ta dùng 1 struct để lưu đúng 2 thứ là string và tần suất thì bộ nhớ vẫn rất lớn tức là dường như vô lý để lưu ngay. Cách duy nhất là lưu mảng string thuần bình thường để đọc data từ file, r sau đó mới xử lý logic đếm tần suất sau với quy hoạch động chứ vừa đọc vừa có tần suất luôn là vô lý về bộ nhớ.

            long[] longs = new long[49999901];
            int x = 0;
            for (int i = 1; i <= 10; i++)
            {
                // Dùng đường dẫn tương đối
                using (StreamReader sr = File.OpenText(@"..\..\..\..\Data\a" + i + ".dat"))
                {
                    string a;
                    string[] words;
                    while ((a = sr.ReadLine()) != null)
                    {
                        words = a.Split(';');
                        for (int j = 0; j < 10; j++)
                        {
                            long hashCode = getHashCodeFromString(words[j].ToLower());
                            longs[x] = 1;
                            x++;
                            // longs.Add(hashCode); 
                            // Khi dùng List thì gọi như trên sẽ dịch con trỏ cuối nhanh hơn chạy từ index (đáng kể khi thao tác số lượng lớn) => lần đầu tiên biết dùng index còn bị chậm
                        }
                        
                    }
                }
                Console.WriteLine("Run file::" + i);
                // Dùng GC Collector giảm bộ nhớ khá nhiều nhưng k nên gọi nhiều vì chạy lâu
                // if (GC.GetTotalMemory(false) > 800000000)
                // {
                //     System.GC.Collect();
                // }
            }
            // Tự implement Quick Sort thủ công cho nhanh
            longs = SortArray(longs, 0, 49999900);
            int count = 1;
            for (int i = 0; i < longs.Count() - 1; i++)
            {
                if (longs[i] == longs[i + 1])
                {
                    count++;
                }
                else
                {
                    saveCount(longs[i], count);
                    count = 1;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(DecodeStr(tempStr[i]) + " - " + temp[i]);
            }

            // Nếu cần làm gì với collection của C# kiểu SQL thì dùng linq ngay như dưới
            // var sortedDict = (from entry in strings orderby entry.Value descending select entry).Take(10);

            Console.ReadLine();
        }

        // Dùng string builder và string thực ra k thay đổi nhiều lắm về bộ nhớ dù nói là tạo string mới. Nhưng StringBuilder chắc chắn nhanh hơn vì string thuần tốn thêm thời gian khởi tạo vùng nhớ mới
        // Lấy hash code unique: có thể có 26 từ nên mỗi từ dùng 5 bit biểu diễn, chỉ cần nối 10 string 5 bit đó lại r biến thành 1 số int64 (long) là được
        static public long getHashCodeFromString(string a)
        {
            // # Biến và kiểu dữ liệu / Dùng Convert 
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < a.Length; i++)
            {
                // Lớp Convert hỗ trợ 
                string stringofA = Convert.ToString(a[i] - 97, 2);
                stringofA = stringofA.PadLeft(5, '0');
                str.Append(stringofA);
            }
            return Convert.ToInt64(str.ToString(), 2);
        }
        // Decode làm ngược lại
        public static string DecodeStr(long a)
        {
            StringBuilder temp = new StringBuilder("");
            StringBuilder result = new StringBuilder("");
            int value;
            string binaryStr = Convert.ToString(a, 2);
            for (int i = 0; i < 50 - binaryStr.Length; i++)
            {
                temp.Append("0");
            }
            temp.Append(binaryStr);
            string tmp = temp.ToString();

            for (int i = 0; i <= 45; i += 5)
            {
                value = Convert.ToInt32(tmp.Substring(i, 5), 2) + 97;
                result.Append((char)value);
            }
            return result.ToString();
        }
    }
}