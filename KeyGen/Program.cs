using System;
using System.Security.Cryptography;

namespace KeyGen
{
    class Program
    {
        static void Main(string[] args)
        {
            HMACSHA256 hmac = new HMACSHA256();

            var key = hmac.Key;

            var rv = Convert.ToBase64String(key);

            Console.WriteLine(rv);
            Console.WriteLine("press enter to exit...");
            Console.ReadLine();
        }
    }
}
