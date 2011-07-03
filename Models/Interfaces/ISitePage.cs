using System.Collections.Generic;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Interfaces
{
	public interface ISitePage
	{
		List<NavigationItem> MainNavigation { get; }
		IList<NavigationItem> FooterNavigation { get; }
		string Title { get; }
	}
}
