using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.ActionFilters
{
    public class PasswordProtectedSiteFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var auth = new SessionBasedAuthenticationService();
            var auth = new FormsAuthenticationBasedAuthenticationService();
            if (auth.RequiresAuthentication)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(
                    new
                    {
                        action = "Login",
                        controller = "Authentication"
                    }
                    ));
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
