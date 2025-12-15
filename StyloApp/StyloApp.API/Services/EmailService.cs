using System.Net.Mail;
using System.Net;

namespace StyloApp.API.Services
{
    public class EmailService
    {
        public async Task SendOtpAsync(string toEmail, string otp)
        {
            var message = new MailMessage
            {
                From = new MailAddress("sbook7107@gmail.com", "Fashion Shop"),
                Subject = "Your verification code",
                Body = $"Your OTP code is: {otp}",
                IsBodyHtml = false
            };

            message.To.Add(toEmail);

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(
                    "sbook7107@gmail.com",
                    "kzid tqky kfyi zcyz"
                ),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}
