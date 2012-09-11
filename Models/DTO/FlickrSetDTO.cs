using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO
{
    public class FlickrSetDto
    {
        public string Id { get; set; }
        public string Title { get; set; }



        public string Description { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        public FlickrPhotoDto Thumbnail { get; set; }
    }
}
