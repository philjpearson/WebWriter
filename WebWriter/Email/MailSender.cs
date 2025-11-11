//
//	Last mod:	11 November 2025 16:40:53
//
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace WebWriter.Email;

internal class MailSender
	{
	private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

	private static string appPassword = "spoz mqub qguu krps";
	private static string smtpServer = "smtp.gmail.com";
	private static int port = 587;
	private static string fromAddress = "staffordchristadelphians@gmail.com";
	private static string[] toAddresses = ["phil@realworldsoftware.co.uk", "philjpearson@outlook.com"];
	private static System.Collections.Specialized.StringCollection emailRecipients = [];

	public static MimeMessage CreateTestEmail()
		{
		return CreateEmail("Test email", "This is a test email from WebWriter.");
		}

	public static MimeMessage CreateProgrammeUpdateEmail()
		{
		var message = new MimeMessage();

		try
			{
			emailRecipients = Properties.Settings.Default.EmailRecipients;
			if (emailRecipients.Count == 0)
				{
				logger.Warn("No email recipients found in settings, not sending email");
				}
			message.From.Add(new MailboxAddress("Stafford Christadelphians", fromAddress));
			message.To.Add(MailboxAddress.Parse(fromAddress));
			foreach (var addr in emailRecipients)
				{
				if (addr is not null)
					{
					var bits = addr.Split('|');
					if (bits.Length == 2 && bits[1] is not null)
						message.Bcc.Add(MailboxAddress.Parse(bits[1]));
					}
				}
			message.Subject = "Stafford Ecclesial programme updated";
			var builder = new BodyBuilder
				{
				// Set the plain-text version of the message text
				TextBody = @$"Dear Brother or Sister,

This is an automated email to let you know that the Stafford Christadelphian Ecclesial Programme has just been updated on the website.
See https://staffordchristadelphians.org.uk/programme for the latest version. It is dated {DateTime.Now:d MMMM yyyy}.

Please don't reply to this email.
",

				HtmlBody = string.Format($@"<p>Dear Brother or Sister,<br></p>
<p>This is an automated email to let you know that the Stafford Christadelphian Ecclesial Programme has just been updated on the website.</p>
<p>See <a href=""https://staffordchristadelphians.org.uk/programme"">https://staffordchristadelphians.org.uk/programme</a> for the latest version. It is dated <b>{DateTime.Now:d MMMM yyyy}</b>.</p>
<p>Please don't reply to this email.</p>"
	)
				};
			message.Body = builder.ToMessageBody();
			}
		catch (Exception ex)
			{
			logger.Error(ex, "Error creating programme update email");
			}
		return message;
		}

	private static MimeMessage CreateEmail(string subject, string body)
		{
		var message = new MimeMessage();
		message.From.Add(new MailboxAddress("Stafford Christadelphians", fromAddress));
		message.To.Add(MailboxAddress.Parse(fromAddress));
		foreach (var addr in toAddresses)
			{
			message.Bcc.Add(MailboxAddress.Parse(addr));
			}
		message.Subject = subject;
		message.Body = new TextPart("plain")
			{
			Text = body
			};
		return message;
		}

	public static async Task<bool> SendMailAsync(MimeMessage msg)
		{
		try
			{
			using var smtp = new SmtpClient();
			await smtp.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
			await smtp.AuthenticateAsync(fromAddress, appPassword);
			await smtp.SendAsync(msg);
			await smtp.DisconnectAsync(true);
			}
		catch (Exception ex)
			{
			logger.Error(ex, "Error sending email");
			return false;
			}
		return true;
		}
	}
