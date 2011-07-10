using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Factories;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
	public class CalendarController : Controller
	{
		readonly NavigationRepository _navs = new NavigationRepository();
		//public ActionResult Calendar(string startDate)
		//{
		//    return Calendar(startDate, String.Empty);
		//}

		public ActionResult Calendar(string startDate, string eventID)
		{
			var date = new DateTime();
			if (!DateTime.TryParseExact(startDate, "yyyy-MM", new CultureInfo("en-GB"), DateTimeStyles.None, out date))
			{
				date = DateTime.Now;
			}
			var events = GetCalendarEvents(ConfigurationManager.AppSettings["GoogleCalendarFeedURL"], date, date.AddMonths(1));
			var entry = String.IsNullOrWhiteSpace(eventID) ? events.FirstOrDefault(e => e.Start > DateTime.Today) : events.FirstOrDefault(e => e.Identifier.Equals(eventID));

			//if we don't have a valid entry and we have been provided an event ID return a 404
			if (entry == null && !String.IsNullOrEmpty(eventID))
			{
				return new HttpNotFoundResult();
			}
			if (entry == null)
			{
				entry = events.LastOrDefault();
			}
			if (entry == null)
			{
				entry = new CalendarEntryDTO { Start = DateTime.Today, Title = "No Further Events This Monrth" };
			}

			var model = new CalendarModel()
			{
				Date = date,
				MainNavigation = _navs.GetMainNavigation().ToList(),
				FooterNavigation = _navs.GetFooterNavigation().ToList(),
				Events = events,
				Event = entry ?? new CalendarEntryDTO(),
				IsDetailsPage = String.IsNullOrEmpty(eventID),
				PageTitle = String.Format("{0} - {1} - {2}",
					String.IsNullOrEmpty(eventID) ? ConfigurationManager.AppSettings["CalendarPageTitle"] : entry.Title,
					date.ToString("MMMM yyyy"),
					ConfigurationManager.AppSettings["SiteName"]
				)
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
			var eventEntries = GetCalendarEvents(feedURL, startDate, endDate);
			if (eventEntries.Count() == 0)
			{
				eventEntries.Add(new CalendarEntryDTO { Title = "Event Information Unavailable at this time" });
			}

			return View("ListView", eventEntries);
		}

		private static List<CalendarEntryDTO> GetCalendarEvents(string feedURL, DateTime startDate, DateTime endDate)
		{
			//get a custom range view of events
			var eventEntries = ExternalModulesFactory.GetCalendarInstance(feedURL)
				.GetEvents(startDate, endDate);

			//we cant just spit out event entries
			//as repeat events simply have multiple times
			//instead loop through each entry and then each time adding the events
			var entryDTOs = new List<CalendarEntryDTO>();
			foreach (var entry in eventEntries)
			{
				foreach (var time in entry.Times)
				{
					var idUrl = new Uri(entry.Id.AbsoluteUri);
					entryDTOs.Add(new CalendarEntryDTO
					{
						Identifier = string.Format("{0}-{1}", idUrl.Segments.Last(), time.StartTime.Ticks),
						Description = entry.Content != null ? entry.Content.Content : "",
						Location = entry.Locations.Count > 0 ? entry.Locations[0].ValueString : "",
						Title = entry.Title.Text,
						Start = time.StartTime,
						End = time.EndTime
					});
				}
			}

			//then on return order by start date
			return entryDTOs.OrderBy(e => e.Start).ToList();
		}
	}
}
