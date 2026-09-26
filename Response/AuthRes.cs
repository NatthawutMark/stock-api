using System;
namespace stock_api.Response;

public class AuthRes
{
    public class userLogin
    {
        public string userid { get; set; }
        public string username { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string tel { get; set; }
        public string email { get; set; }
        public bool IsActive { get; set; }
    }
    public class loginResponse
    {
        public string username { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public roles? role { get; set; }
        public List<Menus>? Menus { get; set; }
    }

    public class roles
    {
        public string roleID { get; set; }
        public string roleTh { get; set; }
        public string roleEn { get; set; }
    }

    public class Menus
    {
        public string menuID { get; set; }
        public string? parentID { get; set; }
        public string NameTh { get; set; }
        public string NameEn { get; set; }
        public List<Menus>? subMenus { get; set; }
    }
}