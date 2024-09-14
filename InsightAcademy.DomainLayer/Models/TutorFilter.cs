using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Models
{
    public class TutorFilter
    {
        public Guid Category { get; set; }
        public string Tags { get; set; }
        public string CurrentUserCountryId { get; set; }
        public bool IsLocal { get; set; }
        public string Subject { get; set; }
        public string Education { get; set; }
        public string Order { get; set; }
        public string Location { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string service { get; set; }
        public string Photo { get; set; }
        public string Gender { get; set; }
        public int Rating { get; set; }

        public int FilterType { get; set; } /*1=List,2=Grid*/

        public List<Guid> SelectedCategories { get; set; } = new List<Guid>();
        public List<double> SelectedRating { get; set; } = new List<double>();
    }
}