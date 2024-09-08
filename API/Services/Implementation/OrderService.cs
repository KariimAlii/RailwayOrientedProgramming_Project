using API.Models;
using API.Services.Contracts;

namespace API.Services.Implementation
{
    public class OrderService : IOrderService
    {
        public int Submit(string transactionId, List<LineItem> lineItems)
        {
            throw new NotImplementedException();
        }
    }
}
