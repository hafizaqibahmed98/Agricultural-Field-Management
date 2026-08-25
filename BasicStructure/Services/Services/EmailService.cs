using MailKit.Net.Smtp;
using MimeKit;
using Services.Models;

namespace Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailService(EmailConfiguration emailConfig) => _emailConfig = emailConfig;
        public void SendEmail(Message message)
        {
            ValidateConfiguration();
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private void ValidateConfiguration()
        {
            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(_emailConfig.Host)) missing.Add("Smtp:Host");
            if (_emailConfig.Port is < 1 or > 65535) missing.Add("Smtp:Port");
            if (string.IsNullOrWhiteSpace(_emailConfig.Username)) missing.Add("Smtp:Username");
            if (string.IsNullOrWhiteSpace(_emailConfig.Password)) missing.Add("Smtp:Password");

            if (missing.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Email cannot be sent because required SMTP configuration is missing or invalid: {string.Join(", ", missing)}. " +
                    "Set these values with environment variables or .NET User Secrets.");
            }
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From ?? _emailConfig.Username));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.Host, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.Username, _emailConfig.Password);

                client.Send(mailMessage);
            }
            catch
            {
                //log an error message or throw an exception or both.
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
