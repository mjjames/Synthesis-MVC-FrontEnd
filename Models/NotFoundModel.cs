using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
    class NotFoundModel : ISitePage
    {
        public List<NavigationItem> MainNavigation
        {
            get;
            set;
        }

        public IList<NavigationItem> FooterNavigation
        {
            get;
            set;
        }

        public string Title
        {
            get { return "404 Page Not Found"; }
        }

        public string PageTitle { get { return Title; } }

        public Site Site { get; internal set; }
    }
}
