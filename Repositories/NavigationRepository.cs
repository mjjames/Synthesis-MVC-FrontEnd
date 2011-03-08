using System.Collections.Generic;
using System.Linq;
using System.Web;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.DataEntities;
using System;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories
{
	public class NavigationRepository
	{
		private PageRepository _pageRepository = new PageRepository();

		internal IQueryable<NavigationItem> GetMainNavigation()
		{
			var navItems = new List<NavigationItem>();
			//we build the navigation by first pulling out the home page from the page repository
			var homePage = _pageRepository.Get("HOME"); //use the home page id
			//if we haven't got a home page something is badly wrong
			if (homePage == null)
			{
				throw new Exception("No Home Page Found - There must be a page with an id of HOME");
			}
			//use the home page to add our first nav item
			var homeItem = new NavigationItem
							{
								CssClass = "navItem home",
								Description = "",
								PageKey = homePage.page_key,
								Title = "Home",
								Url = "/" //note the home page always has a url of /
							};
			navItems.Add(homeItem);

			//we now need to call the recursive build page tree method, this takes a parent item and the child items to add
			//this then for each child page calls it self - thus recursively builds the tree
			//note we filter the child pages to active and on main nav
			navItems.AddRange(BuildPageTree("", homePage.Pages.Where(p => p.active.HasValue && p.active.Value && p.showinnav.HasValue && p.showinnav.Value)));

			return navItems.AsQueryable();
		}

		/// <summary>
		/// Builds the Navigation Item tree ensuring that each item has it's child pages populated
		/// </summary>
		/// <param name="parentUrl">parent urlof each child</param>
		/// <param name="pages">child pages to turn into navigation items</param>
		/// <returns>Enumerable collection of Navigation Items</returns>
		private IEnumerable<NavigationItem> BuildPageTree(string parentUrl, IEnumerable<Page> pages)
		{
			var childPages = (from p in pages
							  select new NavigationItem
							  {
								  Title = p.navtitle,
								  Description = p.metadescription,
								  PageKey = p.page_key,
								  Url = parentUrl + "/" + p.page_url,
								  CssClass = GetCssClass(parentUrl + "/" + p.page_url),
								  ChildPages = GetChildPages(p, parentUrl + "/" + p.page_url),
								  ImageUrl = p.thumbnailimage
							  });
			return childPages;
		}

		/// <summary>
		/// Look at the current page and our page url and see if we need to load the child pages
		/// </summary>
		/// <param name="p">page</param>
		/// <param name="url">url to check against</param>
		/// <returns></returns>
		private List<NavigationItem> GetChildPages(Page p, string url)
		{
			var childPages = new List<NavigationItem>();
			//simply if our url starts with the url provided then load the child pages
			if (HttpContext.Current.Request.Url.AbsolutePath.StartsWith(url))
			{
				childPages.AddRange(BuildPageTree(url, p.Pages.Where(cp => cp.active.HasValue && cp.active.Value && cp.showinnav.HasValue && cp.showinnav.Value)));
			}
			return childPages;
		}

		/// <summary>
		/// Calculates the CSS class for the provided page
		/// </summary>
		/// <param name="pageUrl">page url to use for current page match</param>
		/// <returns>the generated class</returns>
		private string GetCssClass(string pageUrl)
		{
			//always have navItem
			var cssClass = "navItem";
			//to work out if we are a child page see if we have more than 1 / in the url, one at the start and one after
			if (pageUrl.Select(c => c.Equals("/")).Count() > 1)
			{
				cssClass += " childNavItem";
			}

			//if we have an exact url match mark the page as current
			if (HttpContext.Current.Request.Url.AbsolutePath.Equals(pageUrl, System.StringComparison.InvariantCultureIgnoreCase))
			{
				cssClass += " current";
				//once we have current we don't need to check for selected so drop out
				return cssClass;
			}

			//if our url isn't an exact match but starts with the provided url then the item must be at least selected
			if (HttpContext.Current.Request.Url.AbsolutePath.StartsWith(pageUrl))
			{
				cssClass += " selected";
			}

			return cssClass;
		}

		internal IQueryable<NavigationItem> GetFooterNavigation()
		{
			var siteMap = SiteMap.Providers["footerNavigation"];
			if (siteMap == null || siteMap.RootNode == null)
			{
				return new List<NavigationItem>().AsQueryable();
			}
			return (from SiteMapNode navItem in siteMap.RootNode.GetAllNodes()
					where navItem["Visible"] != null && navItem["Visible"].Equals("1")
					select new NavigationItem
							 {
								 Title = navItem.Title,
								 CssClass = "footerItem navItem",
								 Description = navItem.Description,
								 ImageUrl = navItem["imageURL"],
								 PageKey = int.Parse(navItem.Key),
								 Url = navItem.Url
							 }).AsQueryable();
		}

		internal IQueryable<NavigationItem> GetHomePageNavigation()
		{
			var siteMap = SiteMap.Providers["homeNavigation"];
			if (siteMap == null || siteMap.RootNode == null)
			{
				return new List<NavigationItem>().AsQueryable();
			}
			return (from SiteMapNode navItem in siteMap.RootNode.GetAllNodes()
					select new NavigationItem
					{
						Title = navItem.Title,
						CssClass = "homenavItem navItem",
						Description = navItem.Description,
						PageKey = int.Parse(navItem.Key),
						ImageUrl = navItem["imageURL"],
						Url = navItem.Url
					}).AsQueryable();
		}

		internal IQueryable<NavigationItem> GetSiteMapNavigation()
		{
			var siteMap = SiteMap.Providers["siteMap"];
			if (siteMap == null || siteMap.RootNode == null)
			{
				return new List<NavigationItem>().AsQueryable();
			}
			return (from SiteMapNode navItem in siteMap.RootNode.ChildNodes
					where navItem["Visible"] != null && navItem["Visible"].Equals("1")
					select new NavigationItem
					{
						Title = navItem.Title,
						CssClass = "sitemapNavItem navItem",
						Description = navItem.Description,
						PageKey = int.Parse(navItem.Key),
						ImageUrl = navItem["imageURL"],
						Url = navItem.Url,
						ChildPages = navItem.ChildNodes.Cast<SiteMapNode>().Select(n => new NavigationItem
						{
							PageKey = int.Parse(n.Key),
							Title = n.Title,
							Description = n.Description,
							CssClass = "sitemapNavItem navItem",
							ImageUrl = n["imageURL"],
							Url = n.Url,
							ChildPages = n.ChildNodes.Cast<SiteMapNode>().Select(no => new NavigationItem
							{
								PageKey = int.Parse(no.Key),
								Title = no.Title,
								Description = no.Description,
								CssClass = "sitemapNavItem navItem",
								ImageUrl = no["imageURL"],
								Url = no.Url
							}).ToList()
						}).ToList()


					}).AsQueryable();
		}
	}
}
