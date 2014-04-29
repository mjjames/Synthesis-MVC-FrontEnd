using System.Collections.Generic;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Interfaces;
using System.Linq;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
	public class PageModel : PageDTO, ISitePage
	{
		public List<NavigationItem> MainNavigation { get; set; }
		public IList<NavigationItem> FooterNavigation { get; set; }
        public List<NavigationItem> ChildNavigation { get; set; }
        public List<NavigationItem> FeaturedChildNavigation { get; set; }
        public List<NavigationItem> SecondaryFeaturedChildNavigation { get; set; }
        public Site Site { get; internal set; }
	}
}