using DebRegCommunication.Models;
using System.Net.Mail;

namespace DebRegCommunication
{
    public interface IEMailService
    {
        string Host { get; set; }
        int Port { get; set; }
        bool SSL { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string FromAddress { get; set; }
        void Send(EMailMessage message, object userToken = null);

        event SendCompletedEventHandler SendCompleted;
    }
}
