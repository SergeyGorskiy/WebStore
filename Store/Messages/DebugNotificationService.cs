using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Store.Messages
{
    public class DebugNotificationService : INotificationService
    {
        public void SendConfirmationCode(string cellPhone, int code)
        {
            Debug.WriteLine("Cell phone: {0}, code: {1:0000}.", cellPhone, code);
        }

        public Task SendConfirmationAsync(string cellPhone, int code)
        {
            Debug.WriteLine("Cell phone: {0}, code: {1:0000}.", cellPhone, code);
            return Task.CompletedTask;
        }

        public void StartProcess(Order order)
        {
            using (var client = new SmtpClient())
            {
                var message = new MailMessage("from@at.my.domain", "to@at.my.domain");
                message.Subject = "Заказ #" + order.Id;

                var builder = new StringBuilder();
                foreach (var item in order.Items)
                {
                    builder.Append($"{item.BookId}, {item.Count}");
                    builder.AppendLine();
                }
                message.Body = builder.ToString();
                client.Send(message);
            }
        }

        public Task StartProcessAsync(Order order)
        {
            StartProcess(order);
            return Task.CompletedTask;
        }
    }
}