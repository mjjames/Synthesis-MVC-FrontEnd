using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using System.Globalization;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.ActionFilters;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
    [PasswordProtectedSiteFilter]
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
                Podcasts = _podcasts.GetPodcastsByDate(PodcastType.Podcast, date).Select(p => new PodcastDTO
                {
                    Title = p.Title,
                    Description = p.Description,
                    FileName = p.Filename,
                    Published = p.Published
                }).ToList()

            };
            return View("Podcasts", model);
        }

        [ChildActionOnly]
        public ActionResult LatestPodcast(PodcastType podcastType)
        {
            //get our latest podcast
			var latest = _podcasts.GetPodcastsByType(podcastType).Where(p => p.Active).OrderByDescending(p => p.Published).Select(p => new PodcastDTO
            {
                Description = p.Description,
                FileName = p.Filename,
                Published = p.Published,
                Title = p.Title
            }).FirstOrDefault();
            //return our LatestPodcast View - the view should handle not having a podcast
            return View("LatestPodcast", latest);
        }

        [ChildActionOnly]
        public ActionResult PodcastsByType(PodcastType podcastType, int number)
        {
            var podcasts = _podcasts.GetPodcastsByType(podcastType).Where(p => p.Active);
            return View("PodcastsByType", podcasts.Select(p => new PodcastDTO
            {
                Description = p.Description,
                FileName = p.Filename,
                Published = p.Published,
                Title = p.Title
            }).Take(number));
        }

         [ChildActionOnly]
        public ActionResult PodcastsByTypeAndDate(PodcastType podcastType, int number, string startDate)
        {
            var date = new DateTime();
            if (!DateTime.TryParseExact(startDate, "yyyy-MM", new CultureInfo("en-GB"), DateTimeStyles.None, out date))
            {
                date = DateTime.Now;
            }
            var podcasts = _podcasts.GetPodcastsByDate(PodcastType.FeaturedPodcast, date);
            return View("PodcastsByType", podcasts.Select(p => new PodcastDTO
            {
                Description = p.Description,
                FileName = p.Filename,
                Published = p.Published,
                Title = p.Title
            }).Take(number));
        }
    }
}
