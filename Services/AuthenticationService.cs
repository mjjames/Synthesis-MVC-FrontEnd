using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models
{
    public class SessionBasedAuthenticationService : IAuthenticationService
    {
        private Models.Site _site;
        private string _authenticationKey = "Site-Authenticated";
        public SessionBasedAuthenticationService()
        {
            _site = new mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Site();
        }

        public bool IsAuthenticated
        {
            get
            {
                var value = HttpContext.Current.Session[_authenticationKey];
                if (value == null)
                {
                    return false;
                }
                return (bool)value;
            }
        }

        public bool RequiresAuthentication
        {
            get
            {
                return !String.IsNullOrWhiteSpace(_site.Password) && !IsAuthenticated;
            }
        }

        public bool Authenticate(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || _site.Password != password)
            {
                return false;
            }
            HttpContext.Current.Session.Add(_authenticationKey, true);
            return true;
        }

        public void RevokeAuthentication()
        {
            HttpContext.Current.Session.Remove(_authenticationKey);
        }
    }

    public class FormsAuthenticationBasedAuthenticationService : IAuthenticationService
    {
        private Models.Site _site;
        public FormsAuthenticationBasedAuthenticationService()
        {
            _site = new mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Site();
        }

        public bool IsAuthenticated
        {
            get
            {
                return HttpContext.Current.Request.IsAuthenticated;
            }
        }

        public bool RequiresAuthentication
        {
            get
            {
                return !String.IsNullOrWhiteSpace(_site.Password) && !IsAuthenticated;
            }
        }

        public bool Authenticate(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || _site.Password != password)
            {
                return false;
            }
            //FormsAuthentication.RedirectFromLoginPage(Guid.NewGuid().ToString(), false);
            FormsAuthentication.SetAuthCookie(Guid.NewGuid().ToString(), false);
            return true;
        }

        public void RevokeAuthentication()
        {
            FormsAuthentication.SignOut();
        }
    }

    public interface IAuthenticationService
    {
        bool IsAuthenticated { get; }
        bool RequiresAuthentication { get; }
        bool Authenticate(string password);

        void RevokeAuthentication();
    }
}
