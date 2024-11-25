namespace ElProApp.Web.Areas.Admin.Models
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public IEnumerable<string> RoleToRemove { get; set; } = new List<string>(); 
       
    }

}
