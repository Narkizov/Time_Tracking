using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Time_Tracking.Models;

namespace Time_Tracking.ModelsView
{
    public class IndexModelUserPerson
    {
        public List<User> Users { get; set; }
        public List<Report> Reports { get; set; }
    }
}
