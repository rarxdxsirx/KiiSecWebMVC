using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace KiiSecWebMVC.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "bluescreenangels@gmail.com";
            var pw = "tdzaobszxrfuwgwg";

            var client = new SmtpClient("smtp.gmail.com",587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, pw)
            };
            return client.SendMailAsync(new MailMessage(
                from: mail,
                to: email,
                subject,
                message));
        }
    }
}
