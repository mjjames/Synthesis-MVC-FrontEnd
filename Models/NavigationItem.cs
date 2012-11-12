using System.Collections.Generic;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
	public class NavigationItem
	{
        public NavigationItem()
        {
            ChildPages = new List<NavigationItem>();
        }

		public int PageKey { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Url { get; set; }
		public string ImageUrl { get; set; }
		public List<NavigationItem> ChildPages { get; set; }

		public string CssClass { get; set; }
	}
}
