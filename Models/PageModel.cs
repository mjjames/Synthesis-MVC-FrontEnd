using System.Collections.Generic;
using mjjames.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using System.Collections.ObjectModel;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
	public class PageModel : PageDTO
	{
		public IList<NavigationItem> MainNavigation { get; set; }
		public IList<NavigationItem> FooterNavigation { get; set; }
	}
}