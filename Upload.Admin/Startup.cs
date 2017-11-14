using Upload.Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Upload.Admin.Startup))]
namespace Upload.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //init();
        }

        private void init()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists("Administrators"))
                roleManager.Create(new IdentityRole("Administrators"));
            if (!roleManager.RoleExists("Users"))
                roleManager.Create(new IdentityRole("Users"));
            if (!roleManager.RoleExists("Guests"))
                roleManager.Create(new IdentityRole("Guests"));
            ApplicationUser user = new ApplicationUser();
            user.Email = "admin@admin.com";
            user.UserName = "superadmin";

            string pass = "Omega@111";
            var ckCreate = userManager.Create(user, pass);
            if(ckCreate.Succeeded)
                if (!userManager.IsInRole(user.Id, "Administrators"))
                    userManager.AddToRole(user.Id, "Administrators");
        }
    }
}
