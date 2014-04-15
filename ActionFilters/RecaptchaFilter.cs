using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.ActionFilters
{
    public class CaptchaValidatorAttribute : ActionFilterAttribute
    {
        private const string ChallengeFieldKey = "recaptcha_challenge_field";
        private const string ResponseFieldKey = "recaptcha_response_field";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var site = new Site();
            var captchaChallengeValue = filterContext.HttpContext.Request.Form[ChallengeFieldKey];
            var captchaResponseValue = filterContext.HttpContext.Request.Form[ResponseFieldKey];
            var captchaValidtor = new Recaptcha.RecaptchaValidator
            {
                PrivateKey = site.Setting("Recaptcha:PrivateKey"),
                RemoteIP = filterContext.HttpContext.Request.UserHostAddress,
                Challenge = captchaChallengeValue,
                Response = captchaResponseValue
            };

            var recaptchaResponse = captchaValidtor.Validate();

            // this will push the result value into a parameter in our Action  
            filterContext.ActionParameters["captchaValid"] = recaptchaResponse.IsValid;

            base.OnActionExecuting(filterContext);
        }
    }
}
