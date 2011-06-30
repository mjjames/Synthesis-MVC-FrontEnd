using System.Linq;
using System.Web.Mvc;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
	public class ArticlesController : Controller
	{
		readonly NavigationRepository _navs = new NavigationRepository();
		readonly ArticleRepository _articles = new ArticleRepository();

		public ActionResult Index()
		{
			var articles = _articles.FindAllActive().Select(a => ArticleToDto(a));
			var indexArticle = articles.FirstOrDefault();

			return BuildArticle(articles, indexArticle);
		}

		public ActionResult Article(string url)
		{
			var articles = _articles.FindAllActive().Select(a => ArticleToDto(a));
			var indexArticle = articles.FirstOrDefault(a => a.Url.EndsWith(url));

			return BuildArticle(articles, indexArticle);
		}

		private ArticleDTO ArticleToDto(DataEntities.Article article)
		{
			return new ArticleDTO
			{
				Url = article.url,
				Title = article.title,
				Body = article.body,
				Date =
				{
					EndDate = article.end_date,
					StartDate = article.start_date
				},
				InFeed = article.include_in_feed,
				Description = article.shortdescription,
				ThumbnailImage = article.thumbnailimage

			};
		}

		private ActionResult BuildArticle(IQueryable<ArticleDTO> articles, ArticleDTO article)
		{
			if (article == null)
			{
				return View("NotFound");
			}

			var articleModel = new ArticleModel
			{
				Articles = articles.ToList(),
				MainNavigation = _navs.GetMainNavigation().ToList(),
				FooterNavigation = _navs.GetFooterNavigation().ToList(),
				Body = article.Body,
				Date = { EndDate = article.Date.EndDate, StartDate = article.Date.StartDate },
				Description = article.Description,
				InFeed = article.InFeed,
				ThumbnailImage = article.ThumbnailImage,
				Title = article.Title,
				Url = article.Url

			};
			return View(articleModel);
		}


	}
}
