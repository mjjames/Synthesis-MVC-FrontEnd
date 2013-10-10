﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using mjjames.DataEntities;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories
{
    public class PageModelRepository
    {
        readonly PageRepository _pages = new PageRepository();
        readonly NavigationRepository _navs = new NavigationRepository();
        readonly KeyValueRepository _keyvalueRepository = new KeyValueRepository();
        readonly MediaRepository _mediaRepository = new MediaRepository();

        /// <summary>
        /// Get the page model for the page with the numeric ID (key)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PageModel FromId(int id)
        {
            var page = _pages.Get(id);
            return PageToPageModel(page);
        }

        /// <summary>
        /// Get the page model for the page with the PageId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PageModel FromId(string id)
        {
            var page = _pages.Get(id);
            return PageToPageModel(page);
        }

        public PageModel FromUrl(string url)
        {
            var page = _pages.GetPageFromUrl(url);
            return PageToPageModel(page);
        }

        private PageModel PageToPageModel(Page entity)
        {
            if (entity == null)
            {
                return null;
            }

            var textInfo = CultureInfo.CurrentCulture.TextInfo;

            var model = new PageModel
            {
                AccessKey = entity.accesskey,
                Active = entity.active,
                Body = entity.body,
                FooterNavigation = _navs.GetFooterNavigation().ToList(),
                LastModified = entity.lastmodified,
                LinkURL = entity.linkurl,
                MainNavigation = _navs.GetMainNavigation().ToList(),
                MetaDescription = entity.metadescription,
                MetaKeywords = entity.metakeywords,
                NavTitle = entity.navtitle,
                PageFKey = entity.page_fkey,
                PageKey = entity.page_key,
                PageUrl = entity.page_url,
                PageID = entity.pageid ?? "",
                Password = entity.password,
                PasswordProtect = entity.passwordprotect,
                ShowInFeaturedNav = entity.showinfeaturednav,
                ShowInFooter = entity.showinfooter,
                ShowInNav = entity.showinnav,
                ShowOnHome = entity.showonhome,
                SortOrder = entity.sortorder,
                ThumbnailImage = entity.thumbnailimage,
                Title = entity.title,
                KeyValues = _keyvalueRepository.ByLink(entity.page_key, "pagelookup").ToDictionary(kv => kv.lookup.lookup_id, kv =>
                                                                                          new KeyValueDto
                                                                                          {
                                                                                              Id = kv.keyvalue_key,
                                                                                              Title = textInfo.ToTitleCase(kv.lookup.title),
                                                                                              Value = textInfo.ToTitleCase(kv.value)

                                                                                          }),
                GalleryImages = _mediaRepository.GetByMediaLinkType("page_galleryimage")
                                .Where(m => m.active && m.MediaLinks.Any(ml => ml.link_fkey == entity.page_key))
                                .Select(m => new MediaDTO
                                {
                                    Description = m.description,
                                    FileName = m.filename,
                                    Link = m.link,
                                    Title = textInfo.ToTitleCase(m.title)
                                })
            };
            if (!String.IsNullOrEmpty(model.ThumbnailImage))
            {
                model.ThumbnailImage = System.IO.Path.Combine(ConfigurationManager.AppSettings["uploaddir"], model.ThumbnailImage);
            }


            switch (model.PageID.ToUpper())
            {
                case "HOME":
                    var homePage = new HomePageModel(model)
                        {
                            HomeNavigation = _navs.GetHomePageNavigation().ToList()
                        };
                    return homePage;
                case "SITEMAP":
                    var siteMap = new SiteMapPageModel(model)
                        {
                            SiteMapNavigation = _navs.GetSiteMapNavigation().ToList()
                        };
                    return siteMap;
            }

            return model;
        }
    }
}
