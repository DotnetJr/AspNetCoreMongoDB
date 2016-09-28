using System.Threading.Tasks;

namespace M101DotNet.WebApp.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
