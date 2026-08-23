using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace GreenManager_Web.Services
{
    /// <summary>
    /// Service voor het versturen van e-mails via MailKit
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();
            
            // Gegevens ophalen uit configuratie + backup gegevens indien er informatie ontbreekt
            string senderName = _config["EmailSettings:SenderName"] ?? "GreenManager";
            string senderEmail = _config["EmailSettings:SenderEmail"] ?? "test@test.com";
            string mailServer = _config["EmailSettings:MailServer"] ?? "";
            int mailPort = int.Parse(_config["EmailSettings:MailPort"] ?? "2525");
            
            // Haal de gebruikersnaam op (voor Mailtrap.io), of val terug op het e-mailadres
            string senderUsername = _config["EmailSettings:SenderUsername"] ?? senderEmail;
            
            // Gebruik het wachtwoord (uit usersecrets)
            string senderPassword = _config["EmailSettings:SenderPassword"] ?? ""; 

            emailMessage.From.Add(new MailboxAddress(senderName, senderEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            
            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            try
            {
				using (var client = new SmtpClient())
				{

					// Verbinden en mail versturen via MailKit
					await client.ConnectAsync(mailServer, mailPort, SecureSocketOptions.StartTls);

					// Hier loggen we in met de unieke Mailtrap Username in plaats van het e-mailadres
					await client.AuthenticateAsync(senderUsername, senderPassword);

					await client.SendAsync(emailMessage);
					await client.DisconnectAsync(true);
				}
			} catch (Exception ex)
            {
                // Belandt hier in geval dat bv. een van de gegevens niet kloppen
				_logger.LogError(ex, $"Fout bij het versturen van e-mail naar {email}");
			}
            
        }
    }
}