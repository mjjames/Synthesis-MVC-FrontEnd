using mjjames.DataContexts;
using mjjames.DataEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories
{
    public class KeyValueRepository : IRepository<KeyValue>
    {
      
        private readonly CMSDataContext _dc = new CMSDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);

        public IQueryable<KeyValue> FindAll()
        {
            return _dc.KeyValues;
        }

        public IQueryable<KeyValue> FindAllActive()
        {
            return _dc.KeyValues.Where(kv => kv.lookup.active && kv.Lookup1.active);
        }

        public KeyValue Get(int key)
        {
            return _dc.KeyValues.FirstOrDefault(kv => kv.keyvalue_key == key);
        }

        [Obsolete]
        public KeyValue Get(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find all the key values for the provdied link key and link key type
        /// </summary>
        /// <param name="linkKey"></param>
        /// <param name="keyLookupId"></param>
        /// <returns></returns>
        public IQueryable<KeyValue> ByLink(int linkKey, string keyLookupId)
        {
            return _dc.KeyValues.Where(kv => kv.link_fkey == linkKey && kv.Lookup1.lookup_id == keyLookupId);
        }
    }
}
