using System.Web.Mvc;

namespace Upload.Teechip.Areas.Upload
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
                name: "Upload_Teechip",
                url: "Teechip",
                defaults: new { controller = "Teechip", action = "Teechip", id = UrlParameter.Optional },
                namespaces: new[] { "Upload.Teechip.Areas.Upload.Controllers" }
            );
        }
    }
}