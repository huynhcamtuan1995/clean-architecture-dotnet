using System;
using System.Runtime.Serialization;
using IdentityService.AppCore.Enums;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.AppCore.Core.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
    }


}
