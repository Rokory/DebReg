using System;
using System.Text;

namespace DebRegComponents.Helpers {
    internal class BaseHelper {

        private const string Clist20 = "34789ABCEFMNPRTUVWXY";
        private static readonly char[] Clistarr = Clist20.ToCharArray();

        public static ulong Base20Decode(string inputString) {
            ulong result = 0;
            var pow = 0;
            for (var i = inputString.Length - 1; i >= 0; i--) {
                var c = inputString[i];
                var pos = Clist20.IndexOf(c);
                if (pos > -1)
                    result += (uint)pos * (ulong)Math.Pow(Clist20.Length, pow);
                else
                    throw new ArgumentOutOfRangeException();
                pow++;
            }
            return result;
        }

        public static string Base20Encode(ulong inputNumber) {
            var sb = new StringBuilder();
            do {
                sb.Append(Clistarr[inputNumber % (ulong)Clist20.Length]);
                inputNumber /= (ulong)Clist20.Length;
            } while (inputNumber != 0);
            return Reverse(sb.ToString());
        }

        public static string Reverse(string s) {
            var charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}