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
        
        [ChildActionOnly]
		public ActionResult Banners(MediaType mediaType, int number = 4, string viewName = "Banners")
		{
			var banners = _mediaRepository.GetByLookupID(mediaType.ToString())
                .Select( m => new MediaDTO{
				Description = m.description,
				FileName = m.filename,
				Link = m.link,
				Title  = m.title,
                });
            var orderedBanners = banners.AsEnumerable().OrderBy(s => Guid.NewGuid());

            return View(viewName, orderedBanners.Take(number));
		}
	}
}
