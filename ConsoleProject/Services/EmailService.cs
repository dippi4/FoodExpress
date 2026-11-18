using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace ConsoleProject.Services
{
	internal class EmailService : IEmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void SendEmail(string toEmail, string subject, string message)
		{
			string server = GetRequiredSetting("Server");
			string senderEmail = GetRequiredSetting("SenderEmail");
			string password = GetRequiredSetting("Password").Replace(" ", "");
			string senderName = _configuration["EmailSettings:SenderName"] ?? "FoodExpress";

			if (!int.TryParse(_configuration["EmailSettings:Port"], out int port))
				throw new InvalidOperationException("EmailSettings:Port must be a number.");

			if (!IsValidEmail(senderEmail))
				throw new InvalidOperationException("EmailSettings:SenderEmail must be a valid Gmail address.");

			if (!IsValidEmail(toEmail))
				throw new InvalidOperationException("The recipient email address is invalid.");

			using var smtp = new SmtpClient(server, port);
			smtp.UseDefaultCredentials = false;
			smtp.EnableSsl = true;
			smtp.Credentials = new NetworkCredential(senderEmail, password);
			smtp.Timeout = 10000;

			using var email = new MailMessage();
			email.From = new MailAddress(senderEmail, senderName);
			email.To.Add(toEmail);
			email.Subject = subject;
			email.Body = message;
			email.IsBodyHtml = false;

			smtp.Send(email);
		}

		private string GetRequiredSetting(string name)
		{
			string value = _configuration[$"EmailSettings:{name}"] ?? "";
			if (string.IsNullOrWhiteSpace(value))
				throw new InvalidOperationException($"EmailSettings:{name} is missing in appsettings.json.");

			return value.Trim();
		}

		private static bool IsValidEmail(string email)
		{
			try
			{
				MailAddress address = new MailAddress(email);
				return address.Address == email;
			}
			catch (FormatException)
			{
				return false;
			}
		}
	}
}
