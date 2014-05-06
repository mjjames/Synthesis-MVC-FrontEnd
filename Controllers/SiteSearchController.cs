using mjjames.MVC_MultiTenant_Controllers_and_Models.ActionFilters;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Interfaces;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
    [HandleError]
    [PasswordProtectedSiteFilter]
    public class SiteSearchController : Controller
    {
        private ISiteSearchRepository _siteSearchRepository;
        private Site _site;
        public SiteSearchController()
        {
            _site = new Site();
            _siteSearchRepository = new SiteSearchRepository(new PageRepository(_site), new ArticleRepository(), new ProjectRepository(), new MediaRepository(), new DatabaseSiteSettings(_site));
        }

        public SiteSearchController(ISiteSearchRepository siteSearchRepository)
        {
            _siteSearchRepository = siteSearchRepository;
        }

        [HttpGet]
        public ActionResult Search(string searchTerm)
        {
            var results = _siteSearchRepository.Search(searchTerm);
            return View(new SiteSearchModel
            {
                SearchTerm = searchTerm,
                Results = results,
                Site = _site,
                BreadcrumbNavigation = new List<NavigationItem>(new []{
                    new NavigationItem{
                        Title = "Home",
                        Url = "/"
                    },
                    new NavigationItem{
                        Title = "Search Results",
                        Url= "/search/process" //todo: fix
                    }
                }),
                MainNavigation = new List<NavigationItem>(),
                FooterNavigation = new List<NavigationItem>()
            });
        }
    }
}
