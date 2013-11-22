using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO
{
    public class KeyValueDto
    {
        public KeyValueDto()
        {

        }

        public KeyValueDto(int id, string title, string value)
        {
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            Id = id;
            Title = textInfo.ToTitleCase(title);
            Value = value.StartsWith("http") ? value : textInfo.ToTitleCase(value);
        }
        public string Title { get; set; }
        public int Id { get; set; }
        public string Value { get; set; }
    }

    
}
