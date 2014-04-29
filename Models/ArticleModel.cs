using System.Collections.Generic;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Interfaces;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
	public class ArticleModel : ArticleDTO, ISitePage
	{
		public List<NavigationItem> MainNavigation { get; set; }
		public IList<NavigationItem> FooterNavigation { get; set; }
        public IList<NavigationItem> BreadcrumbNavigation { get; set; }
		public IList<ArticleDTO> Articles { get; set; }
        public Site Site { get; internal set; }
	}
}
