using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using System.Globalization;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
	public class PodcastsController : Controller
	{
		readonly NavigationRepository _navs = new NavigationRepository();
		readonly PodcastRepository _podcasts = new PodcastRepository();
		public ActionResult Podcasts(string startDate)
		{
			var date = new DateTime();
			if (!DateTime.TryParseExact(startDate, "yyyy-MM", new CultureInfo("en-GB"), DateTimeStyles.None, out date))
			{
				date = DateTime.Now;
			}
			var model = new PodcastModel()
			{
				Date = date,
				MainNavigation = _navs.GetMainNavigation().ToList(),
				FooterNavigation = _navs.GetFooterNavigation().ToList(),
				Podcasts = _podcasts.GetPodcastsByDate(date).Select(p => new PodcastDTO { 
					Title = p.Title,
					Description = p.Description,
					FileName = p.Filename,
					Published = p.Published
				}).ToList()

			};
			return View("Podcasts", model);
		}
	}
}
