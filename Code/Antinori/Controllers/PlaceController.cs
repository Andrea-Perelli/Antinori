using Antinori.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Antinori.Controllers {
    public class PlaceController : ApplicationController {

        [Authorize(Roles = "Admin")]
        public JsonResult Add() {
            // return the add book page.

            Places place = new Places();

            return Json(GetRenderPartialView(this, "UC_Place", place), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetMap() {
            // get map.
            List<Places> places = this.Dc.Places_Gets();
            return Json(GetRenderPartialView(this, "UC_Map", places), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string id) {
            // delete a book by id.

            // set default value for esito.
            bool esito = false;

            // delete place.
            Places p = this.Dc.Places_Get(id);
            p.Deleted = true;
            esito = this.Dc.Places_Save() > -1;

            Log_Insert(p.Id, "PLACE", "DELETE", false, "Errore nella cancellazione");

            //save and update the esito value.
            return Json(esito, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult Edit(string id) {
            // retrieve the place.
            Places p = this.Dc.Places_Get(id);

            // return the partial view.
            return Json(GetRenderPartialView(this, "UC_Place", p), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult GetPlaceNumber() {

            return Json(new { numberOfPlaces = this.Dc.Places_Count()}, JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Editor")]
        public JsonResult List() {
            // return the list of the users.

            List<Places> places = this.Dc.Places_Gets();
            return Json(GetRenderPartialView(this, "UC_Places_List", places), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult P_Create() {
            // open p create page.
            Places p = new Places();

            return View(p);
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpPost]
        public JsonResult Save(Places place, FormCollection forms, HttpPostedFileBase PreviewImagePath) {
            // save. 
            OpEsitoModel op;
            try {
                if (ModelState.IsValid) {
                    if(place.Id != null && place.Id.Length > 0) {
                        // edit case.
                        Places fromDB = this.Dc.Places_Get(place.Id);
                        if(TryUpdateModel(fromDB)){
                            // ok.
                            Log_Insert(place.Id, "PLACE", "UPDATE", true, "Operazione conclusa con successo", "", "", "", "");
                            op = new OpEsitoModel() { idReturn = place.Id, riuscita = true };
                            return Json(op, JsonRequestBehavior.AllowGet);
                        }
                        // not ok
                        Log_Insert(place.Id, "PLACE", "UPDATE", true, "Operazione non conclusa", "", "", "", "");
                        op = new OpEsitoModel() { idReturn = place.Id, riuscita = false };
                        return Json(op, JsonRequestBehavior.AllowGet);
                    }
                    else {
                        // creation.
                        place.Id = Guid.NewGuid().ToString();
                        place.Deleted = false;
                        if(this.Dc.Places_Insert(place) > -1) {
                            // ok.
                            Log_Insert(place.Id, "PLACE", "INSERT", true, "Operazione conclusa con successo", "", "", "", "");
                            op = new OpEsitoModel() { idReturn = place.Id, riuscita = true };
                            return Json(op, JsonRequestBehavior.AllowGet);
                        }
                        // not ok
                        Log_Insert(place.Id, "PLACE", "INSERT", true, "Operazione non conclusa", "", "", "", "");
                        op = new OpEsitoModel() { idReturn = place.Id, riuscita = false };
                        return Json(op, JsonRequestBehavior.AllowGet);
                    }
                }
                
            }
            catch(Exception ex) {
                // not ok
                string placeId = "";
                if(place.Id != null) {
                    placeId = place.Id;
                }
                Log_Insert(placeId, "PLACE", "INSERT", true, "Operazione non conclusa" + ex.Message, "", "", "", "");
                op = new OpEsitoModel() { idReturn = place.Id, riuscita = false };
                return Json(op, JsonRequestBehavior.AllowGet);
            }
            
            return null;
        }
    }
}