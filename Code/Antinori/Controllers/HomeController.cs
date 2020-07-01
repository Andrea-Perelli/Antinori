using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Antinori.Controllers {

    public class HomeController : ApplicationController {

        [AllowAnonymous]
        public ActionResult ChiSiamo() {
            // return public P_ContestoProgetto page.
            return View("P_ChiSiamo");
        }

        [AllowAnonymous]
        public ActionResult Home() {
            // return public home page.
            ViewBag.numberOfPages = this.Dc.Pages_Count();
            ViewBag.numberOfBooks = this.Dc.Books_Count();
            ViewBag.numberOfNormalUsers = this.Dc.AspNetUsers_Get_ByRole("User").Count + 15;
            ViewBag.numberOfTranscription = this.Dc.Transcriptions_Count() + 53;

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