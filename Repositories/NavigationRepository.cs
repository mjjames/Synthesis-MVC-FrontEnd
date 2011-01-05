﻿using System.Collections.Generic;
using System.Linq;
using System.Web;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories
{
	public class NavigationRepository
	{
		internal IQueryable<NavigationItem> GetMainNavigation()
		{
			//we build a main navigation by loading the navigation sitemap and then spinning through the top level and creating nav items from it
			//then we find the current page and pull out its children 
			var navItems = new List<NavigationItem>();

			//load our main navigation sitemap
			var siteMap = SiteMap.Providers["siteNavigation"];
			if (siteMap == null || siteMap.RootNode == null) //if null return an empty nav list
			{
				System.Diagnostics.Debug.WriteLine("No Site Mavigation SiteMap Found");
				return navItems.AsQueryable();
			}
			navItems.Add(new NavigationItem
							{
								CssClass = "navItem home",
								Description = "",
								PageKey = int.Parse(siteMap.RootNode.Key),
								Title = "Home",
								Url = siteMap.RootNode.Url
							});

			//if we have a current node use it's parent else use the root node
			var parentNode = SiteMap.CurrentNode != null && SiteMap.CurrentNode.ParentNode != null ? SiteMap.CurrentNode.ParentNode : SiteMap.RootNode;

			foreach (SiteMapNode node in siteMap.RootNode.ChildNodes)
			{
				//skip sitemapnodes that aren't visible
				if (node["Visible"].Equals("0")) continue;
				var navItem = new NavigationItem
								{
									PageKey = int.Parse(node.Key),
									Description = node.Description,
									Title = node.Title,
									Url = node.Url,
									CssClass = "navItem"
								};

				navItems.Add(navItem);

				//if our node is the current node add a class
				if (node.Equals(SiteMap.CurrentNode))
				{
					navItem.CssClass += " current";
				}

				//if our current node is this node OR its parent is the parent of the current node then get it's children
				if (!node.Equals(SiteMap.CurrentNode) && !node.Equals(parentNode)) continue;

				navItem.CssClass += " selected";



				var childPages = (from SiteMapNode child in node.ChildNodes
								  where child["Visible"] != null && child["Visible"].Equals("1")
								  select new NavigationItem
											{
												Title = child.Title,
												Description = child.Description,
												PageKey = int.Parse(child.Key),
												Url = child.Url,
												CssClass = "childNavItem navItem"
											}).ToList();
				navItem.ChildPages = childPages;

				//if we dont have a current node dont bother to try and match
				if (SiteMap.CurrentNode == null) continue;

				//incase a child page is actually the current page try to find a nav item with a matching key
				var currentPage = childPages.FirstOrDefault(i => i.PageKey.Equals(int.Parse(SiteMap.CurrentNode.Key)));
				//if we have a page add a current class
				if (currentPage != null)
				{
					currentPage.CssClass += " current";
				}
			}

			return navItems.AsQueryable();
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
						ChildPages = navItem.ChildNodes.Cast<SiteMapNode>().Select(n => new NavigationItem{
							PageKey = int.Parse(n.Key),
							Title = n.Title,
							Description = n.Description,
							CssClass ="sitemapNavItem navItem",
							ImageUrl = n["imageURL"],
							Url = n.Url,
							ChildPages = n.ChildNodes.Cast<SiteMapNode>().Select(no => new NavigationItem{
								PageKey = int.Parse(no.Key),
								Title = no.Title,
								Description = no.Description,
								CssClass ="sitemapNavItem navItem",
								ImageUrl = no["imageURL"],
								Url = no.Url
							}).ToList()
						}).ToList()


					}).AsQueryable();
		}
	}
}
