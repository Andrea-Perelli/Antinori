using Antinori.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Antinori.Controllers {
    public class BookController : ApplicationController {

        [Authorize(Roles = "Admin")]
        public JsonResult Add() {
            // return the add book page.

            Books book = new Books();
            // return the partial view containing the P_Create page.
            return Json(GetRenderPartialView(this, "UC_Book", book), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string id) {
            // delete a book by id.

            // set default value for esito.
            bool esito = false;

            // delete book.
            esito = this.Dc.Books_Delete(this.Dc.Books_Get(id)) > -1;
           
            //save and update the esito value.
            return Json(esito, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult Edit(string id) {
            // retrieve the book.
            var book = this.Dc.Books_Get(id);

            // return the partial view containing the user.
            return Json(GetRenderPartialView(this, "UC_Book", book), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult GetBooksInfo() {
            // return user info.

            // create a model. return the model.
            DashboardModelBooks model = new DashboardModelBooks();
            model.numberOfBooks = this.Dc.Books_Gets().Count;
            // return the partial view .
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Index() {
            // return the opere list view.           

            return View();
        }

        [Authorize(Roles = "Admin, Editor")]
        public JsonResult List() {
            // return the list of the users.

            List<Books> books = this.Dc.Books_Gets();
            // return the partial view containing all the users.
            return Json(GetRenderPartialView(this, "UC_Books_List", books), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult P_Create() {
            // open p create page.
            Books s = new Books();

            return View(s);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult P_Sections(int numberOfSections) {
            // return the section page.

            List<Sections> Sections = new List<Sections>(numberOfSections);
            for(int i = 0; i < numberOfSections; i++) {
                Sections.Add(new Sections());
            }

            // return the partial view containing the P_Create page.
            return Json(GetRenderPartialView(this, "UC_Section", Sections), JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin,Editor")]
        public JsonResult P_SubSections(int numberOfSubSections, string parent) {
            // return the section page.

            List<SubSections> subSections = new List<SubSections>(numberOfSubSections);
            for(int i = 0; i < numberOfSubSections; i++) {
                subSections.Add(new SubSections());
            }
            ViewBag.Parent = parent.Substring("subSectionBody".Length);

            // return the partial view containing the P_Create page.
            return Json(GetRenderPartialView(this, "UC_SubSection", subSections), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Editor")]
        public ActionResult P_SubSectionsList() {
            // return the subsection section page.

            // return the view.
            return View();
        }


        [Authorize(Roles = "Admin,Editor")]
        public JsonResult P_SubSectionsListContent() {
            // return the Subsection list page.

            List<SubSections> subSections = this.Dc.SubSections_Gets();

            // return the partial view containing the P_Create page.
            return Json(GetRenderPartialView(this, "UC_SubSectionListContent", subSections), JsonRequestBehavior.AllowGet);
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
                Books newBook = new Books {
                    Id = Guid.NewGuid().ToString(),
                    Author = book.Author,
                    Description = book.Description,
                    EndDate = book.EndDate,
                    Title = book.Title
                };
                int numberOfSection = Convert.ToInt16(forms["numberOfSection"]);
                if(numberOfSection > 0) {
                    // create sections.
                    int index = 0;
                    while(index < numberOfSection) {
                        Sections sec = new Sections {
                            Id = Guid.NewGuid().ToString(),
                            Name = forms["name" + index],
                            Description = forms["description" + index],
                            Book = newBook.Id
                        };
                        int numberOfSubSection = Convert.ToInt16(forms["numberOfSubSection" + index]);
                        int index2 = 0;
                        while(index2 < numberOfSubSection) {
                            SubSections sub = new SubSections {
                                Id = Guid.NewGuid().ToString(),
                                Name = forms[index + "subName" + index2],
                                Description = forms[index + "subDescription" + index2],
                                Section = sec.Id,                                
                            };
                            // add to the section.
                            sec.SubSections.Add(sub);
                            index2++;
                        }                        

                        // add to the book.
                        newBook.Sections.Add(sec);
                        index += 1;
                    }
                }
                // add the sezione to the context.
                this.Dc.Books_Insert(newBook);


                // save context.
                int esito = this.Dc.Books_Save();
                // check if all is ok.
                if(esito > -1) {
                    Log_Insert(newBook.Id, "BOOKS", "INSERT", true, "Operazione conclusa con successo", "", "", "", "");
                    op = new OpEsitoModel() { idReturn = newBook.Id, riuscita = true };
                    return Json(op, JsonRequestBehavior.AllowGet);
                }
                else {
                    Log_Insert(newBook.Id, "BOOKS", "INSERT", false, "Operazione conclusa con successo", "", "", "", "");
                    op = new OpEsitoModel() { idReturn = newBook.Id, riuscita = false };
                    return Json(op, JsonRequestBehavior.AllowGet);
                }
                
            }
        }

    }
}