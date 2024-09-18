using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using ArooshyStore.BLL.Services;

namespace ArooshyStore.BLL.EmailService
{
    public class EmailServiceRepository
    {
        private static string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
        private static string fromPassword = ConfigurationManager.AppSettings["FromPassword"];
        private static string toEmail = ConfigurationManager.AppSettings["ToEmail"];
        private static string host = ConfigurationManager.AppSettings["Host"];
        private static int port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
        public static string SendEmailString(string toEmailAddress, string subject, string body)
        {
            string message = "";
            try
            {
                var senderEmail = new MailAddress(fromEmail, subject);
                var receiverEmail = new MailAddress(toEmailAddress, "Receiver");
                var smtp = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, fromPassword)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(mess);
                }
                message = "Success";
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "EmailServiceRepository", "SendEmailString");
            }
            return message;
        }
        public static void SendEmailVoid(string toEmailAddress, string subject, string body, string systemType)
        {
            try
            {
                var senderEmail = new MailAddress(fromEmail, subject);
                var receiverEmail = new MailAddress(toEmailAddress, "Receiver");
                var smtp = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, fromPassword)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(mess);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "EmailServiceRepository", "SendEmailVoid");
            }
        }
    }
}
