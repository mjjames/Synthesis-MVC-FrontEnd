using System.Configuration;
using System.Linq;
using System.Web;
using mjjames.DataEntities;
using mjjames.DataContexts;
using System;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories
{
	internal class PageRepository : IRepository<Page>
	{
		private mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Site _site;

		/// <summary>
		/// Site
		/// </summary>
		private mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Site Site
		{
			get
			{
				//if we have already loaded the site return it
				if (_site == null)
				{
					_site = new mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Site();
				}
				return _site;
			}
		}

		private readonly CMSDataContext _dc = new CMSDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
		//Query Methods
		
		/// <summary>
		/// Get all Pages
		/// </summary>	
		/// <returns>Pages</returns>
		public IQueryable<Page> FindAll()
		{
			return _dc.Pages.Where(p => p.site_fkey.Equals(Site.Key));
		}

		/// <summary>
		/// Get all Active PAges
		/// </summary>
		/// <returns>Active Pages</returns>
		public IQueryable<Page> FindAllActive()
		{
			return from p in _dc.Pages
				   where p.active == true
				   && p.site_fkey.Equals(Site.Key)
				   select p;
		}

		/// <summary>
		/// Get Page From Page Key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Page Get(int key)
		{
			//for key we don't need to check the site key
			return _dc.Pages.SingleOrDefault(p => p.page_key == key);
		}

		public Page Get(string id)
		{
			return _dc.Pages.SingleOrDefault(p => p.pageid.ToLower() == id.ToLower() && p.site_fkey.Equals(Site.Key));
		}

		public Page GetPageFromUrl(string url)
		{
			
			//if we don't start with a slash add one
			if(!url.StartsWith("/"))
			{
				url = "/" + url;
			}

            //func that says how to find our page
			Func<Page, bool> urlFinder;
			//our page url part is our last item after splitting on /
			var pageUrlParts = url.Split(new[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries);

			//if the url is just / then we are actually looking for a home page
			if (url == "/")
			{
                //for the home page search for the page with the HOME Page ID
                urlFinder = p => p.pageid.Equals("HOME", StringComparison.InvariantCultureIgnoreCase) && p.site_fkey.Equals(Site.Key);
            }
			//otherwise we need to do some real work
			else
			{
				//we need to then find a page that matches this url part
				urlFinder = p => p.page_url.Equals(pageUrlParts.Last()) && p.site_fkey.Equals(Site.Key);
			}
			//find any pages that match our url part 
            var pages = _dc.Pages.Where(urlFinder);
			//if we dont have any pages then return null
			if(!pages.Any()){
				return null;
			}
			//now if we have multiple pages we have to find an exact match
			//other wise just return the page
			if (pages.Count() == 1)
			{
				return pages.First();
			}

			//we are going to be a bit nieve and say that if up to 3 urls match then we have a match
			var counter = 0;
			//loop until we hit our page parts limit or 3
			while(counter < pageUrlParts.Length && counter < 3){
				//simply overwrite our pages collection with the results of the filter
				pages = pages.Where(p => p.page_url == pageUrlParts[pageUrlParts.Length - counter]);
			}
			//we should now just have one page so return the first
			return pages.FirstOrDefault();
		}
	}
}
