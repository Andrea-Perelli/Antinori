using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Antinori.Models {


    public class DashboardModelUsers {

        // dashboard model.
        public int numberOfUsers { get; set; }
        public int numberOfNormalUsers { get; set; }
        
    }

    public class DashboardModelBooks{

        // dashboard model.
        public int numberOfBooks { get; set; }
        public int numberOfTranscriptionsToCheck { get; set; }
    }
}