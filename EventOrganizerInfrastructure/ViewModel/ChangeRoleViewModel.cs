using Microsoft.AspNetCore.Identity;
using EventOrganizerDomain.Model;
namespace EventOrganizerInfrastructure.ViewModel
{
    public class ChangeRoleViewModel
    {

            public string UserId { get; set; }
            public string UserEmail { get; set; }
            public List<Role> AllRoles { get; set; }
            public IList<string> UserRoles { get; set; }
            public ChangeRoleViewModel()
            {
                AllRoles = new List<Role>();
                UserRoles = new List<string>();
            }       
    }

}