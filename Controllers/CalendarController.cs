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
using mjjames.MVC_MultiTenant_Controllers_and_Models.Interfaces;
using System.Threading.Tasks;
using mjjames.MVC_MultiTenant_Controllers_and_Models.ActionFilters;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
    [PasswordProtectedSiteFilter]
    public class CalendarController : Controller
    {
        readonly NavigationRepository _navs = new NavigationRepository();
        private ISiteSettings _siteSettings;
        private Site _site;

        public CalendarController()
            : this(new DatabaseSiteSettings(new Site()))
        {
        
        }

        public CalendarController(ISiteSettings siteSettings)
        {
            _siteSettings = siteSettings;
            _site = new Site();
        }

        public ActionResult Calendar(string startDate, string eventID)
        {
            var date = new DateTime();
            if (!DateTime.TryParseExact(startDate, "yyyy-MM", new CultureInfo("en-GB"), DateTimeStyles.None, out date))
            {
                date = DateTime.Now;
            }
            var events = GetCalendarEvents(_siteSettings.GetSetting<string>("GoogleCalendarFeedURL"), date, date.AddMonths(1));
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
                PageTitle = String.Format("{0} - {1}",
                    String.IsNullOrEmpty(eventID) ? _siteSettings.GetSetting<string>("CalendarPageTitle") : entry.Title,
                    date.ToString("MMMM yyyy")
                ),
                Site = _site
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

        [ChildActionOnly]
        public ActionResult Month(string startDate)
        {
            var feedUrl = _siteSettings.GetSetting<string>("GoogleCalendarFeedURL");
            return MonthCustomFeed(startDate, feedUrl);
        }

        [ChildActionOnly]
        public ActionResult MonthCustomFeed(string startDate, string feedUrl)
        {
            var date = new DateTime();
            if (!DateTime.TryParseExact(startDate, "yyyy-MM", new CultureInfo("en-GB"), DateTimeStyles.None, out date))
            {
                date = DateTime.Now;
            }
            var events = GetCalendarEvents(feedUrl, date, date.AddMonths(1));
            if (!events.Any())
            {
                events.Add(new CalendarEntryDTO { Title = "Event Information Unavailable at this time" });
            }
            else
            {
                events = events.Where(e => e.Start >= DateTime.Now && e.End >= DateTime.Now).ToList();
                if (!events.Any())
                {
                    events.Add(new CalendarEntryDTO { Title = "No More Events This Month" });
                }
            }
            ViewBag.Site = _site;
            return View(events);
        }

        [ChildActionOnly]
        public ActionResult ThisWeek()
        {
            ViewBag.Site = _site;
            var events = GetCalendarEvents(_siteSettings.GetSetting<string>("GoogleCalendarFeedURL"), DateTime.Today, DateTime.Today.AddDays(6));
            return View(events);
        }

        [ChildActionOnly]
        /// <summary>
        /// Gets the WhatsOnNext PartialView using Google Calendars
        /// </summary>
        /// <returns>PartialView</returns>
        public ActionResult WhatsOnNext()
        {
            //get our  next event entry
            var eventEntry = ExternalModulesFactory.GetCalendarInstance(_siteSettings.GetSetting<string>("GoogleCalendarFeedURL")).GetNextEvent();
            //convert the eventEntry into a CalendarEntry
            var whatsonNext = new CalendarEntryDTO();

            if (eventEntry != null)
            {
                whatsonNext = new CalendarEntryDTO
                {
                    Description = eventEntry.Summary.Text,
                    Location = eventEntry.Locations[0] != null ? eventEntry.Locations[0].ValueString : "",
                    Title = eventEntry.Title.Text,
                    End = eventEntry.Times.Any() && eventEntry.Times[0] != null ? eventEntry.Times[0].EndTime : new DateTime(),
                    Start = eventEntry.Times.Any() && eventEntry.Times[0] != null ? eventEntry.Times[0].StartTime : new DateTime()
                };
            }
            else
            {
                whatsonNext.Title = "Event Information Unavailable at this time";
            }

            return View("WhatsOnNext", whatsonNext);
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
