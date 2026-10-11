using static stock_api.Request.TransactionRequest;
using static stock_api.Response.AuthRes;

namespace stock_api.Interfaces;

public interface ISystemService
{
    string GenGUID();
    List<string> GetGUIDList(int count = 10);
    string? GetUserId();
    string? GetUsername();
    Task<List<Menus>?> GetMenuByUser();
    Task<List<Menus>?> GetMenuByUser(string userIdentifier);
    Task<List<Menus>?> GetMenuByUser(userLogin user);
    Task<List<TransactionMenuResponse>> GetTransactionMenuByUser();
    Task<List<TransactionMenuResponse>> GetTransactionMenuByUser(string? userIdentifier);
    Task<List<TransactionMenuResponse>> GetTransactionMenuByUser(userLogin user);
    Task<string> GetDocNumber(DocNumber req);
}