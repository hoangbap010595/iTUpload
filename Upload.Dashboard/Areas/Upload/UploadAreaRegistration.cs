using System.Web.Mvc;

namespace Upload.Dashboard.Areas.Upload
{
    public class UploadAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Upload";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: "Upload_Dashboard",
                url: "Upload/Dashboard/{action}/{id}",
                defaults: new { controller= "Dashboard", action = "Dashboard", id = UrlParameter.Optional },
                namespaces: new[] { "Upload.Dashboard.Areas.Upload.Controllers" }

            );
        }
    }
}