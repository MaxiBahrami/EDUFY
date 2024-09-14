using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Policy;
using MimeKit;
using Azure.Core;
using NodaTime;
using InsightAcademy.InfrastructureLayer.Helper;

namespace InsightAcademy.ApplicationLayer.Services
{
    public class EmailService : IEmailSender, IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ITutorService _tutorService;
        private readonly IWebHostEnvironment _webEnvironment;
        private const string WelcomeEmailePath = @"EmailTemplate/Welcome.html";
        private const string TutorVarifyEmailePath = @"EmailTemplate/TutorVarify.html";
        private const string ResetPasswordPath = @"EmailTemplate/ResetPassword.html";
        private const string StudentBookingConfirmation = @"EmailTemplate/StudentBookingEmail.html";
        private const string TutorBookingConfirmation = @"EmailTemplate/TutorBookingConfirmation.html";

        public EmailService(IConfiguration configuration, IWebHostEnvironment webEnvironment, IUserService userService, ITutorService tutorService)
        {
            _config = configuration;
            _webEnvironment = webEnvironment;
            _userService = userService;
            _tutorService = tutorService;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var host = _config["SMTPConfig:Host"];
            var port = Convert.ToInt32(_config["SMTPConfig:Port"]);
            var username = _config["SMTPConfig:UserName"];
            var password = _config["SMTPConfig:Password"];
            var displayName = _config["SMTPConfig:SenderDisplayName"];

            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(displayName, username));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = "";

                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = htmlMessage
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(host, port, true);
                    client.Authenticate(username, password);
                    client.Send(emailMessage);
                    client.Disconnect(true);
                }
            }
            catch (Exception e)
            {
                //_logger.LogError(e.Message);
            }
        }

        public async Task SendWelcomeEmail(string fName, string toEmail, string text)
        {
            var host = _config["SMTPConfig:Host"];
            var port = Convert.ToInt32(_config["SMTPConfig:Port"]);
            var username = _config["SMTPConfig:UserName"];
            var password = _config["SMTPConfig:Password"];
            var cc = _config["SMTPConfig:CCEmail"];
            var displayName = _config["SMTPConfig:SenderDisplayName"];
            try
            {
                var filepath = Path.Combine(_webEnvironment.WebRootPath, WelcomeEmailePath);

                string html = File.ReadAllText(filepath);
                html = html.Replace("{text}", text);

                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(displayName, username));
                emailMessage.To.Add(new MailboxAddress("", toEmail));
                emailMessage.Subject = "Welcome to Our Platform!";

                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = html
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(host, port, true);
                    client.Authenticate(username, password);
                    client.Send(emailMessage);
                    client.Disconnect(true);
                }

                //var client = new SmtpClient(host, port)
                //{
                //    Credentials = new NetworkCredential(username, password),
                //    //EnableSsl = true,
                //    //UseDefaultCredentials = true,
                //    DeliveryMethod = SmtpDeliveryMethod.Network,
                //};

                //var mailMessage = new MailMessage
                //{
                //    From = new MailAddress(username, displayName),
                //    To = { new MailAddress(toEmail) },
                //    Subject = "Welcome to Our Platform!",
                //    Body = html,
                //    IsBodyHtml = true,
                //};
                //if (!string.IsNullOrEmpty(cc))
                //{
                //    mailMessage.CC.Add(cc);
                //}
                //await client.SendMailAsync(mailMessage);
            }
            catch (Exception e)
            {
            }
        }

        public async Task SendTutorVarifyEmail(string fName, string toEmail)
        {
            var host = _config["SMTPConfig:Host"];
            var port = Convert.ToInt32(_config["SMTPConfig:Port"]);
            var username = _config["SMTPConfig:UserName"];
            var password = _config["SMTPConfig:Password"];
            var cc = _config["SMTPConfig:CCEmail"];
            var displayName = _config["SMTPConfig:SenderDisplayName"];
            try
            {
                var filepath = Path.Combine(_webEnvironment.WebRootPath, TutorVarifyEmailePath);

                string html = File.ReadAllText(filepath);

                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(displayName, username));
                emailMessage.To.Add(new MailboxAddress("", toEmail));
                emailMessage.Subject = "Conngratulations!You are varified now";

                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = html
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(host, port, true);
                    client.Authenticate(username, password);
                    client.Send(emailMessage);
                    client.Disconnect(true);
                }
            }
            catch (Exception e)
            {
            }
        }

        public async Task ResetConfrimationEmail(string toEmails, string URL)
        {
            var host = _config["SMTPConfig:Host"];
            var port = Convert.ToInt32(_config["SMTPConfig:Port"]);
            var username = _config["SMTPConfig:UserName"];
            var password = _config["SMTPConfig:Password"];
            var cc = _config["SMTPConfig:CCEmail"];
            var displayName = _config["SMTPConfig:SenderDisplayName"];

            try
            {
                var filepath = Path.Combine(_webEnvironment.WebRootPath, ResetPasswordPath);

                string html = File.ReadAllText(filepath);
                html = html.Replace("{URL}", URL);

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(displayName, username));
                email.To.Add(new MailboxAddress("", toEmails));
                email.Subject = "Reset yout password";

                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = html
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(host, port, true);
                    client.Authenticate(username, password);
                    client.Send(email);
                    client.Disconnect(true);
                }
                //var filepath = Path.Combine(_webEnvironment.WebRootPath, ResetPasswordPath);

                //string html = File.ReadAllText(filepath);
                //html = html.Replace("{URL}", URL);
                //// Include Order Items in the email template

                //var client = new SmtpClient(host, port)
                //{
                //    Credentials = new NetworkCredential(username, password),
                //    EnableSsl = true,
                //    //UseDefaultCredentials = true,
                //    DeliveryMethod = SmtpDeliveryMethod.Network,
                //};

                //var mailMessage = new MailMessage
                //{
                //    From = new MailAddress(username, displayName),
                //    To = { new MailAddress(toEmails) },
                //    Subject = "Reset Your Password",
                //    Body = html,
                //    IsBodyHtml = true,
                //};
                //if (!string.IsNullOrEmpty(cc))
                //{
                //    mailMessage.CC.Add(cc);
                //}
                //await client.SendMailAsync(mailMessage);
            }
            catch (Exception e)
            {
            }
        }

        public async Task NotifyMessageSend(string toEmail, string name, string image, string message)
        {
            var host = _config["SMTPConfig:Host"];
            var port = Convert.ToInt32(_config["SMTPConfig:Port"]);
            var username = _config["SMTPConfig:UserName"];
            var password = _config["SMTPConfig:Password"];
            var cc = _config["SMTPConfig:CCEmail"];
            var displayName = _config["SMTPConfig:SenderDisplayName"];
            try
            {
                //var filepath = Path.Combine(_webEnvironment.WebRootPath, ResetPasswordPath);

                //string html = File.ReadAllText(filepath);
                //html = html.Replace("{URL}", URL);
                // Include Order Items in the email template

                var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                    //UseDefaultCredentials = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(username, displayName),
                    To = { new MailAddress(toEmail) },
                    Subject = $"New message from {name}",
                    Body = message,
                    IsBodyHtml = false,
                };
                if (!string.IsNullOrEmpty(cc))
                {
                    mailMessage.CC.Add(cc);
                }
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception e)
            {
            }
        }

        public async Task SendStudentBookingConfirmationEmail(Guid tutorId, string studentId, string bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            var host = _config["SMTPConfig:Host"];
            var port = Convert.ToInt32(_config["SMTPConfig:Port"]);
            var username = _config["SMTPConfig:UserName"];
            var password = _config["SMTPConfig:Password"];
            var cc = _config["SMTPConfig:CCEmail"];
            var displayName = _config["SMTPConfig:SenderDisplayName"];

            var user = await _userService.GetUserDetailById(studentId);
            var tutor = await _tutorService.GetTutor(tutorId);
            try
            {
                var filepath = Path.Combine(_webEnvironment.WebRootPath, StudentBookingConfirmation);

                string html = File.ReadAllText(filepath);
                html = html.Replace("[StudentName]", user.FullName);
                html = html.Replace("[BookingDate]", bookingDate);
                html = html.Replace("[StartTime]", startTime.ToString());
                html = html.Replace("[EndTime]", endTime.ToString());
                html = html.Replace("[TutorName]", tutor.FullName);
                html = html.Replace("[JoinLink]", tutor.GoogleMeetLink);

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(displayName, username));
                email.To.Add(new MailboxAddress("", user.Email));
                email.Subject = "Booking Confirmation";

                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = html
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(host, port, true);
                    client.Authenticate(username, password);
                    client.Send(email);
                    client.Disconnect(true);
                }
            }
            catch (Exception e)
            {
            }
        }

        public Task NotifyMessageSend(string toEmail, string studentId, string name, string image, string message)
        {
            throw new NotImplementedException();
        }

        public async Task SendTutorBookingConfirmationEmail(Guid tutorId, string studentId, string bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            var host = _config["SMTPConfig:Host"];
            var port = Convert.ToInt32(_config["SMTPConfig:Port"]);
            var username = _config["SMTPConfig:UserName"];
            var password = _config["SMTPConfig:Password"];
            var cc = _config["SMTPConfig:CCEmail"];
            var displayName = _config["SMTPConfig:SenderDisplayName"];

            var user = await _userService.GetUserDetailById(studentId);
            var tutor = await _tutorService.GetTutor(tutorId);
            var tutorUser = await _userService.GetUserDetailById(tutor.ApplicationUserId);

            var startTimeSpan = startTime;
            var endTimeSpan = endTime;
            try
            {
                // Get the time zones for the tutor and the student
                var tutorTimeZone = DateTimeZoneProviders.Tzdb[tutorUser.TimeZone];
                var studentTimeZone = DateTimeZoneProviders.Tzdb[user.TimeZone];

                // Create the student's local time using the provided TimeSpan (assumed to be in student's time zone)
                var studentStartTime = new LocalDateTime(2024, 9, 5, startTimeSpan.Hours, startTimeSpan.Minutes);
                var studentEndTime = new LocalDateTime(2024, 9, 5, endTimeSpan.Hours, endTimeSpan.Minutes);

                // Convert student's local time to a ZonedDateTime using the student's time zone
                var studentStartTimeZoned = studentTimeZone.AtLeniently(studentStartTime);
                var studentEndTimeZoned = studentTimeZone.AtLeniently(studentEndTime);

                // Convert the student's local time (ZonedDateTime) to UTC
                var studentStartTimeUTC = studentStartTimeZoned.ToInstant();
                var studentEndTimeUTC = studentEndTimeZoned.ToInstant();

                // Convert UTC time to the tutor's local time zone
                var tutorStartTimeLocal = studentStartTimeUTC.InZone(tutorTimeZone);
                var tutorEndTimeLocal = studentEndTimeUTC.InZone(tutorTimeZone);

                // Convert the tutor's local time back to TimeSpan for display or further calculations
                string tutorFormattedStartTime = tutorStartTimeLocal.ToString("hh:mm tt", null); // "tt" adds AM/PM
                string tutorFormattedEndTime = tutorEndTimeLocal.ToString("hh:mm tt", null); // "tt" adds AM/PM

                var filepath = Path.Combine(_webEnvironment.WebRootPath, TutorBookingConfirmation);

                string html = File.ReadAllText(filepath);
                html = html.Replace("[StudentName]", user.FullName);
                html = html.Replace("[BookingDate]", bookingDate);
                html = html.Replace("[StartTime]", $"{tutorFormattedStartTime}");
                html = html.Replace("[EndTime]", $"{tutorFormattedEndTime}");
                html = html.Replace("[TutorName]", tutor.FullName);
                html = html.Replace("[JoinLink]", "");

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(displayName, username));
                email.To.Add(new MailboxAddress("", tutorUser.Email));
                email.Subject = "Booking Confirmation";

                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = html
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(host, port, true);
                    client.Authenticate(username, password);
                    client.Send(email);
                    client.Disconnect(true);
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}