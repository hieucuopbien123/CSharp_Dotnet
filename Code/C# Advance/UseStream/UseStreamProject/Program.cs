using System;

namespace UseStreamProject
{
    class Program
    {
        static void Main(string[] args)
        {
            //UseFileStream.TestStreamWrite();
            //UseFileStream.TestStreamRead();
            //UseBufferedStream.WriteUseBuffer();
            UseMemoryStream.UseMemStream();

            Console.ReadLine();
        }
    }
}
