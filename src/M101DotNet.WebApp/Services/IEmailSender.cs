using System.Threading.Tasks;

namespace M101DotNet.WebApp.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
