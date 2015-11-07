using System;
using System.Security.Cryptography;

namespace CreateKeys {
    class Program {
        static void Main(string[] args) {
            using (Aes myAes = Aes.Create()) {
                Console.WriteLine("Generating AES keys");
                Console.Write("Key: ");
                var key = Convert.ToBase64String(myAes.Key);
                Console.WriteLine(key);
                Console.Write("IV: ");
                var iV = Convert.ToBase64String(myAes.IV);
                Console.WriteLine(iV);
                Console.WriteLine("Press any key to quit.");
                Console.ReadKey();
            }
        }
    }
}
