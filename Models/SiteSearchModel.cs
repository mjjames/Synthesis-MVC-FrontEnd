using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
    public class SiteSearchModel : ISitePage
    {
        public List<NavigationItem> MainNavigation
        {
            get;
            internal set;
        }

        public IList<NavigationItem> FooterNavigation
        {
            get;
            internal set;
        }

        public IList<NavigationItem> BreadcrumbNavigation
        {
            get;
            internal set;
        }

        public string Title
        {
            get;
            internal set;
        }

        public string PageTitle
        {
            get;
            internal set;
        }

        public Site Site
        {
            get;
            internal set;
        }

        public IList<SiteSearchResultDto> Results { get; internal set; }
        public string SearchTerm { get; internal set; }
    }
}
