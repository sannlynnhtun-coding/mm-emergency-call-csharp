using System.Net;
using System.Net.Mail;

namespace MMEmergencyCall.Domain.Client.Features.Register;

public interface IEmailSender
{
    void SendOtp(string toEmail, string otp);
}

public class SmtpEmailSender : IEmailSender
{
    public void SendOtp(string toEmail, string otp)
    {
        using var mail = new MailMessage();
        mail.From = new MailAddress("mmemergencycall@gmail.com");
        mail.To.Add(toEmail);
        mail.Subject = "Email Verification OTP";
        mail.Body = $"Your OTP is: {otp}";
        mail.IsBodyHtml = false;

        using var smtp = new SmtpClient("smtp.gmail.com", 587);
        smtp.Credentials = new NetworkCredential("mmemergencycall@gmail.com", "xzrl ajmh aumn ixmw");
        smtp.EnableSsl = true;
        smtp.Send(mail);
    }
}
