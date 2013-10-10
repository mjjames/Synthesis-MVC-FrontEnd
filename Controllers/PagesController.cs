using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using mjjames.DataEntities;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using System.Globalization;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
	[HandleError]
	public class PagesController : Controller
	{
	    readonly PageModelRepository pageModelRepository = new PageModelRepository();

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
            var pageModel = (String.IsNullOrEmpty(id) || id.Equals("HOME", StringComparison.OrdinalIgnoreCase)) ? pageModelRepository.FromId("HOME") : pageModelRepository.FromUrl(id);
			return BuildView(pageModel);
		}

		public ActionResult PageFromID(string id)
		{
		    var pageModel = pageModelRepository.FromId(id);
            return BuildView(pageModel);
		}

		/// <summary>
		/// loads a view from its key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public ActionResult PageFromKey(int key)
		{
            var pageModel = pageModelRepository.FromId(key);

            return BuildView(pageModel);
		}

		/// <summary>
		/// Takes a Page and converts it into a PageModel with Navigations
		/// </summary>
		/// <param name="model">the View model</param>
		/// <returns></returns>
		private ActionResult BuildView(PageModel model)
		{
            if (model == null)
			{
				return View("NotFound");
			}
            if (!String.IsNullOrEmpty(model.LinkURL))
			{
                Response.Redirect(model.LinkURL, true);
			}
            
            //default to the page template
            var templateName = "Page";

			if (!String.IsNullOrEmpty(model.PageID))
			{

                switch (model.PageID.ToUpper())
                {
                    case "HOME":
                        templateName = "Home";
                        break;
                    case "SITEMAP":
                            templateName = "SiteMap";
                            break;
                    case "CONTACTUS":
                            return RedirectToAction("ContactUsIndex", "Forms");
                    default:
                        //if we don't match any other special cases use the page id as the view name
                        templateName = model.PageID;
                        break;
                }
			}

			return View(templateName, model);
		}

	   
	}
}
