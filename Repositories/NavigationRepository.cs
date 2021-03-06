﻿using System.Collections.Generic;
using System.Linq;
using System.Web;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.DataEntities;
using System;
using System.IO;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories
{
    public class NavigationRepository
    {
        public NavigationRepository()
        {
            _site = new mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Site();
            _pageRepository = new PageRepository(_site);
        }
        //all pages come from the page repository - by doing this we don't have to deal with sites ourselves
        private readonly PageRepository _pageRepository;
        private Models.Site _site;

        internal IQueryable<NavigationItem> GetMainNavigation()
        {
            var navItems = new List<NavigationItem>();
            //we build the navigation by first pulling out the home page from the page repository
            var homePage = _pageRepository.Get("HOME"); //use the home page id
            //if we haven't got a home page something is badly wrong
            if (homePage == null)
            {
                throw new ApplicationException("No Home Page Found - There must be a page with an id of HOME");
            }
            //use the home page to add our first nav item
            var homeItem = new NavigationItem
                            {
                                CssClass = "navItem home",
                                Description = "",
                                PageKey = homePage.page_key,
                                Title = "Home",
                                Url = _site.UrlBase,
                                ImageUrl = homePage.thumbnailimage
                            };
            navItems.Add(homeItem);

            //we now need to call the recursive build page tree method, this takes a parent item and the child items to add
            //this then for each child page calls it self - thus recursively builds the tree
            //note we filter the child pages to active and on main nav
            Func<Page, bool> filter = p => p.active.HasValue && p.active.Value && p.showinnav.HasValue && p.showinnav.Value;
            Func<Page, string, List<NavigationItem>> childPages = (p, parentUrl) => GetChildPages(p, Path.Combine(parentUrl, p.page_url), filter);
            navItems.AddRange(BuildPageTree(_site.UrlBase, homePage.Pages.Where(filter).OrderBy(p => p.sortorder), childPages));

            return navItems.AsQueryable();
        }

        /// <summary>
        /// Builds the Navigation Item tree ensuring that each item has it's child pages populated
        /// </summary>
        /// <param name="parentUrl">parent urlof each child</param>
        /// <param name="pages">child pages to turn into navigation items</param>
        /// <param name="childpagesFilter">function to obtain and filter the child pages</param>
        /// <returns>Enumerable collection of Navigation Items</returns>
        private IEnumerable<NavigationItem> BuildPageTree(string parentUrl, IEnumerable<Page> pages, Func<Page, string, List<NavigationItem>> childpagesFilter)
        {
            var childPages = (from p in pages
                              orderby p.sortorder
                              select new NavigationItem
                              {
                                  Title = p.navtitle,
                                  Description = p.metadescription,
                                  PageKey = p.page_key,
                                  Url = Path.Combine(parentUrl, p.page_url),
                                  CssClass = GetCssClass(Path.Combine(parentUrl, p.page_url)),
                                  ChildPages = childpagesFilter(p, parentUrl),
                                  ImageUrl = p.thumbnailimage
                              });
            return childPages;
        }

        /// <summary>
        /// Look at the current page and our page url and see if we need to load the child pages
        /// </summary>
        /// <param name="p">page</param>
        /// <param name="url">url to check against</param>
        /// <param name="filter">Func which indicates how to filter the child pages</param>
        /// <returns></returns>
        private List<NavigationItem> GetChildPages(Page page, string url, Func<Page, bool> filter)
        {
            var childPages = new List<NavigationItem>();
            //simply if our url starts with the url provided then load the child pages
            if (HttpContext.Current.Request.Url.AbsolutePath.StartsWith(url))
            {
                Func<Page, string, List<NavigationItem>> pages = (p, parentUrl) => GetChildPages(p, Path.Combine(parentUrl, p.page_url), filter);
                childPages.AddRange(BuildPageTree(url, page.Pages.Where(filter).OrderBy(p => p.sortorder), pages));
            }
            return childPages;
        }

        /// <summary>
        /// Calculates the CSS class for the provided page
        /// </summary>
        /// <param name="pageUrl">page url to use for current page match</param>
        /// <returns>the generated class</returns>
        private string GetCssClass(string pageUrl)
        {
            //always have navItem
            var cssClass = "navItem";
            //to work out if we are a child page see if we have more than 1 / in the url, one at the start and one after
            if (pageUrl.Select(c => c.Equals("/")).Count() > 1)
            {
                cssClass += " childNavItem";
            }

            //if we have an exact url match mark the page as current
            if (HttpContext.Current.Request.Url.AbsolutePath.Equals(pageUrl, System.StringComparison.InvariantCultureIgnoreCase))
            {
                cssClass += " current";
                //once we have current we don't need to check for selected so drop out
                return cssClass;
            }

            //if our url isn't an exact match but starts with the provided url then the item must be at least selected
            if (HttpContext.Current.Request.Url.AbsolutePath.StartsWith(pageUrl))
            {
                cssClass += " selected";
            }

            return cssClass;
        }

        /// <summary>
        /// Builds the full url for the provided page
        /// </summary>
        /// <param name="page">Page to build the url for</param>
        /// <returns>url for page</returns>
        private string BuildUrl(Page page)
        {
            //create a list to store our url segments in
            var urlSegments = new List<string>();
            //first add our current page url
            urlSegments.Add(page.page_url);
            //now spin through all of our parent pages until we don't have any more and add their urls to the list
            var parent = page.Page1;
            //keep looping until we have no parent and we don't have the home page
            while (parent != null && parent.page_fkey != 0)
            {
                urlSegments.Add(parent.page_url);
                parent = parent.Page1;
            }
            //now we have our segments we need to reverse the list and then join the segments together with /'s
            return "/" + string.Join("/", urlSegments.Reverse<string>().ToArray());
        }

        /// <summary>
        /// Get all the Navigation Items that belong to the Footer Navigation
        /// </summary>
        /// <returns></returns>
        internal IQueryable<NavigationItem> GetFooterNavigation()
        {
            //first get all the active pages that are on the footer
            //next trim out any that don't have parent pages as these are invalid
            return _pageRepository.FindAllActive()
                                    .Where(p => p.showinfooter.HasValue && p.showinfooter.Value)
                                    .ToList()
                                    .Where(p => p.Page1 != null)
                                    .AsQueryable()
                                    .Select(p => new NavigationItem
                                            {
                                                Title = p.title,
                                                CssClass = "footerItem navItem",
                                                Description = p.metadescription,
                                                ImageUrl = p.thumbnailimage,
                                                PageKey = p.page_key,
                                                Url = BuildUrl(p)
                                            });

        }

        /// <summary>
        /// Gets all the Navigation Items that belong to the HomePage Navigation
        /// </summary>
        /// <returns></returns>
        internal IQueryable<NavigationItem> GetHomePageNavigation()
        {
            return _pageRepository.FindAllActive()
                                    .Where(p => p.showonhome.HasValue && p.showonhome.Value)
                                    .ToList()
                                    .Where(p => p.Page1 != null)
                                    .AsQueryable()
                                    .Select(p => new NavigationItem
                                    {
                                        Title = p.title,
                                        CssClass = "homenavItem navItem",
                                        Description = p.metadescription,
                                        PageKey = p.page_key,
                                        ImageUrl = p.thumbnailimage,
                                        Url = BuildUrl(p)
                                    });
        }

        internal IQueryable<NavigationItem> GetFeaturedNavigation()
        {
            //trim out any that don't have parent pages as these are invalid
            return _pageRepository.FindAllActive()
                                    .Where(p => p.showinfeaturednav.HasValue && p.showinfeaturednav.Value)
                                    .ToList()
                                    .Where(p => p.Page1 != null)
                                    .AsQueryable()
                                    .Select(p => new NavigationItem
                                    {
                                        Title = p.title,
                                        CssClass = "featuredItem navItem",
                                        Description = p.metadescription,
                                        ImageUrl = p.thumbnailimage,
                                        PageKey = p.page_key,
                                        Url = BuildUrl(p)
                                    });

        }

        internal IQueryable<NavigationItem> GetChildFeaturedNavigationForPage(int pageKey)
        {
            //trim out any that don't have parent pages as these are invalid
            return _pageRepository.FindAllActive()
                                    .Where(p => p.page_fkey == pageKey && p.showinfeaturednav.HasValue && p.showinfeaturednav.Value)
                                    .ToList()
                                    .Where(p => p.Page1 != null)
                                    .AsQueryable()
                                    .Select(p => new NavigationItem
                                    {
                                        Title = p.title,
                                        CssClass = "featuredItem navItem",
                                        Description = p.metadescription,
                                        ImageUrl = p.thumbnailimage,
                                        PageKey = p.page_key,
                                        Url = BuildUrl(p)
                                    });

        }

        internal IQueryable<NavigationItem> GetSecondaryFeaturedNavigation()
        {
            //trim out any that don't have parent pages as these are invalid
            return _pageRepository.FindAllActive()
                                    .Where(p => p.showinfeaturednav.HasValue && p.showinfeaturednav.Value)
                                    .ToList()
                                    .Where(p => p.Page1 != null)
                                    .AsQueryable()
                                    .Select(p => new NavigationItem
                                    {
                                        Title = p.title,
                                        CssClass = "featuredItem navItem",
                                        Description = p.metadescription,
                                        ImageUrl = p.thumbnailimage,
                                        PageKey = p.page_key,
                                        Url = BuildUrl(p)
                                    });

        }

        internal IQueryable<NavigationItem> GetChildSecondaryFeaturedNavigationForPage(int pageKey)
        {
            //trim out any that don't have parent pages as these are invalid
            return _pageRepository.FindAllActive()
                                    .Where(p => p.page_fkey == pageKey && p.showinsecondaryfeatured.HasValue && p.showinsecondaryfeatured.Value)
                                    .ToList()
                                    .Where(p => p.Page1 != null)
                                    .AsQueryable()
                                    .Select(p => new NavigationItem
                                    {
                                        Title = p.title,
                                        CssClass = "featuredItem navItem",
                                        Description = p.metadescription,
                                        ImageUrl = p.thumbnailimage,
                                        PageKey = p.page_key,
                                        Url = BuildUrl(p)
                                    });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal IQueryable<NavigationItem> GetSiteMapNavigation()
        {
            var navItems = new List<NavigationItem>();
            //we build the navigation by first pulling out the home page from the page repository
            var homePage = _pageRepository.Get("HOME"); //use the home page id
            //if we haven't got a home page something is badly wrong
            if (homePage == null)
            {
                throw new Exception("No Home Page Found - There must be a page with an id of HOME");
            }
            //use the home page to add our first nav item
            var homeItem = new NavigationItem
            {
                CssClass = "navItem home",
                Description = "",
                PageKey = homePage.page_key,
                Title = "Home",
                Url = _site.UrlBase
            };
            navItems.Add(homeItem);

            //we now need to call the recursive build page tree method, this takes a parent item and the child items to add
            //this then for each child page calls it self - thus recursively builds the tree
            //note we filter the child pages to active and on main nav
            Func<Page, bool> filter = p => p.active.HasValue && p.active.Value;
            Func<Page, string, List<NavigationItem>> childPages = (page, parentUrl) => GetChildren(page, Path.Combine(parentUrl, page.page_url), filter);
            navItems.AddRange(BuildPageTree(_site.UrlBase, homePage.Pages.Where(p => p.active.HasValue && p.active.Value), childPages));

            return navItems.AsQueryable();
        }
        private List<NavigationItem> GetChildren(Page page, string parentUrl, Func<Page, bool> filter)
        {
            return new List<NavigationItem>(BuildPageTree(parentUrl, page.Pages.Where(filter), (p, url) => GetChildren(p, Path.Combine(url, p.page_url), filter)));
        }

        internal IQueryable<NavigationItem> GetChildNavigationForPage(int pageKey)
        {
            return GetChildNavigation(pageKey, page => page.showinnav.HasValue && page.showinnav.Value ,"childItem navItem", page => page.sortorder);
        }

        private IQueryable<NavigationItem> GetChildNavigation<TKey>(int parentPageKey, Func<Page, bool> filter ,string cssClass, Func<Page,TKey> sort)
        {
            return _pageRepository.FindAllActive()
                                    .Where(p => p.page_fkey == parentPageKey)
                                    .Where(filter)
                                    .OrderBy(sort)
                                    .ThenBy(p => p.title)
                                    .ToList()
                                    .Where(p => p.Page1 != null)
                                    .OrderBy(sort)
                                    .AsQueryable()
                                    .Select(p => new NavigationItem
                                    {
                                        Title = p.title,
                                        CssClass = cssClass,
                                        Description = p.metadescription,
                                        ImageUrl = p.thumbnailimage,
                                        PageKey = p.page_key,
                                        Url = BuildUrl(p)
                                    });
        }

        internal IList<NavigationItem> GetBreadcrumbNavigationForPage(int pageKey)
        {
            var page = _pageRepository.Get(pageKey);
            var navParts = new List<NavigationItem>();
            GetNavParts(page, navParts);
            navParts.Reverse();
            return navParts;
        }

        private void GetNavParts(Page page, List<NavigationItem> navParts)
        {
            navParts.Add(new NavigationItem
                                    {
                                        Title = page.navtitle,
                                        CssClass = "breadcrumbItem",
                                        Description = page.metadescription,
                                        ImageUrl = page.thumbnailimage,
                                        PageKey = page.page_key,
                                        Url = BuildUrl(page)
                                    });
            if (page.Page1 != null)
            {
                GetNavParts(page.Page1, navParts);
            }
        }
    }
}
