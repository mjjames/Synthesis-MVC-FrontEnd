using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
	public class SiteMapPageModel : PageModel
	{
		/// <summary>
		/// creates a sitemappagemodel from a pagemodel
		/// </summary>
		/// <param name="model">PageModel to create a SiteMapPageModel from</param>
		public SiteMapPageModel(PageModel model)
		{
			AccessKey = model.AccessKey;
			Active = model.Active;
			Body = model.Body;
			FooterNavigation = model.FooterNavigation;
			LastModified = model.LastModified;
			LinkURL = model.LinkURL;
			MainNavigation = model.MainNavigation;
			MetaDescription = model.MetaDescription;
			MetaKeywords = model.MetaKeywords;
			NavTitle = model.NavTitle;
			PageFKey = model.PageFKey;
			PageID = model.PageID;
			PageKey = model.PageKey;
			PageUrl = model.PageUrl;
			Password = model.Password;
			PasswordProtect = model.PasswordProtect;
			Site = model.Site;
			ShowInFeaturedNav = model.ShowInFeaturedNav;
			ShowInFooter = model.ShowInFooter;
			ShowInNav = model.ShowInNav;
			ShowOnHome = model.ShowOnHome;
			SiteFKey = model.SiteFKey;
			SortOrder = model.SortOrder;
			ThumbnailImage = model.ThumbnailImage;
			Title = model.Title;
		}

		public SiteMapPageModel() { }

		public IList<NavigationItem> SiteMapNavigation { get; set; }
	}
}
