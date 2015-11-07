using DebRegCommunication.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace DebRegCommunication
{
    public class SMTPEMail : IEMailService
    {
        private SmtpClient smtpClient = new SmtpClient();
        private Queue<MessageQueueItem> queue = new Queue<MessageQueueItem>();
        private bool smtpClientReady = true;

        #region IEMailService Members

        public event SendCompletedEventHandler SendCompleted;
        public string Host
        {
            get
            {
                return smtpClient.Host;
            }
            set
            {
                smtpClient.Host = value;
            }
        }

        public int Port
        {
            get
            {
                return smtpClient.Port;
            }
            set
            {
                smtpClient.Port = value;
            }
        }

        public bool SSL
        {
            get
            {
                return smtpClient.EnableSsl;
            }
            set
            {
                smtpClient.EnableSsl = value;
            }
        }

        private string _username;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                smtpClient.Credentials = new NetworkCredential(_username, _password);
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                smtpClient.Credentials = new NetworkCredential(_username, _password);
            }
        }

        public string FromAddress
        {
            get;
            set;
        }

        public void Send(EMailMessage message, object userToken = null)
        {

            // Build mail message
            MailMessage mailMessage = message.ToMailMessage();
            mailMessage.From = new MailAddress(FromAddress);


            MessageQueueItem item = new MessageQueueItem
            {
                Message = mailMessage,
                UserToken = userToken
            };

            // Enqueue message

            queue.Enqueue(item);

            // Initiate despooling queue
            ThreadPool.QueueUserWorkItem(SendNextQueueItem);
        }

        #endregion

        public SMTPEMail()
        {
            smtpClient.SendCompleted += smtpClient_SendCompleted;
        }

        void smtpClient_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // One message send, so spool next one
            smtpClientReady = true;

            // Callback
            var sendCompleted = SendCompleted;
            if (sendCompleted != null)
            {
                sendCompleted(sender, e);
            }

            SendNextQueueItem(null);

        }


        void SendNextQueueItem(object state)
        {

            // if smtpclient is not ready, this method is called from  smtpClient.SendCompleted
            if (smtpClientReady)
            {
                if (queue.Count > 0)
                {
                    // smtpclient is occupied now
                    smtpClientReady = false;

                    // Send next message from queue
                    var item = queue.Dequeue();
                    try
                    {

                        smtpClient.SendAsync(item.Message, item.UserToken);
                    }

                    // On errors callback
                    catch (SmtpException e)
                    {

                        //client is ready
                        // smtpClientReady = true;

                        var sendCompleted = SendCompleted;
                        if (sendCompleted != null)
                        {
                            AsyncCompletedEventArgs a = new AsyncCompletedEventArgs(e, false, item.UserToken);
                            sendCompleted(this, a);
                        }
                    }
                }
            }
        }

    }

    class MessageQueueItem
    {
        public MailMessage Message { get; set; }
        public object UserToken { get; set; }

    }
}
