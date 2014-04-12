using mjjames.MVC_MultiTenant_Controllers_and_Models.ActionFilters;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
    [PasswordProtectedSiteFilter]
    public class ProjectsController : Controller
    {
        public ProjectsController()
        {
            _site = new Site();
            _page = new PageRepository(_site);
        }

        private readonly ProjectRepository _repository = new ProjectRepository();
        private readonly PageRepository _page;
        private readonly NavigationRepository _navs = new NavigationRepository();
        private readonly MediaRepository _mediaRepository = new MediaRepository();
        private readonly KeyValueRepository _keyvalueRepository = new KeyValueRepository();
        private readonly Site _site;

        /// <summary>
        /// Project Listing Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int year)
        {
            var activeProjects = GetActiveProjects();

            var pageContent = _page.Get("PROJECTS");

            return View(new ProjectListingModel
            {
                FooterNavigation = _navs.GetFooterNavigation().ToList(),
                MainNavigation = _navs.GetMainNavigation().ToList(),
                Title = pageContent.title,
                Description = pageContent.body,
                Projects = activeProjects,
                ProjectYear = year,
                Site = _site
            });
        }

        private IEnumerable<ProjectDto> GetActiveProjects()
        {
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            var activeProjects = _repository.FindAllActive().Where(p => (p.end_date == null || p.start_date == null || (p.end_date >= DateTime.Today && p.start_date <= DateTime.Today)))
                .ToList()
                .Select(p => new ProjectDto
                {
                    Description = p.description,
                    DisplayDate = new Models.DisplayDate
                    {
                        EndDate = p.end_date,
                        IsActive = p.active,
                        StartDate = p.start_date
                    },
                    MediaInfo = new Models.MediaInfo
                    {
                        PhotoGalleryId = p.photogallery_id,
                        VideoId = p.video_id,
                        GalleryImages = _mediaRepository.GetByMediaLinkType("project_galleryimage")
                                                   .Where(m => m.active && m.MediaLinks.Any(ml => ml.link_fkey == p.project_key))
                                                   .Select(m => new MediaDTO
                                                   {
                                                       Description = m.description,
                                                       FileName = m.filename,
                                                       Link = m.link,
                                                       Title = textInfo.ToTitleCase(m.title)
                                                   })
                    },
                    Title = p.title,
                    Url = p.url,
                    MetaDescription = p.metadescription,
                    PageTitle = p.pagetitle,
                    ThumbnailImage = p.thumbnailimage,
                    KeyValues = _keyvalueRepository.ByLink(p.project_key, "projectlookup").ToDictionary(kv => kv.lookup.lookup_id, kv => new KeyValueDto(kv.keyvalue_key, kv.lookup.title, kv.value))
                });
            return activeProjects;
        }

        [ChildActionOnly]
        public ActionResult ActiveProjects()
        {
            var activeProjects = GetActiveProjects();
            return View(activeProjects);
        }

        /// <summary>
        /// loads a project from its url
        /// </summary>
        /// <param name="id">Project URL</param>
        /// <param name="year">Year of the project</param>
        /// <returns></returns>
        public ActionResult Project(string id, int year)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(id);
            }
            var project = _repository.GetProjectFromUrlAndYear(id, year);
            if (project == null)
            {
                return View("NotFound", new NotFoundModel()
                {
                    FooterNavigation = _navs.GetFooterNavigation().ToList(),
                    MainNavigation = _navs.GetMainNavigation().ToList(),
                    Site = _site
                });
            }
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            var projectModel = new ProjectModel
            {
                Description = project.description,
                DisplayDate = new DisplayDate
                {
                    EndDate = project.end_date,
                    IsActive = project.active,
                    StartDate = project.start_date
                },
                FooterNavigation = _navs.GetFooterNavigation().ToList(),
                MainNavigation = _navs.GetMainNavigation().ToList(),
                MediaInfo = new MediaInfo
                {
                    PhotoGalleryId = project.photogallery_id,
                    VideoId = project.video_id,
                    GalleryImages = _mediaRepository.GetByMediaLinkType("project_galleryimage")
                                                    .Where(m => m.active && m.MediaLinks.Any(ml => ml.link_fkey == project.project_key))
                                                    .Select(m => new MediaDTO
                                                    {
                                                        Description = m.description,
                                                        FileName = m.filename,
                                                        Link = m.link,
                                                        Title = textInfo.ToTitleCase(m.title)
                                                    })
                },
                Site = _site,
                Title = project.title,
                Url = project.url,
                KeyValues = _keyvalueRepository.ByLink(project.project_key, "projectlookup").ToDictionary(kv => kv.lookup.lookup_id, kv =>
                    new KeyValueDto
                {
                    Id = kv.keyvalue_key,
                    Title = textInfo.ToTitleCase(kv.lookup.title),
                    Value = textInfo.ToTitleCase(kv.value)

                }),
                ThumbnailImage = project.thumbnailimage,
                MetaDescription = project.metadescription
            };
            return View(projectModel);
        }
    }
}
