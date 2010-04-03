using System.Configuration;
using System.Linq;
using System.Web;
using mjjames.DataEntities;
using mjjames.DataContexts;

namespace mjjames.Models
{
	public class PageRepository
	{
		private readonly CMSDataContext _dc = new CMSDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
		//Query Methods
		
		/// <summary>
		/// Get all Pages
		/// </summary>	
		/// <returns>Pages</returns>
		public IQueryable<Page> FindAllPages()
		{
			return _dc.Pages;
		}

		/// <summary>
		/// Get all Active PAges
		/// </summary>
		/// <returns>Active Pages</returns>
		public IQueryable<Page> FindAllActivePages()
		{
			return from p in _dc.Pages
				   where p.active == true
				   select p;
		}

		/// <summary>
		/// Get Page From Page Key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Page GetPage(int key)
		{
			return _dc.Pages.SingleOrDefault(p => p.page_key == key);
		}

		public Page GetPage(string id)
		{
			return _dc.Pages.SingleOrDefault(p => p.pageid.ToLower() == id.ToLower());
		}

		public Page GetPageFromUrl(string url)
		{
			
			//if we don't start with a slash add one
			if(!url.StartsWith("/"))
			{
				url = "/" + url;
			}

			//next query our default sitemap provider for a page with this url
			var node = SiteMap.Provider.FindSiteMapNode(url);
			//if found use it's key to call the GetPage method, else return null
			return node != null ? GetPage(int.Parse(node.Key)) : null;
		}
	}
}
