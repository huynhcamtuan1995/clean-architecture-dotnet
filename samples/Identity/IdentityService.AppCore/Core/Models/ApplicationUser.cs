using System;
using System.Runtime.Serialization;
using IdentityService.AppCore.Enums;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.AppCore.Core.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; protected set; } = RoleEnum.User;
        public bool IsEnabled { get; set; }

        public void UpdateRole(string role)
        {
            Role = role;
        }

        [IgnoreDataMember]
        public string FullName => $"{FirstName} {LastName}";
    }


}
