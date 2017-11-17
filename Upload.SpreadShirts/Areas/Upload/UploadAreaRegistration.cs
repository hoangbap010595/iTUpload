using System.Web.Mvc;

namespace Upload.SpreadShirts.Areas.Upload
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
               name: "Upload_SpreadShirt",
               url: "Upload/SpreadShirt/{action}/{id}",
               defaults: new { action = "SpreadShirt", controller = "SpreadShirt", id = UrlParameter.Optional },
               namespaces: new[] {""}

            );
        }
    }
}