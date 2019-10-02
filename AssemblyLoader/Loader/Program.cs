using System;
using System.Linq;
using System.Reflection;

namespace Loader
{
    class Program
    {
        static void Main(string[] args)
        {
            string assembly = "PASTE THE OUTPUT FROM SERIALIZER HERE";
            object[] cmd = args.ToArray();
            Aes aes = new Aes(Config.Key, Config.IV);
            Byte[] bin = aes.DecryptFromBase64String(assembly);
            loadAssembly(bin, cmd);

        }
        public static void loadAssembly(byte[] bin, object[] commands)
        {
            Assembly a = Assembly.Load(bin);
            try
            {
                a.EntryPoint.Invoke(null, new object[] { commands });
            }
            catch
            {
                MethodInfo method = a.EntryPoint;
                if (method != null)
                {
                    object o = a.CreateInstance(method.Name);
                    method.Invoke(o, null);
                }
            }            
        }
    }
}
