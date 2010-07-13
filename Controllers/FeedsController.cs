using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Feeds;
using System.Configuration;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
	public class FeedsController : Controller
	{
		private readonly PodcastRepository _podcasts = new PodcastRepository();

		public ActionResult Podcasts(string type)
		{
			this.HttpContext.Response.AddHeader("Content-Type", "text/xml");
			var data = _podcasts.FindAllActive().ToList();

			var buildDate = data.Max(p => p.Published);

			var feed = new PodcastFeed
			{
				Title = ConfigurationManager.AppSettings["SiteName"] + " - Podcasts Feed",
				Link = HttpContext.Request.Url.ToString(),
				Description = ConfigurationManager.AppSettings["PodcastDescription"],
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

			return View(type.ToUpper(), feed);
		}
	}
}
