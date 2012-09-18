using mjjames.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DataEntities
{
    /// <summary>
    /// A Project with all its key value pairs
    /// </summary>
    public class KeyValuedProject : Project
    {
        public KeyValuedProject(Project project)
        {
            this.active = project.active;
            this.description = project.description;
            this.end_date = project.end_date;
            this.include_in_rss = project.include_in_rss;
            this.photogallery_id = project.photogallery_id;
            this.project_key = project.project_key;
            this.Site = project.Site;
            this.site_fkey = project.site_fkey;
            this.start_date = project.start_date;
            this.Testimonies = project.Testimonies;
            this.title = project.title;
            this.url = project.url;
            this.video_id = project.video_id;
        }

        public IQueryable<KeyValue> KeyValues { get; set; }
    }
}
