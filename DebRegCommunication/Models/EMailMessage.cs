using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;

namespace DebRegCommunication.Models {
    public class EMailMessage {
        public virtual List<string> To { get; set; }
        public virtual List<string> Cc { get; set; }
        public virtual List<string> Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string HTMLBody { get; set; }

        public EMailMessage() {
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();
        }

        public MailMessage ToMailMessage() {
            MailMessage message = new MailMessage();
            message.Subject = Subject;
            message.Body = Body;

            if (!string.IsNullOrEmpty(HTMLBody)) {
                ContentType mimeType = new ContentType("text/html");
                message.AlternateViews.Add(
                    AlternateView.CreateAlternateViewFromString(
                        HTMLBody,
                        mimeType)
                );
            }
            AddToMailAddressCollection(To, message.To);
            AddToMailAddressCollection(Cc, message.CC);
            AddToMailAddressCollection(Bcc, message.Bcc);

            return message;
        }

        private void AddToMailAddressCollection(IEnumerable<string> recipients, MailAddressCollection collection) {
            foreach (var item in recipients) {
                collection.Add(new MailAddress(item));
            }
        }

    }
}
