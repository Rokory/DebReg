using DebReg.Models.Security;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DebReg.Models {
    public class SMTPHostConfiguration {
        [HiddenInput]
        public Guid Id { get; set; }

        [MinLength(5)]
        [MaxLength(255)]
        public String Host { get; set; }

        public int Port { get; set; }

        public Boolean SSL { get; set; }

        [MaxLength(255)]
        public String Username { get; set; }

        [NotMapped]
        [MaxLength(255)]
        public String Password {
            get {
                CheckAESParameters();
                return Encryption.DecryptStringFromBytes_Aes(EncryptedPassword, AESKey, AESIV);
            }
            set {
                CheckAESParameters();
                EncryptedPassword = Encryption.EncryptStringToBytes_Aes(value, AESKey, AESIV);
            }
        }

        public byte[] EncryptedPassword { get; set; }

        private byte[] AESKey {
            get {
                string appsetting_aes_key = Environment.GetEnvironmentVariable("APPSETTING_AES_KEY");
                if (appsetting_aes_key != null) {
                    return Convert.FromBase64String(appsetting_aes_key);
                }
                else {
                    return null;
                }

            }
        }

        private byte[] AESIV {
            get {
                string appsetting_aes_iv = Environment.GetEnvironmentVariable("APPSETTING_AES_IV");
                if (appsetting_aes_iv != null) {
                    return Convert.FromBase64String(appsetting_aes_iv);
                }
                else {
                    return null;
                }
            }
        }
        [MaxLength(255)]
        public String FromAddress { get; set; }

        public DateTime? LastSMTPOperationDateTime { get; set; }

        [MaxLength(1500)]
        public String LastSMTPError { get; set; }

        private void CheckAESParameters() {
            if (AESKey == null) {
                throw new ArgumentNullException("APPSETTING_AES_KEY");
            }
            if (AESIV == null) {
                throw new ArgumentNullException("APPSETTING_AES_IV");
            }
        }
    }
}
