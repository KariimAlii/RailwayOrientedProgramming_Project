namespace API.Services.Contracts
{
    public interface IPaymentService
    {
        string ChargeCreditCard(int creditCartId, int totalAmount);
    }
}
