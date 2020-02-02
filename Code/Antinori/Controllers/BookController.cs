using Antinori.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Antinori.Controllers {
    public class BookController : ApplicationController {

        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Index() {
            // return the opere list view.           

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult P_Create() {
            Books s = new Books();

            return View(s);
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpPost]
        public JsonResult Save(Books book, FormCollection forms) {
            // save. 
            OpEsitoModel op;

            if(book.Id != null) {
                // we are editing user.
                Books daDb = this.Dc.Books_Get(book.Id);

                //map the two objects: it updates the DB object.
                if(TryUpdateModel(daDb)) {
                    try {
                        // set again attivo field (for some reason is set always to to true).
                        //daDb.LockoutEnabled = !forms["Attivo"].ToString().ToLower().Equals("true");
                        //daDb.LockoutEndDateUtc = DateTime.MaxValue;

                        // delete roles
                        //daDb.AspNetRoles.Clear();
                        //this.Dc.AspNetUsers_Save();
                        ////update (add) roles.
                        //foreach(var key in forms) {
                        //    if(key.ToString().Equals("Role")) {
                        //        // add roles: the entity framework will create the relationship.
                        //        daDb.AspNetRoles.Add(this.Dc.Ruoli_Get(forms[key.ToString()].Replace("R_", "")));
                        //    }
                        //}
                        //this.Dc.AspNetUsers_Save();

                        // set log.
                        Log_Insert(book.Id, "Books", "UPDATE", true, "Operazione conclusa con successo", "", "", "", "");
                        op = new OpEsitoModel() { idReturn = book.Id, riuscita = true };
                        return Json(op, JsonRequestBehavior.AllowGet);
                    }
                    catch(Exception ex) {
                        Log_Insert(book.Id, "Books", "UPDATE", false, "Errore:" + ex.Message);
                        op = new OpEsitoModel() { idReturn = "", riuscita = false, msg = ex.Message };
                        return Json(op, JsonRequestBehavior.AllowGet);
                    }
                }
                // I couldn't map the object.
                Log_Insert(book.Id, "Books", "UPDATE", false, "Errore mappatura su DB");
                op = new OpEsitoModel() { idReturn = "", riuscita = false, msg = "Errore mappatura su DB" };
                return Json(op, JsonRequestBehavior.AllowGet);
            }
            // if we are creating a new object.
            else {              
                 // create Book.
                
            }
        }

    }
}