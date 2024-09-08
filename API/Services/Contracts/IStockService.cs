using API.Models;

namespace API.Services.Contracts
{
    public interface IStockService
    {
        void UpdateInventory(List<LineItem> lineItems);
        bool IsEnoughStock(List<LineItem> lineItems);

    }
}
