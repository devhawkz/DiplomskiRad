namespace Server.Repository.EmailRespository;

public interface IEmail
{
    public Task SendEmailAsync(string toEmail, string subject, string message);
}
