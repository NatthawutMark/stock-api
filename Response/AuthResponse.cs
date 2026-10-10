using System;
namespace stock_api.Response;

public class AuthRes
{
    #region AuthResponse
    public class userLogin
    {
        public string userid { get; set; }
        public string username { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string tel { get; set; }
        public string email { get; set; }
        public bool IsActive { get; set; }
    }
    public class loginResponse
    {
        public string username { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public roles? role { get; set; }
        public List<Menus>? Menus { get; set; }
    }

    public class RefreshtokenResponse
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string? ReplacedToken { get; set; }
    }

    #endregion

    public class roles
    {
        public string roleId { get; set; }
        public string roleTh { get; set; }
        public string roleEn { get; set; }
    }

    public class Menus
    {
        public string menuID { get; set; }
        public string? parentID { get; set; }
        public string nameTh { get; set; }
        public string nameEn { get; set; }
        public int? orderNo { get; set; }
        public string? menuType { get; set; }
        public string? icon { get; set; }
        public string? menuName => !string.IsNullOrEmpty(nameEn) ? $"{nameTh}({nameEn})" : nameTh;
        public string? url { get; set; }
        public bool isActive { get; set; }
        public List<Menus>? subMenus { get; set; }
    }

    public class TransactionMenuResponse
    {
        public string id { get; set; } = null!;
        public string? nameTh { get; set; }
        public string? nameEn { get; set; }
        public string? menuName { get; set; }
        public int? orderNo { get; set; }
    }
}