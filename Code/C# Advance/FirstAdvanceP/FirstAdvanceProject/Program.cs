using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using UseDLL;

namespace FirstAdvanceProject
{
    public class Employer
    {
        // # Data Annotation / Dùng Data Annotation của C# có sẵn
        [Required(ErrorMessage = "Employee {0} is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Ten tu 3 den 100 ky tu thoi")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Range(18, 99, ErrorMessage = "Age should be between 18 and 99")]
        public int Age { get; set; }


        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string PhoneNumber { set; get; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
    }

    class Program
    {
        // # Data Annotation / Dùng Data Annotation của C# có sẵn
        [Obsolete("Obsolete method -> use ABC instead nha")]
        public static void Method1()
        {
            Console.WriteLine("Call method1");
        }

        // Check Data Annotation bằng lớp Validator 
        public static void checkValidationContext()
        {
            Employer user = new Employer();
            user.Name = "AF";
            user.Age = 6;
            user.PhoneNumber = "1234as";
            user.Email = "test@re";

            ValidationContext context = new ValidationContext(user, null, null);
            // results lưu danh sách ValidationResult là kết quả kiểm tra
            List<ValidationResult> results = new List<ValidationResult>();

            // Check
            bool valid = Validator.TryValidateObject(user, context, results, true);

            ConsoleColor temp = Console.ForegroundColor;
            if (!valid)
            {
                // Duyệt qua các lỗi và in ra
                foreach (ValidationResult vr in results)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write($"{vr.MemberNames.First(),13}");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"    {vr.ErrorMessage}");
                }
            }
            Console.ForegroundColor = temp;
        }
        static void Main(string[] args)
        {
            Method1();
            TestReadAttribute.test();
            checkValidationContext();

            String html = "Example use dll HtmlHelper".HtmlTag("div", "text-danger");
            Console.WriteLine(html);
        }
    }
}
