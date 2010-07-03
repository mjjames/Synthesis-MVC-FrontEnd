using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mjjames.Models;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
	internal class SiteMapPageModel : PageModel
	{
		public IList<NavigationItem> SiteMapNavigation { get; set; }
	}
}
