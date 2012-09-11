using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO
{
    public class FlickrPhotoDto
    {
        public string ThumbnailUrl { get; set; }

        public string SquareThumbnailUrl { get; set; }

        public string MediumUrl { get; set; }

        public string LargeUrl { get; set; }

        public string SmallLarge { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Id { get; set; }
    }
}
