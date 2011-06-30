using System.Configuration;
using System.Linq;
using mjjames.DataContexts;
using mjjames.DataEntities;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories
{
	public class ArticleRepository : IRepository<Article>
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

		public System.Linq.IQueryable<Article> FindAll()
		{
			return _dc.Articles.Where(a => a.site_fkey.Equals(Site.Key));
		}

		public System.Linq.IQueryable<Article> FindAllActive()
		{
			return _dc.Articles.Where(a => a.site_fkey.Equals(Site.Key) && a.active);
		}

		public Article Get(int key)
		{
			return _dc.Articles.FirstOrDefault(a => a.article_key == key);
		}

		public Article Get(string url)
		{
			return _dc.Articles.FirstOrDefault(a => a.url.EndsWith(url));
		}
	}
}
