using System.Net.Mail;
using System.Net;
using Training.Models;
namespace Training.Send_Email {
    public static class SendEmail
    {
        public static bool SendEmailMessage(string to, string subject, string body, string attachFile)
        {
            try
            {
                MailMessage message = new MailMessage(ConstantHelper.emailSender, to, subject, body);
                message.IsBodyHtml = true;

                using (var client = new SmtpClient(ConstantHelper.hostEmail, ConstantHelper.port))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(ConstantHelper.emailSender, ConstantHelper.passwordSender);

                    if (!string.IsNullOrEmpty(attachFile))
                    {
                        Attachment attachment = new Attachment(attachFile);
                        message.Attachments.Add(attachment);
                    }

                    client.Send(message);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết lỗi
                Console.WriteLine($"Error sending email: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        public static bool SendAccountInfoEmail(string userEmail, string username, string password)
        {
            try
            {
                string subject = "Thông tin tài khoản của bạn";
                string body = $@"
                <h3>Thông tin tài khoản</h3>
                <p>Tài khoản của bạn đã được tạo thành công:</p>
                <p><strong>Tên đăng nhập:</strong> {username}</p>
                <p><strong>Mật khẩu:</strong> {password}</p>
                <p>Vui lòng đăng nhập và thay đổi mật khẩu ngay lập tức vì lý do bảo mật.</p>
                <p><em>Đây là email tự động, vui lòng không trả lời.</em></p>
            ";

                return SendEmailMessage(userEmail, subject, body, null);
            }
            catch
            {
                return false;
            }
        }
    }
}