using EXE_FAIEnglishTutor.Helpers;
using EXE_FAIEnglishTutor.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace EXE_FAIEnglishTutor.Mail
{
    public class EmailSendVetification
    {
        private readonly IRazorViewToStringRenderer _renderer;
        private readonly EmailSettings _emailSettings;

        public EmailSendVetification(IRazorViewToStringRenderer renderer, IOptions<EmailSettings> emailSettings)
        {
            _renderer = renderer;
            _emailSettings = emailSettings.Value;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string token)
        {
            try
            {
                string verificationLink = "http://localhost:5037/Account/ConfirmVerificationToken?token=" + token;
                // Render view thành HTML string
                string body;
                try
                {
                    // Kiểm tra xem có render được email không
                    body = await _renderer.RenderViewToStringAsync("Emails/VerificationEmail", verificationLink);
                    Console.WriteLine("✅ Rendered Email Body: " + body);
                }
                catch (Exception renderEx)
                {
                    Console.WriteLine($"❌ Lỗi render email: {renderEx.Message}");
                    throw new Exception("Lỗi khi render email. Vui lòng kiểm tra Razor View.");
                }

                string subject = "Xác thực email của bạn - FAI English";

                using (var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort))
                {
                    client.Credentials = new NetworkCredential(_emailSettings.FromEmail, _emailSettings.FromPassword);
                    client.EnableSsl = true;

                    MailMessage mail = new MailMessage
                    {
                        From = new MailAddress(_emailSettings.FromEmail),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mail.To.Add(toEmail);

                    await client.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                throw new Exception("Email gửi thất bại");
            }
        }


        public async Task TestSendEmail(string toEmail)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new NetworkCredential("anhnthe172115@fpt.edu.vn", "wfnk uwfc llxo yazo");
                    client.EnableSsl = true;

                    MailMessage mail = new MailMessage
                    {
                        From = new MailAddress("anhnthe172115@fpt.edu.vn"),
                        Subject = "Test Email",
                        Body = "Đây là email test!",
                        IsBodyHtml = false
                    };
                    mail.To.Add(toEmail);

                    await client.SendMailAsync(mail);
                    Console.WriteLine("✅ Email sent successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi gửi email: {ex.Message}");
            }
        }

    }
}
