using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using mjjames.Imaging;

namespace mjjames.Controllers
{
	public class ImageResizerController : Controller
	{
		//
		// GET: /ImageResizer/

		public ActionResult Index()
		{
			return ResizeImage(String.Empty);
		}

		public ImageResult ResizeImage(string filename)
		{
			int height;
			int width;
			string postFilter = RouteData.Values["postfilter"] as String ?? String.Empty;

			int.TryParse(RouteData.Values["height"].ToString(), out height);
			int.TryParse(RouteData.Values["width"].ToString(), out width);

			//to stop abuse and empty values set some defaults if to small or big

            int maxHeight = 700;
            int maxWidth = 970;
            int configHeight;
            int configWidth;

            int.TryParse(ConfigurationManager.AppSettings["ImageResizer:MaxHeight"], out configHeight);
            int.TryParse(ConfigurationManager.AppSettings["ImageResizer:MaxWidth"], out configWidth);

            maxHeight = configHeight > 0 ? configHeight : maxHeight;
            maxWidth = configWidth > 0 ? configWidth : maxWidth; 

			height = height <= 0 || height >= maxHeight ? maxHeight : height;
			width = width <= 0 || width>= 970 ? maxWidth : width;
			
			if(!String.IsNullOrEmpty(filename)) //do a file system check
			{
				filename = filename.Replace("_-_", "/");
				if(!System.IO.File.Exists(Server.MapPath("~/uploads/"+ filename+".jpg")))
				{
					filename = String.Empty;
				}
			}

			
			filename = Server.MapPath(String.IsNullOrEmpty(filename) ? "/content/images/noimage.jpg" : "/uploads/" + filename + ".jpg");
			
			ResizerImage.ResizerAction action = Enum.IsDefined(typeof(ResizerImage.ResizerAction),
			                                                   RouteData.GetRequiredString("resizeaction"))
			                                    	?
			                                    		(ResizerImage.ResizerAction)
			                                    		Enum.Parse(typeof(ResizerImage.ResizerAction),
                                                                   RouteData.GetRequiredString("resizeaction"))
			                                    	: ResizerImage.ResizerAction.resizecrop;
			var resizerImage = new ResizerImage
			                            	{
			                            		Action = action,
			                            		Height = height,
			                            		Width = width
			                            	};


			string sCache = ConfigurationManager.AppSettings["cacheLocation"];

			Image ourImage = LoadTheImage(filename, resizerImage, sCache, postFilter);
			
			return new ImageResult(ourImage);
		}

		private Image LoadTheImage(string filename, ResizerImage resizerImage, string cache, string postFilter)
		{
			var resizer = new Resizer()
			                  	{
			                  		GreyScaleImage =
			                  			postFilter.Equals("blackwhite", StringComparison.OrdinalIgnoreCase)
			                  	};
			
			string imageCacheKey = resizerImage.Action + "-" + resizerImage.Height+ "-" + resizerImage.Width+ "-" + Path.GetFileName(filename);

			ImageCodecInfo codecInfo = GetEncoderInfo("image/jpeg");

			Encoder encoder = Encoder.Quality;
			var encoderParams = new EncoderParameters(1);
			const long compression = 85;
			var encoderParam = new EncoderParameter(encoder, compression);
			encoderParams.Param[0] = encoderParam;


			if (!String.IsNullOrEmpty(cache) && System.IO.File.Exists(Server.MapPath(cache + "/" + imageCacheKey)))
			{
				return Image.FromFile(Server.MapPath(cache + "/" + imageCacheKey));		
			}

			Image ourImage;

			switch (resizerImage.Action)
			{
				case ResizerImage.ResizerAction.resizecrop:
					ourImage = resizer.ResizeCropImage(filename, resizerImage.Width, resizerImage.Height);
					break;
				case ResizerImage.ResizerAction.resize:
					ourImage = resizer.ResizeImage(filename, resizerImage.Width, resizerImage.Height);
					break;
				case ResizerImage.ResizerAction.crop:
					ourImage = resizer.CropImage(filename, resizerImage.Width, resizerImage.Height);
					break;
				default:
					ourImage = Image.FromFile(filename);
					break;
			}
			ourImage.Save(Server.MapPath(Path.Combine(cache, imageCacheKey)), codecInfo, encoderParams);
			return ourImage;
		}

		private static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			var codecs = ImageCodecInfo.GetImageEncoders();
			return codecs.FirstOrDefault(codec => codec.MimeType == mimeType);
		}
	}
}


