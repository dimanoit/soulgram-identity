using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace soulgram.identity.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Hobbies { get; set; }
        public string ProfileImg { get; set; }
    }
}
