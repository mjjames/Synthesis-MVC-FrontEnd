using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Feeds;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
    public class FeedsController : Controller
    {
        private readonly PodcastRepository _podcasts = new PodcastRepository();
        private Site _site;
        public FeedsController()
        {
            _site = new Site();
        }

        public ActionResult Podcasts(string feedType)
        {
            return PodcastsByType(PodcastType.Podcast, feedType);
        }



        public ActionResult PodcastsByType(PodcastType podcastType, string feedType)
        {
            this.HttpContext.Response.AddHeader("Content-Type", "text/xml");
            var data = _podcasts.GetPodcastsByType(podcastType).ToList();

            var buildDate = data.Max(p => p.Published);

            var feed = new PodcastFeed
            {
                Title = String.Format("{0} - {1}s {2} Feed", _site.Setting("SiteName"), podcastType, feedType.ToUpper()),
                Link = HttpContext.Request.Url.ToString(),
                Description = _site.Setting("Podcasts:Description:" + podcastType),
                BuildDate = buildDate.HasValue ? buildDate.Value : DateTime.Now,
                PubDate = DateTime.Now,
                FeedItems = data.Select(p => new PodcastDTO()
                {
                    Title = p.Title,
                    Description = p.Description,
                    FileName = p.Filename.Replace(" ", "+"),
                    Published = p.Published
                }).AsEnumerable(),

            };

            return View(feedType.ToUpper(), feed);
        }
    }
}
