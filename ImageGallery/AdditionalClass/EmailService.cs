using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace ImageGallery.AdditionalClass
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();   //формирование письма

                emailMessage.From.Add(new MailboxAddress("Обратная связь", "updateimage@u1053028.plsk.regruhosting.ru"));
                emailMessage.To.Add(new MailboxAddress("", "i_e.v.antonov@mpt.ru"));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart("Plain")
                {
                    Text = "Почтовый адрес пользователя: " + email + " " + message
                };

                using (var client = new SmtpClient())   //подключение к почтовому клиенту
                {
                    await client.ConnectAsync("mail.hosting.reg.ru", 465, true);
                    await client.AuthenticateAsync("updateimage@u1053028.plsk.regruhosting.ru", "7f9c1a5e");
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
