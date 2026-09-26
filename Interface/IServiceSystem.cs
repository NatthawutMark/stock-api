namespace stock_api.Interfaces;

public interface ISystemService
{
    string GenGUID();
    List<string> GetGUIDList(int count = 10);
}