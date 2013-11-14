using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
	public class HomePageModel : PageModel
	{
		public HomePageModel() 
		{
		}

		/// <summary>
		/// creates a homepagemodel from a pagemodel
		/// </summary>
		/// <param name="model">PageModel to create a HomePageModel from</param>
		public HomePageModel(PageModel model)
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
			ShowInFeaturedNav = model.ShowInFeaturedNav;
			ShowInFooter = model.ShowInFooter;
			ShowInNav = model.ShowInNav;
			ShowOnHome = model.ShowOnHome;
			SiteFKey = model.SiteFKey;
			SortOrder = model.SortOrder;
			ThumbnailImage = model.ThumbnailImage;
			Title = model.Title;
            PageTitle = model.PageTitle;
		}
		public IList<NavigationItem> HomeNavigation { get; set; }
	}
}
