using API.Models;

namespace API.Services.Contracts
{
    public interface IOrderService
    {
        int Submit(string transactionId, List<LineItem> lineItems);
    }
}
