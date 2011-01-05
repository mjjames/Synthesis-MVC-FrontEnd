using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using mjjames.DataEntities;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
	[HandleError]
	public class PagesController : Controller
	{
		readonly PageRepository _pages = new PageRepository();
		readonly NavigationRepository _navs = new NavigationRepository();

		/// <summary>
		/// Default catch all, calls home
		/// </summary>
		/// <returns></returns>
		public ActionResult Index()
		{
			return Page("HOME");
		}

		/// <summary>
		/// loads a view from its ID
		/// </summary>
		/// <param name="id">Page URL</param>
		/// <returns></returns>
		public ActionResult Page(string id)
		{
			var ourPage = (String.IsNullOrEmpty(id) || id.Equals("HOME", StringComparison.OrdinalIgnoreCase)) ? _pages.Get("HOME") : _pages.GetPageFromUrl(id);
			return BuildPage(ourPage);
		}

		public ActionResult PageFromID(string id)
		{
			return BuildPage(_pages.Get(id));
		}

		/// <summary>
		/// loads a view from its key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public ActionResult PageFromKey(int key)
		{
			var ourPage = _pages.Get(key);

			return BuildPage(ourPage);
		}

		/// <summary>
		/// Takes a Page and converts it into a PageModel with Navigations
		/// </summary>
		/// <param name="ourPage">Page to load</param>
		/// <returns></returns>
		private ActionResult BuildPage(Page ourPage)
		{
			if (ourPage == null)
			{
				return View("NotFound");
			}
			if (!String.IsNullOrEmpty(ourPage.linkurl))
			{
				Response.Redirect(ourPage.linkurl, true);
			}

			PageModel newPage = new PageModel
							{
								AccessKey = ourPage.accesskey,
								Active = ourPage.active,
								Body = ourPage.body,
								FooterNavigation = _navs.GetFooterNavigation().ToList(),
								LastModified = ourPage.lastmodified,
								LinkURL = ourPage.linkurl,
								MainNavigation = _navs.GetMainNavigation().ToList(),
								MetaDescription = ourPage.metadescription,
								MetaKeywords = ourPage.metakeywords,
								NavTitle = ourPage.navtitle,
								PageFKey = ourPage.page_fkey,
								PageKey = ourPage.page_key,
								PageUrl = ourPage.page_url,
								PageID = ourPage.pageid,
								Password = ourPage.password,
								PasswordProtect = ourPage.passwordprotect,
								ShowInFeaturedNav = ourPage.showinfeaturednav,
								ShowInFooter = ourPage.showinfooter,
								ShowInNav = ourPage.showinnav,
								ShowOnHome = ourPage.showonhome,
								SortOrder = ourPage.sortorder,
								ThumbnailImage = ourPage.thumbnailimage,
								Title = ourPage.title
							};
			if (!String.IsNullOrEmpty(newPage.ThumbnailImage))
			{
				newPage.ThumbnailImage = System.IO.Path.Combine(ConfigurationManager.AppSettings["uploaddir"], newPage.ThumbnailImage);
			}

			if (!String.IsNullOrEmpty(newPage.PageID))
			{
				if (newPage.PageID.Equals("HOME", StringComparison.OrdinalIgnoreCase))
				{
					var homePage = new HomePageModel(newPage);
					homePage.HomeNavigation = _navs.GetHomePageNavigation().ToList();
					return View("Home", homePage);
				}

				if (newPage.PageID.Equals("SITEMAP", StringComparison.OrdinalIgnoreCase))
				{
					var siteMap = new SiteMapPageModel(newPage);
					siteMap.SiteMapNavigation = _navs.GetSiteMapNavigation().ToList();
					return View("SiteMap", siteMap);
				}
			}
			
			//TODO: Add templating by fiddling the view
			var templateView = "Page";
			return View(templateView, newPage);
		}

	   
	}
}
