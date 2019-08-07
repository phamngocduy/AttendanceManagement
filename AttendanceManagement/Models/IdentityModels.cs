using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AttendanceManagement.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace AttendanceManagement.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
	public static class IdentityExtensions
	{
		public static string GetAvatar(this System.Security.Principal.IIdentity user)
		{
            try
            {
                APIController api = new APIController();
                string avatarData = api.ReadData("https://fitlogin.vanlanguni.edu.vn/GroupManagement/api/getUserImage?searchString=" + user.Name);
                AvatarBase64 ava = JsonConvert.DeserializeObject<AvatarBase64>(avatarData);
                return ava.Avatar;
            } catch (Exception)
            {
                return null;
            }
		}

	}

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}