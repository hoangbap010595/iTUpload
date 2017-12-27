using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Upload.Admin.Models;

namespace Upload.Admin.Controllers
{
    public class GroupController : ApiController
    {
        //public JsonResult 
        ApplicationContext context = new ApplicationContext();

        private int getMemberOfGroup()
        {
            var x = from a in context.AppConnecteds
                    join u in context.UserUseApps on a.AppId equals u.AppID
                    join g in context.GroupUseApps on u.GroupID equals g.Id
                    select new
                    {
                        u,
                        a,
                        g
                    };
            return x.Count();
        }
    }
}
