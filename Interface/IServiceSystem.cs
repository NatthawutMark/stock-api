namespace stock_api.Interfaces;

public interface ISystemService
{
    string GenGUID();
    List<string> GetGUIDList(int count = 10);
    string? GetUserId();
    string? GetUsername();
}