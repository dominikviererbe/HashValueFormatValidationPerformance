using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace HashValueFormatValidationPerformance
{
    class HashValidator
    {
        public Regex ValidationPattern { get; }
        public int HashValueLength { get; }

        public HashValidator(int hashValueLength)
        {
            HashValueLength = hashValueLength;
            ValidationPattern = new Regex("\\A[0-9A-Fa-f]{" + hashValueLength + "}\\z", RegexOptions.Compiled);
        }

        public bool ValidateWithForeachLoop1(string hash)
        {
            if (hash == null || hash.Length != HashValueLength)
                return false;

            foreach (char c in hash)
            {
                if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
                    return false;
            }

            return true;
        }

        public bool ValidateWithForeachLoop2(string hash)
        {
            if (hash == null || hash.Length != HashValueLength)
                return false;

            foreach (char c in hash.ToCharArray())
            {
                if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
                    return false;
            }

            return true;
        }

        public bool ValidateWithForLoop1(string hash)
        {
            if (hash == null || hash.Length != HashValueLength)
                return false;

            char c;

            for (int i = 0; i < hash.Length; ++i)
            {
                 c = hash[i];
                
                if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
                    return false;
            }

            return true;
        }

        public bool ValidateWithForLoop2(string hash)
        {
            if (hash == null || hash.Length != HashValueLength)
                return false;

            char c;
            char[] charArr = hash.ToCharArray();

            for (int i = 0; i < hash.Length; ++i)
            {
                c = charArr[i];
                
                if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
                    return false;
            }

            return true;
        }

        public bool ValidateWithBidirectionalForLoop(string hash)
        {
            if (hash == null || hash.Length != HashValueLength)
                return false;

            char c;
            char[] charArr = hash.ToCharArray();
            int end = charArr.Length / 2;

            for (int i = 0; i < end;)
            {
                c = charArr[i++];
                
                if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
                    return false;

                c = charArr[^i];

                if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
                    return false;
            }

            return true;
        }

        const string allowedChars = "0123456789ABCDEFabcdef";

        public bool ValidateWithLinq(string hash)
        {
            return hash != null && hash.Length == HashValueLength && hash.All(c => allowedChars.Contains(c));
        }

        public bool ValidateWithRegex(string hash)
        {
            return hash != null && ValidationPattern.IsMatch(hash);
        }
    }

    class Program
    {
        static string ByteHashToStringHash(byte[] hash)
        {
            StringBuilder hashString = new StringBuilder();

            for (int i = 0; i < hash.Length; ++i)
            {
                hashString.Append(hash[i].ToString("X2"));
            }

            return hashString.ToString();
        }

        static void Main(string[] args)
        {
            int sampleSize = 10_000;

            if (args.Length == 1)
            {
                if (!int.TryParse(args[0], out sampleSize))
                {
                    Console.Error.WriteLine("Sample-Size has to be a number!");
                
                    Environment.ExitCode = 1;
                    return;    
                }

                if (sampleSize <= 0)
                {
                    Console.Error.WriteLine("Sample-Size has to be a positive!");
                
                    Environment.ExitCode = 1;
                    return;    
                }
            }
            else if (args.Length > 1)
            {
                Console.Error.WriteLine("Too many Arguments!");
                
                Environment.ExitCode = 1;
                return;
            }

            Console.Write($"1. Generating {sampleSize} Random Hash Strings");

            byte[] dataBuffer = new byte[512];

            string[] md5Hahses = new string[sampleSize * 2];
            string[] sha1Hahses = new string[sampleSize * 2];
            string[] sha256Hahses = new string[sampleSize * 2];
            string[] sha384Hahses = new string[sampleSize * 2];
            string[] sha512Hahses = new string[sampleSize * 2];

            HashAlgorithm md5 = HashAlgorithm.Create("MD5");
            HashAlgorithm sha1 = HashAlgorithm.Create("SHA1");
            HashAlgorithm sha256 = HashAlgorithm.Create("SHA256");
            HashAlgorithm sha384 = HashAlgorithm.Create("SHA384");
            HashAlgorithm sha512 = HashAlgorithm.Create("SHA512");

            byte[] hashBuffer;
            string hashValue;

            int dividor = sampleSize < 10 ? 1 : sampleSize / 10;

            for (int i = 0; i < sampleSize; ++i)
            {
                //progess bar
                if (i % dividor == 0)
                {
                    Console.Write(".");
                }

                RNGCryptoServiceProvider.Fill(dataBuffer);

                //MD5
                hashBuffer = md5.ComputeHash(dataBuffer);
                hashValue = ByteHashToStringHash(hashBuffer);

                md5Hahses[i] = hashValue;
                md5Hahses[i+sampleSize] = hashValue.ToLower();

                //SHA1
                hashBuffer = sha1.ComputeHash(dataBuffer);
                hashValue = ByteHashToStringHash(hashBuffer);

                sha1Hahses[i] = hashValue;
                sha1Hahses[i+sampleSize] = hashValue.ToLower();

                //SHA256
                hashBuffer = sha256.ComputeHash(dataBuffer);
                hashValue = ByteHashToStringHash(hashBuffer);

                sha256Hahses[i] = hashValue;
                sha256Hahses[i+sampleSize] = hashValue.ToLower();
                
                //SHA384
                hashBuffer = sha384.ComputeHash(dataBuffer);
                hashValue = ByteHashToStringHash(hashBuffer);

                sha384Hahses[i] = hashValue;
                sha384Hahses[i+sampleSize] = hashValue.ToLower();

                //SHA512
                hashBuffer = sha512.ComputeHash(dataBuffer);
                hashValue = ByteHashToStringHash(hashBuffer);

                sha512Hahses[i] = hashValue;
                sha512Hahses[i+sampleSize] = hashValue.ToLower();
            }

            Console.WriteLine(" Done!");

            Console.WriteLine("2. Measuring:");
            
            Console.WriteLine("  MD5:");
            Measure(md5Hahses, new HashValidator(32));

            Console.WriteLine("  SHA1:");
            Measure(sha1Hahses, new HashValidator(40));

            Console.WriteLine("  SHA256:");
            Measure(sha256Hahses, new HashValidator(64));

            Console.WriteLine("  SHA384:");
            Measure(sha384Hahses, new HashValidator(96));

            Console.WriteLine("  SHA512:");
            Measure(sha512Hahses, new HashValidator(128));
        }

        public static void Measure(string[] hashes, HashValidator validator)
        {
            int i;
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            
            for (i = 0; i < hashes.Length; ++i)
            {
                validator.ValidateWithRegex(hashes[i]);
            }
            
            stopwatch.Stop();

            Console.WriteLine("    ValidateWithRegex:                    " + FormatTimeSpan(stopwatch.Elapsed));
            
            stopwatch.Reset();
            stopwatch.Start();
            
            for (i = 0; i < hashes.Length; ++i)
            {
                validator.ValidateWithLinq(hashes[i]);
            }
            
            stopwatch.Stop();

            Console.WriteLine("    ValidateWithLinq:                     " + FormatTimeSpan(stopwatch.Elapsed));

            stopwatch.Reset();
            stopwatch.Start();
            
            for (i = 0; i < hashes.Length; ++i)
            {
                validator.ValidateWithForLoop1(hashes[i]);
            }
            
            stopwatch.Stop();

            Console.WriteLine("    ValidateWithForLoop1:                 " + FormatTimeSpan(stopwatch.Elapsed));

            stopwatch.Reset();
            stopwatch.Start();
            
            for (i = 0; i < hashes.Length; ++i)
            {
                validator.ValidateWithForLoop2(hashes[i]);
            }
            
            stopwatch.Stop();

            Console.WriteLine("    ValidateWithForLoop2:                 " + FormatTimeSpan(stopwatch.Elapsed));

            stopwatch.Reset();
            stopwatch.Start();
            
            for (i = 0; i < hashes.Length; ++i)
            {
                validator.ValidateWithBidirectionalForLoop(hashes[i]);
            }
            
            stopwatch.Stop();
                                   
            Console.WriteLine("    ValidateWithBidirectionalForLoop:     " + FormatTimeSpan(stopwatch.Elapsed));

            stopwatch.Reset();
            stopwatch.Start();

            for (i = 0; i < hashes.Length; ++i)
            {
                validator.ValidateWithForeachLoop1(hashes[i]);
            }
            
            stopwatch.Stop();

            Console.WriteLine("    ValidateWithForeachLoop1:             " + FormatTimeSpan(stopwatch.Elapsed));

            stopwatch.Reset();
            stopwatch.Start();

            for (i = 0; i < hashes.Length; ++i)
            {
                validator.ValidateWithForeachLoop2(hashes[i]);
            }
            
            stopwatch.Stop();

            Console.WriteLine("    ValidateWithForeachLoop2:             " + FormatTimeSpan(stopwatch.Elapsed));
        }

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            return String.Format("{0:00}:{1:00}:{2:00}.{3:000} ; {4}",
                timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds,
                timeSpan.Milliseconds, timeSpan.Ticks);
        }
    }
}
