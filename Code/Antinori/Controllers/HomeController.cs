using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Antinori.Controllers {

    public class HomeController : ApplicationController {

        [AllowAnonymous]
        public ActionResult ContestoProgetto() {

            // return public home page.
            return View("P_ContestoProgetto");
        }

        [AllowAnonymous]
        public ActionResult Home() {

            // return public home page.
            return View();
        }

        [Authorize(Roles = "Admin,Editor")]
        public ActionResult Index() {
            // internal homepage.
            return View();
        }

        [AllowAnonymous]
        public ActionResult P_GuidaFiltri() {
            // set the guida Filtri view. 

            return View();
        }

        [AllowAnonymous]
        public ActionResult P_GuidaTEI() {
            // set the guida TEI view. 

            return View();
        }

        [AllowAnonymous]
        public ActionResult Privacy() {
            // set the privacy view. 

            return View();
        }

    }
}