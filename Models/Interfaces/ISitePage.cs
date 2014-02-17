using System.Collections.Generic;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Interfaces
{
	public interface ISitePage
	{
		List<NavigationItem> MainNavigation { get; }
		IList<NavigationItem> FooterNavigation { get; }
		/// <summary>
		/// The title that should be used for headings
		/// </summary>
        string Title { get; }

        /// <summary>
        /// The Title that should be used for SEO
        /// </summary>
        string PageTitle { get; }

        Site Site { get; }
	}
}
