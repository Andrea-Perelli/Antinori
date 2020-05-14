using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Antinori.Static {
    public class BL_Utility {

        public static string[] professions {
            get {
                string[] professions = new string[]{
                    "Altro", 
                    "Insegnante",
                    "Insegnante in pensione",
                    "Studente"
                };
                return professions;
            }
        }
    }
}