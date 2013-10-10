using System.Configuration;
using System.Linq;
using System.Web;
using mjjames.DataEntities;
using mjjames.DataContexts;
using System;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DataEntities;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories
{

    public class ProjectRepository : IRepository<Project>
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
        /// Get all Projects
        /// </summary>	
        /// <returns>Projects</returns>
        public IQueryable<Project> FindAll()
        {
            return from p in _dc.Projects
                   where p.site_fkey == Site.Key
                   orderby p.start_date descending , p.end_date descending 
                   select p;

        }

        /// <summary>
        /// Get all Active Projects
        /// </summary>
        /// <returns>Active Projects</returns>
        public IQueryable<Project> FindAllActive()
        {
            return FindAll().Where(p => p.active);
        }

        /// <summary>
        /// Get Project From Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Project Get(int key)
        {
            //for key we don't need to check the site key
            return FindAll().SingleOrDefault(p => p.project_key == key);
        }

        public Project GetProjectFromUrlAndYear(string url, int year)
        {
            return FindAll().FirstOrDefault(p => p.url.Equals(url) && (p.start_date >= new DateTime(year, 1, 1) && p.start_date <= new DateTime(year, 12, 31)));
        }


        public Project Get(string id)
        {
            return FindAll().FirstOrDefault(p => p.project_key == int.Parse(id));
        }
    }
}
