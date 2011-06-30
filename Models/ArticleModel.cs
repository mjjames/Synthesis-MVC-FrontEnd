using System.Collections.Generic;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
	public class ArticleModel : ArticleDTO
	{
		public List<NavigationItem> MainNavigation { get; set; }
		public IList<NavigationItem> FooterNavigation { get; set; }
		public IList<ArticleDTO> Articles { get; set; }
	}
}
