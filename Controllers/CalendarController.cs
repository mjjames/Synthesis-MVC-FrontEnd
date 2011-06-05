using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Globalization;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="feedURL"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public ActionResult CalendarList(string feedURL, DateTime startDate, DateTime endDate)
		{
			//get a custom range view of events
			var eventEntries = new GoogleCalendarWrapper.GoogleCalendar(feedURL)
				.GetEvents(startDate, endDate).Select(eventEntry => new CalendaryEntryDTO
				{
					Identifier = eventEntry.Id.ToString(),
					Description = eventEntry.Content != null ? eventEntry.Content.Content : "",
					Location = eventEntry.Locations.Count > 0 ? eventEntry.Locations[0].ValueString : "",
					Title = eventEntry.Title.Text,
					End = eventEntry.Times.Count > 0 ? eventEntry.Times[0].EndTime : new DateTime(),
                    Start = eventEntry.Times.Count > 0 ? eventEntry.Times[0].StartTime : new DateTime()
				}).OrderBy(e => e.Start).ToList();
			if (eventEntries.Count() == 0)
			{
				eventEntries.Add(new CalendaryEntryDTO { Title = "Event Information Unavailable at this time" });
			}

			return View("ListView", eventEntries);
		}
	}
}
