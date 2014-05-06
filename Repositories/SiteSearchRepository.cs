using mjjames.DataEntities;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Interfaces;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories
{
    internal class SiteSearchRepository : ISiteSearchRepository
    {
        private IRepository<Page> _pageRepository;
        private IRepository<Article> _articleRepository;
        private IRepository<Project> _projectRepository;
        private IRepository<Media> _mediaRepository;
        private ISiteSettings _siteSettings;
        internal SiteSearchRepository(IRepository<Page> pageRepository,
                                    IRepository<Article> articleRepository,
                                    IRepository<Project> projectRepository,
                                    IRepository<Media> mediaRepository,
                                    ISiteSettings siteSettings
            )
        {
            _pageRepository = pageRepository;
            _articleRepository = articleRepository;
            _projectRepository = projectRepository;
            _mediaRepository = mediaRepository;
            _siteSettings = siteSettings;
        }

        public IList<SiteSearchResultDto> Search(string searchTerm)
        {
            var results = new List<SiteSearchResultDto>();
            //eeee need some free text love
            results.AddRange(GetPageResults(searchTerm));

            if (_siteSettings.GetSetting<bool>("SiteSearch:ArticleEnabled"))
            {
                results.AddRange(GetArticleResults(searchTerm));
            }

            if (_siteSettings.GetSetting<bool>("SiteSearch:ProjectEnabled"))
            {
                results.AddRange(GetProjectResults(searchTerm));
            }

            if (_siteSettings.GetSetting<bool>("SiteSearch:MediaEnabled"))
            {
                results.AddRange(GetMediaResults(searchTerm, _siteSettings.GetSetting<string>("SiteSearch:MediaTypeIds").Split(',')));
            }
            return results;
        }

        private IEnumerable<SiteSearchResultDto> GetMediaResults(string searchTerm, string[] mediaTypeIds)
        {
            return _mediaRepository.FindAllActive()
                            .Where(p => (p.title.Contains(searchTerm) || p.description.Contains(searchTerm)) && mediaTypeIds.Contains(p.Lookup.lookup_id))
                            .Select(p => new SiteSearchResultDto
                            {
                                Description = p.description,
                                Published = p.publishedonutc,
                                Title = p.title,
                                Url = p.filename,
                            }).ToList();
        }

        private IEnumerable<SiteSearchResultDto> GetProjectResults(string searchTerm)
        {
            return _projectRepository.FindAllActive()
                            .Where(p => p.title.Contains(searchTerm) || p.metadescription.Contains(searchTerm))
                            .Select(p => new SiteSearchResultDto
                            {
                                Description = p.metadescription,
                                Published = p.start_date,
                                Title = p.title,
                                Url = p.url,
                            }).ToList();
        }

        private IEnumerable<SiteSearchResultDto> GetArticleResults(string searchTerm)
        {
            return _articleRepository.FindAllActive()
                                        .Where(p => p.title.Contains(searchTerm) || p.metadescription.Contains(searchTerm) || p.body.Contains(searchTerm))
                                        .Select(p => new SiteSearchResultDto
                                        {
                                            Description = p.metadescription,
                                            Published = p.start_date,
                                            Title = p.title,
                                            Url = p.url,
                                        }).ToList();
        }

        private IEnumerable<SiteSearchResultDto> GetPageResults(string searchTerm)
        {
            return _pageRepository.FindAllActive()
                                            .Where(p => p.title.Contains(searchTerm) || p.metadescription.Contains(searchTerm) || p.body.Contains(searchTerm))
                                            .Select(p => new SiteSearchResultDto
                                            {
                                                Description = p.metadescription,
                                                Published = p.lastmodified,
                                                Title = p.title,
                                                Url = p.page_url,
                                            }).ToList();
        }
    }

    public interface ISiteSearchRepository
    {
        IList<SiteSearchResultDto> Search(string searchTerm);
    }
}
