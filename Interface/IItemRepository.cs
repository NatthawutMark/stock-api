
using stock_api.dbo;
using stock_api.Models;
using static stock_api.dbo.ItemDbo;
using static stock_api.Request.MastItemRequest;
namespace stock_api.Interfaces;

public interface IItemRepository
{
    Task<List<ItemList>> GetAll(reqFields obj);
}