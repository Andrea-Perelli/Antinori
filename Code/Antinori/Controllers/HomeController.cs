using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Antinori.Controllers {

    public class HomeController : ApplicationController {
       [Authorize]
        public ActionResult Index() {            
            return View();
        }

    }
}