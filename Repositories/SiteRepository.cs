using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mjjames.DataEntities;
using mjjames.DataContexts;
using System.Configuration;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories
{
	internal class SiteRepository : IRepository<Site>
	{
		private readonly CMSDataContext _dc = new CMSDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
		

		public IQueryable<Site> FindAll()
		{
			return _dc.Sites;
		}

		public IQueryable<Site> FindAllActive()
		{
			return FindAll().Where(s => s.active);
		}

		public Site Get(int key)
		{
			return _dc.Sites.Where(s => s.site_key == key).FirstOrDefault();
		}

		public Site Get(string id)
		{
			return _dc.Sites.Where(s => s.hostname.Equals(id)).FirstOrDefault();
		}
	}
}
