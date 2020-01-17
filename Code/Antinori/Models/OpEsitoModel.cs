using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Antinori.Models {

    public class OpEsitoModel {
        // operation result esito model.
        public bool riuscita { get; set; }
        public string idReturn { get; set; }
        public string msg { get; set; }
    }
}