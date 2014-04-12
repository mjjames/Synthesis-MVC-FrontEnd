using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using mjjames.MVC_MultiTenant_Controllers_and_Models.ActionFilters;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
    [PasswordProtectedSiteFilter]
    public class ArticlesController : Controller
    {
        readonly NavigationRepository _navs = new NavigationRepository();
        readonly ArticleRepository _articles = new ArticleRepository();

        public ActionResult Index()
        {
            var articles = _articles.FindAllActive().Select(a => ArticleToDto(a)).ToArray().OrderByDescending(a => a.Date.StartDate).ToList();
            var indexArticle = articles.FirstOrDefault();

            return BuildArticle(articles, indexArticle);
        }

        public ActionResult Article(string url)
        {
            var articles = GetArticleListing();
            var indexArticle = articles.FirstOrDefault(a => a.Url.EndsWith(url));

            return BuildArticle(articles, indexArticle);
        }

        private List<ArticleDTO> GetArticleListing()
        {
            var articles = _articles.FindAllActive().Select(a => ArticleToDto(a)).ToArray().OrderByDescending(a => a.Date.StartDate).ToList();
            return articles;
        }

        [ChildActionOnly]
        public ActionResult ArticleListing()
        {
            return View(GetArticleListing());
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
                ThumbnailImage = article.thumbnailimage,
                MetaDescription = article.metadescription,
                PageTitle = article.pagetitle

            };
        }

        private ActionResult BuildArticle(IList<ArticleDTO> articles, ArticleDTO article)
        {
            if (article == null)
            {
                return View("NotFound");
            }

            var articleModel = new ArticleModel
            {
                Articles = articles,
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
            return View("Article", articleModel);
        }


    }
}
