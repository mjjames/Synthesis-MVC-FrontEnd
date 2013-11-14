using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
    public class ProjectModel : ProjectDto , ISitePage
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

        public string PageTitle { get { return Title; } }
    }
}
