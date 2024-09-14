using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmail(string fname, string toEmail, string text);

        Task SendTutorVarifyEmail(string fname, string toEmail);

        Task ResetConfrimationEmail(string toEmail, string URL);

        Task NotifyMessageSend(string toEmail, string studentId, string name, string image, string message);

        Task SendStudentBookingConfirmationEmail(Guid tutorId, string studentId, string bookingDate, TimeSpan startTime, TimeSpan endTime);

        Task SendTutorBookingConfirmationEmail(Guid tutorId, string studentId, string bookingDate, TimeSpan startTime, TimeSpan endTime);
    }
}