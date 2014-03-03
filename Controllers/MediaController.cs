using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
    [HandleError]
    public class MediaController : Controller
    {
        private readonly MediaRepository _mediaRepository = new MediaRepository();
        private readonly KeyValueRepository _keyvalueRepository = new KeyValueRepository();

        [ChildActionOnly]
        public ActionResult Banners(MediaType mediaType, int number = 4, string viewName = "Banners")
        {
            var banners = _mediaRepository.GetByLookupID(mediaType.ToString())
                .Select(m => new MediaDTO
                {
                    Description = m.description,
                    FileName = m.filename,
                    Link = m.link,
                    Title = m.title,
                });
            var orderedBanners = banners.AsEnumerable().OrderBy(s => Guid.NewGuid());

            return View(viewName, orderedBanners.Take(number));
        }

        [ChildActionOnly]
        public ActionResult Downloads(string viewName = "Downloads")
        {
            var downloads = _mediaRepository.GetByLookupID(MediaType.Download.ToString())
                                               .OrderByDescending(o => o.media_key)
                                               .Select(m => new
                                               {
                                                   Description = m.description,
                                                   FileName = m.filename,
                                                   Title = m.title,
                                                   KeyValues = _keyvalueRepository.ByLink(m.media_key, "medialookup")
                                               });

            var dtos = downloads.Take(4).ToList().Select(d => new DownloadDto
            {
                Description = d.Description,
                FileName = d.FileName,
                Title = d.Title,
                KeyValues = d.KeyValues.ToDictionary(kv => kv.lookup.lookup_id, kv => new KeyValueDto(kv.keyvalue_key, kv.lookup.title, kv.value))
            });

            return View(viewName, dtos.AsEnumerable());
        }

        public FilePathResult Download(string filename)
        {
            return File(filename, MimeMapping.GetMimeMapping(filename));
        }
    }
}
