using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
    public class AuthenticationController : Controller
    {
        private IAuthenticationService _authenticationService;
        private Site _site;
     
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _site = new Site();
        }

        public AuthenticationController()
            : this(new FormsAuthenticationBasedAuthenticationService())
        {
            
        }
        
        [HttpGet]
        public ActionResult Login()
        {
            if (!_authenticationService.RequiresAuthentication || _authenticationService.IsAuthenticated)
            {
                return Redirect(_site.UrlBase);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string password)
        {
            if (!_authenticationService.Authenticate(password))
            {
                ModelState.AddModelError("Password", "Invalid Password Provided");
                return View();
            }
            return Redirect(_site.UrlBase);

        }

        [HttpGet]
        public ActionResult Logout()
        {
            _authenticationService.RevokeAuthentication();
            return View();
        }

    }
}
