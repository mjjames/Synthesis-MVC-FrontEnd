using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mjjames.DataEntities;
using mjjames.DataContexts;
using System.Configuration;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories
{
	class MediaRepository : IRepository<Media>
	{
		private readonly CMSDataContext _dc = new CMSDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);

		public IQueryable<Media> FindAll()
		{
			return from m in _dc.Medias
				   select m;
		}

		public IQueryable<Media> FindAllActive()
		{
			return from m in _dc.Medias
				   where m.active
				   select m;
		}

		public Media Get(int key)
		{
			return (from m in _dc.Medias
					where m.media_key == key && m.active
					select m).FirstOrDefault();
		}

		[Obsolete]
		public Media Get(string id)
		{
			throw new NotImplementedException();
		}

		public IQueryable<Media> GetByLookupID(string lookupID)
		{
			return from m in _dc.Medias
				   where m.Lookup.lookup_id == lookupID
                   && m.active
				   select m;
		}

		public IQueryable<Media> GetByMediaLinkType(string mediaLinkID)
		{
			return from ml in _dc.MediaLinks
					where ml.Lookup.lookup_id == mediaLinkID
                    && ml.Media.active
					select ml.Media;
		}
	}
}
