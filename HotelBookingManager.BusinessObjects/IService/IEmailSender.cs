using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.BusinessObjects.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body);
    }

    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var host = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:SmtpPort"]!);
            var from = _config["EmailSettings:SenderEmail"];
            var pass = _config["EmailSettings:SenderPassword"];

            using var client = new System.Net.Mail.SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(from, pass)
            };

            using var msg = new System.Net.Mail.MailMessage(from!, to, subject, body);
            msg.IsBodyHtml = false;

            await client.SendMailAsync(msg);
        }
    }

}
