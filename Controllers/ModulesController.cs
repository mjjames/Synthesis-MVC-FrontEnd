using System;
using System.Linq;
using System.Web.Mvc;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
	[HandleError]
	public class ModulesController : Controller
	{
		/// <summary>
		/// Gets the BibleGatewayVerseOfTheDay PartialView
		/// </summary>
		/// <param name="version">BibleGateway Bible Version</param>
		/// <returns>PartialView</returns>
		public ActionResult BibleGatewayVerseOfTheDay(BibleGateway.BibleTranslation version)
		{
			var verseoftheday = new BibleGateway.BibleGatewayWrapper().GetVerseOfTheDay(version);

			return View("VerseOfTheDay", verseoftheday);
		}

		/// <summary>
		/// Gets the WhatsOnNext PartialView using Google Calendars
		/// </summary>
		/// <param name="feedURL">Google Calendar Feed URL</param>
		/// <returns>PartialView</returns>
		public ActionResult WhatsOnNext(string feedURL)
		{
			//get our  next event entry
			var eventEntry = new GoogleCalendarWrapper.GoogleCalendar(feedURL).GetNextEvent();
			//convert the eventEntry into a CalendarEntry
			var whatsonNext = new CalendarEntryDTO();

			if (eventEntry != null)
			{
				whatsonNext = new CalendarEntryDTO
								{
									Description = eventEntry.Summary.Text,
									Location = eventEntry.Locations[0] != null ? eventEntry.Locations[0].ValueString : "",
									Title = eventEntry.Title.Text,
                                    End = eventEntry.Times.Any()  && eventEntry.Times[0] != null ? eventEntry.Times[0].EndTime : new DateTime(),
									Start = eventEntry.Times.Any() && eventEntry.Times[0] != null ? eventEntry.Times[0].StartTime : new DateTime()
								};
			}
			else
			{
				whatsonNext.Title = "Event Information Unavailable at this time";
			}

			return View("WhatsOnNext", whatsonNext);
		}

		//TODO refactor this calendar code into the Calendar Controller, shouldn't be here

		/// <summary>
		/// 
		/// </summary>
		/// <param name="feedURL"></param>
		/// <param name="startDate"></param>
		/// <returns></returns>
		public ActionResult WhatsOnMonthView(string feedURL, DateTime startDate)
		{
			//get a month's worth of entries based upon the start date
			var eventEntries = new GoogleCalendarWrapper.GoogleCalendar(feedURL)
				.GetEvents(startDate, startDate.AddMonths(1)).Select(eventEntry => new CalendarEntryDTO
																					   {
																						   Identifier = eventEntry.Id.ToString(),
																						   Description = eventEntry.Content != null ? eventEntry.Content.Content : "",
																						   Location = eventEntry.Locations[0] != null ? eventEntry.Locations[0].ValueString : "",
																						   Title = eventEntry.Title.Text,
																						   End = eventEntry.Times.Any()  && eventEntry.Times[0] != null ? eventEntry.Times[0].EndTime : new DateTime(),
																						   Start = eventEntry.Times.Any() && eventEntry.Times[0] != null ? eventEntry.Times[0].StartTime : new DateTime()
																					   }).OrderBy(e => e.Start).ToList();
            if (!eventEntries.Any())
            {
                eventEntries.Add(new CalendarEntryDTO { Title = "Event Information Unavailable at this time" });
            }
            else
            {
                eventEntries = eventEntries.Where(e => e.Start >= DateTime.Now && e.End >= DateTime.Now).ToList();
                if (!eventEntries.Any())
                {
                    eventEntries.Add(new CalendarEntryDTO { Title = "No More Events This Month" });
                }
            }

			return View("CalendarMonthView", eventEntries);
		}		
	}
}
