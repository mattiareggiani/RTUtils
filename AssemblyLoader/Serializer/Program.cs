using System;
using System.IO;
using System.Reflection;

namespace Serializer
{
    class Program
    {
        static void Main(string[] args)
        {
            Aes aes = new Aes(Config.Key, Config.IV);
            if (args.Length < 1)
            {
                Console.WriteLine("Specify a .NET executable");
                return;
            }
            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.WriteLine("The file does not exist");
                return;
            }
            Byte[] fileBytes = File.ReadAllBytes(filePath);
            string encryptedBase64String = aes.EncryptToBase64String(fileBytes);
            File.WriteAllText("serialised.txt", encryptedBase64String);
                     
            
        }
    }
}
