using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Globalization;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
	public class CalendarController : Controller
	{
		readonly NavigationRepository _navs = new NavigationRepository();
		public ActionResult Calendar(string startDate)
		{
			var date = new DateTime();
			if (!DateTime.TryParseExact(startDate, "yyyy-MM", new CultureInfo("en-GB") , DateTimeStyles.None,  out date))
			{
				date = DateTime.Now;
			}
			var model = new CalendarModel(){
				Date = date,
				MainNavigation = _navs.GetMainNavigation().ToList(),
				FooterNavigation = _navs.GetFooterNavigation().ToList()

			};
			return View("Calendar", model);
		}
	}
}
