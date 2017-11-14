using System.Web.Mvc;

namespace PartialView.Areas.PartialView
{
    public class PartialViewAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PartialView";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
               name: "PartialView_PartialView",
               url: "PartialView/PartialView/{action}/{id}",
               defaults: new { controller = "PartialView", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "PartialView.Areas.PartialView.Controllers" }
            );
        }
    }
}