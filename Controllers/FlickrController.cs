using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlickrNet;
using System.Web.Mvc;
using System.Web;
using System.Configuration;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
    public class FlickrController : Controller
    {
        private Flickr _flickr;
        readonly NavigationRepository _navs = new NavigationRepository();
        private Site _site;
        public FlickrController()
        {
            _site = new Site();
        }

        private Flickr Flickr
        {
            get
            {
                if (_flickr != null)
                {
                    return _flickr;
                }
                string flickrCache = ConfigurationManager.AppSettings["cacheLocation"];
                if (string.IsNullOrWhiteSpace(flickrCache))
                {
                    throw new ApplicationException("cacheLocation not set in config");
                }
                Flickr.CacheLocation = Server.MapPath(flickrCache);

                string apikey = _site.Setting("Flickr:ApiKey");

                if (string.IsNullOrWhiteSpace(apikey))
                {
                    throw new ApplicationException("flickrApiKey not set in settings");
                }

                string apisecret = _site.Setting("Flickr:ApiSecret");

                if (string.IsNullOrWhiteSpace(apisecret))
                {
                    throw new ApplicationException("flickrApiSecret not set in settings");
                }

                _flickr = new Flickr(apikey, apisecret);
                return _flickr;
            }
        }

        /// <summary>
        /// Returns all the flickr sets for a username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult FlickrSets(string userName)
        {
            var sets = findSets(userName);
            return View("FlickrSets", sets.Select(s => new FlickrSetDto
            {
                Id = s.PhotosetId,
                Title = s.Title,
                Description = s.Description,
                DateCreated = s.DateCreated,
                DateUpdated = s.DateUpdated,
                Thumbnail = GetPhoto(s.PrimaryPhotoId)
            }));
        }

        public ActionResult Index()
        {
            return View("Index", new PageModel
            {
                Title = "Galleries",
                FooterNavigation = _navs.GetFooterNavigation().ToList(),
                MainNavigation = _navs.GetMainNavigation().ToList()
            });
        }

        public ActionResult Gallery(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Index();
            }

            var photoSetCacheKey = "photoset-photos-" + id;
            var photosetInfoCacheKey = "photoset-info-" + id;

            var photosetPhotos = HttpContext.Cache[photoSetCacheKey] as PhotosetPhotoCollection;
            if (photosetPhotos == null)
            {
                photosetPhotos = Flickr.PhotosetsGetPhotos(id);
                HttpContext.Cache[photoSetCacheKey] = photosetPhotos;
            }

            var photoset = HttpContext.Cache[photosetInfoCacheKey] as Photoset;
            if (photoset == null)
            {
              photoset = Flickr.PhotosetsGetInfo(id);
              HttpContext.Cache[photosetInfoCacheKey] = photoset;
            }
            return View(new FlickrGalleryModel
            {
                Title = "Gallery: " + photoset.Title,
                GalleryInfo = new FlickrSetDto
                {
                    Title = photoset.Title,
                    DateCreated = photoset.DateCreated,
                    DateUpdated = photoset.DateUpdated,
                    Description = photoset.Description,
                    Id = photoset.PhotosetId
                },
                Photos = photosetPhotos.Select(p => PhotoToPhotoDto(p)),
                FooterNavigation = _navs.GetFooterNavigation().ToList(),
                MainNavigation = _navs.GetMainNavigation().ToList()
            });
        }

       

        private FlickrPhotoDto GetPhoto(string photoId)
        {
            var cacheKey = "photo-" + photoId;
            var photo = HttpContext.Cache[cacheKey] as PhotoInfo;
            if (photo == null)
            {
                photo = Flickr.PhotosGetInfo(photoId);
                HttpContext.Cache[cacheKey] = photo;
            }
            return PhotoInfoToPhotoDto(photo);
        }

        private FlickrPhotoDto PhotoInfoToPhotoDto(PhotoInfo photo){
            return new FlickrPhotoDto
            {
                Id = photo.PhotoId,
                ThumbnailUrl = photo.ThumbnailUrl,
                SquareThumbnailUrl = photo.SquareThumbnailUrl,
                MediumUrl = photo.MediumUrl,
                LargeUrl = photo.LargeUrl,
                SmallLarge = photo.SmallUrl,
                Title = photo.Title,
                Description = photo.Description
            };
        }
        
        private FlickrPhotoDto PhotoToPhotoDto(Photo photo)
        {
            return new FlickrPhotoDto
            {
                Id = photo.PhotoId,
                ThumbnailUrl = photo.ThumbnailUrl,
                SquareThumbnailUrl = photo.SquareThumbnailUrl,
                MediumUrl = photo.MediumUrl,
                LargeUrl = photo.LargeUrl,
                SmallLarge = photo.SmallUrl,
                Title = photo.Title,
                Description = photo.Description
            };
        }


        private PhotosetCollection findSets(string userName)
        {
            var cacheKey = "username-sets";
            var cache = HttpContext.Cache[cacheKey] as PhotosetCollection;
            if (cache != null)
            {
                return cache;
            }
            PhotosetCollection userPhotoSets = new PhotosetCollection();
            try
            {
                FoundUser ourUser = Flickr.PeopleFindByUserName(userName);
                string strUserID = ourUser.UserId;
                userPhotoSets = Flickr.PhotosetsGetList(strUserID);
            }
            catch (Exception e) //if an error occurs let something else handle it
            {
                //userPhotoSets = null;
            }
            HttpContext.Cache[cacheKey] = userPhotoSets;
            return userPhotoSets;
        }

    }
}
