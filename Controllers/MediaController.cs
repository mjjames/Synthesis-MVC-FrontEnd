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
		public ActionResult Banners(MediaType mediaType, int number)
		{
			var banners = _mediaRepository.GetByLookupID(mediaType.ToString()).Select( m => new MediaDTO{
				Description = m.description,
				FileName = m.filename,
				Link = m.link,
				Title  = m.title,
			}).ToList();

			return View("Banners", banners);
		}
	}
}
