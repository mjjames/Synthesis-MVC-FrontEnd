using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Globalization;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using System.Configuration;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Factories;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
	public class CalendarController : Controller
	{
		readonly NavigationRepository _navs = new NavigationRepository();
		//public ActionResult Calendar(string startDate)
		//{
		//    return Calendar(startDate, String.Empty);
		//}

		public ActionResult Calendar(string startDate, string eventID){
			var date = new DateTime();
			if (!DateTime.TryParseExact(startDate, "yyyy-MM", new CultureInfo("en-GB") , DateTimeStyles.None,  out date))
			{
				date = DateTime.Now;
			}
			var events = GetCalendarEvents(ConfigurationManager.AppSettings["GoogleCalendarFeedURL"], date, date.AddMonths(1));
			var	entry = String.IsNullOrWhiteSpace(eventID) ?  events.FirstOrDefault(e => e.Start > DateTime.Today) : events.FirstOrDefault(e => e.Identifier.Equals(eventID));
			
			//if we don't have a valid entry and we have been provided an event ID return a 404
			if(entry == null && !String.IsNullOrEmpty(eventID)){
				return new HttpNotFoundResult();
			}
		
			var model = new CalendarModel(){
				Date = date,
				MainNavigation = _navs.GetMainNavigation().ToList(),
				FooterNavigation = _navs.GetFooterNavigation().ToList(),
				Events = events,
				Event = entry,
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
				.GetEvents(startDate, endDate).Select(eventEntry => new CalendarEntryDTO
				{
					Identifier = eventEntry.Id.AbsoluteUri,
					Description = eventEntry.Content != null ? eventEntry.Content.Content : "",
					Location = eventEntry.Locations.Count > 0 ? eventEntry.Locations[0].ValueString : "",
					Title = eventEntry.Title.Text,
					End = eventEntry.Times.Count > 0 ? eventEntry.Times[0].EndTime : new DateTime(),
					Start = eventEntry.Times.Count > 0 ? eventEntry.Times[0].StartTime : new DateTime()
				}).OrderBy(e => e.Start).ToList();
			return eventEntries;
		}
	}
}
