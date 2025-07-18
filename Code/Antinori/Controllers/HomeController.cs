﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Antinori.Controllers {

    public class HomeController : ApplicationController {

        [AllowAnonymous]
        public ActionResult Progetto() {
            // return public P_ContestoProgetto page.
            return View("P_Progetto");
        }

        [AllowAnonymous]
        public ActionResult Home() {
            // return public home page.
            ViewBag.numberOfPages = this.Dc.Pages_Count();
            ViewBag.numberOfBooks = this.Dc.Books_Count();
            ViewBag.numberOfNormalUsers = this.Dc.AspNetUsers_Get_ByRole("User").Count;
            ViewBag.numberOfTranscription = this.Dc.Transcriptions_Count() + 53;

            // retrieve homepage image.
            ViewBag.page = this.Dc.FrontSlider_GetFromDate();

            return View();
        }

        [AllowAnonymous]
        public ActionResult Supporto() {
            // return public P_ContestoProgetto page.
            return View("P_Supporto");
        }

        [Authorize(Roles = "Admin,Editor")]
        public ActionResult Index() {
            // internal homepage.           
            return View();
        }

        [AllowAnonymous]
        [Route("home/consultazione/presentazione")]
        public ActionResult ConsultazionePresentazione() {
            // set the view. 

            return View("Consultazione/Presentazione");
        }

        [AllowAnonymous]
        [Route("home/consultazione/risorse")]
        public ActionResult AccedereAllaRisorsa() {
            // set the view. 

            return View("Consultazione/Risorsa");
        }

        [AllowAnonymous]
        [Route("home/consultazione/struttura")]
        public ActionResult StrutturaDeiManoscritti() {
            // set the view. 

            return View("Consultazione/Struttura");
        }

        [AllowAnonymous]
        [Route("home/consultazione/ricerca")]
        public ActionResult RicercaDegliArgomentiEDeiFacsimili() {
            // set the view. 

            return View("Consultazione/Ricerca");
        }

        [AllowAnonymous]
        [Route("home/consultazione/restauro")]
        public ActionResult RestauroMicrofileCopieAnastatiche() {
            // set the view. 

            return View("Consultazione/Restauro");
        }

        [AllowAnonymous]
        [Route("home/trascrizione/presentazione")]
        public ActionResult TrascrizionePresentazione() {
            // set the view. 

            return View("trascrizione/Presentazione");
        }

        [AllowAnonymous]
        [Route("home/trascrizione/criteri")]
        public ActionResult TrascrizioneCriteri() {
            // set the view. 

            return View("trascrizione/Criteri");
        }

        [AllowAnonymous]
        [Route("home/trascrizione/edizioni")]
        public ActionResult TrascrizioneEdizioni() {
            // set the view. 

            return View("trascrizione/Edizioni");
        }

        [AllowAnonymous]
        [Route("home/trascrizione/codifica/introduzione")]
        public ActionResult TrascrizioneCodificaIntroduzione() {
            // set the view. 

            return View("trascrizione/codifica/introduzione");
        }

        [AllowAnonymous]
        [Route("home/trascrizione/codifica/modello-codifica")]
        public ActionResult TrascrizioneCodificaModello() {
            // set the view. 

            return View("trascrizione/codifica/modello");
        }

        [AllowAnonymous]
        [Route("home/trascrizione/codifica/marcatori")]
        public ActionResult TrascrizioneCodificaMarcatori() {
            // set the view. 

            return View("trascrizione/codifica/marcatori");
        }

        [AllowAnonymous]
        [Route("home/informazioni/biografia")]
        public ActionResult InformazioniBiografia() {
            // set the view. 

            return View("informazioni/biografia");
        }

        [AllowAnonymous]
        [Route("home/informazioni/corpus")]
        public ActionResult InformazioniCorpus() {
            // set the view. 

            return View("informazioni/corpus");
        }

        [AllowAnonymous]
        [Route("home/informazioni/altri-manoscritti")]
        public ActionResult InformazioniAltriManoscritti() {
            // set the view. 

            return View("informazioni/manoscritti");
        }

        [AllowAnonymous]
        [Route("home/informazioni/curiosita")]
        public ActionResult InformazioniNomeAntinori() {
            // set the view. 

            return View("informazioni/antinori");
        }

        [AllowAnonymous]
        [Route("home/sommario")]
        public ActionResult Sommario() {
            // set the view. 

            return View("sommario");
        }

        [AllowAnonymous]
        public ActionResult Privacy() {
            // set the privacy view. 

            return View();
        }

        [AllowAnonymous]
        public ActionResult Cookie() {
            // set the Cookie view. 

            return View();
        }

    }
}