using FinalProjectFiruz.Models;

namespace FinalProjectFiruz.Abstraction
{
    public interface IBasketService
    {
        Task<List<BasketItem>> GetBasketItemsAsync();
    }
}
