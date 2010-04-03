using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Mvc;
using System.Web;

namespace mjjames.Models
{
	public class ImageResult : ActionResult
	{
		readonly Image _image;

		public ImageResult(Image image)
		{
			this._image = image;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			HttpResponseBase response = context.HttpContext.Response;
			response.ContentType = "image/jpeg";
			_image.Save(response.OutputStream, ImageFormat.Jpeg);
			_image.Dispose();


			response.Cache.SetExpires(DateTime.Now.AddMonths(1));
			response.Cache.SetCacheability(HttpCacheability.Public);
			response.Cache.SetNoServerCaching();
		}
	}
}