using System;
using System.Text;

namespace DebReg.Web.Infrastructure {
    public class PasswordHelper : IPasswordHelper {
        static string validLetters = "abcdefghkmnpqrstwxyz";
        static string validNumbers = "123456789";
        static Random random = new Random((int)(DateTime.UtcNow.Ticks & 0xFFFFFFFF));

        public string GeneratePassword() {
            StringBuilder password = new StringBuilder();
            // First letter upperc ase
            password.Append(validLetters.Substring(random.Next(validLetters.Length), 1).ToUpper());

            // Next 3 letters lower case
            for (int i = 0; i < 3; i++) {
                password.Append(validLetters.Substring(random.Next(validLetters.Length), 1));
            }

            // Next 4 characters numbers

            for (int i = 0; i < 4; i++) {
                password.Append(validNumbers.Substring(random.Next(validNumbers.Length), 1));
            }

            return password.ToString();

        }
    }
}