namespace API.Services.Contracts
{
    public interface IEmailService
    {
        void SendOrderConfirmation(int customerId, int orderId);
    }
}
