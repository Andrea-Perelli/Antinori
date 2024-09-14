using Antinori.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using System.Configuration;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace Antinori.Controllers {
    public class BookController : ApplicationController {

        // number of pages to show inside read opera page.
        //private double numberOfPageToShow = 9.0;       

        [Authorize(Roles = "Admin")]
        public JsonResult Add() {
            // return the add book page.

            Books book = new Books();

            
            // return the partial view containing the P_Create page.
            return Json(GetRenderPartialView(this, "UC_Book", book), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult AddPage(Pages page, FormCollection forms, HttpPostedFileBase PhotoPath, HttpPostedFileBase BigPhotoPath) {
            // add a page to a section.

            // set default value for esito.
            bool result = false;

            string relativePathJpeg = "~/Pics/JPEG/";
            string relativePathTiff = "~/Pics/Tiff/";

            string diskPathJpeg = ControllerContext.HttpContext.Server.MapPath(relativePathJpeg) + PhotoPath.FileName;
            string diskPathTiff = ControllerContext.HttpContext.Server.MapPath(relativePathTiff) + BigPhotoPath.FileName;

            try {
                // upload photos.
                PhotoPath.SaveAs(diskPathJpeg);
                BigPhotoPath.SaveAs(diskPathTiff);

                SubSections sec = this.Dc.SubSectionss_Get(page.SubSection);

                // set data
                page.SectionName = sec.Sections.Name;
                page.BookTitle = sec.Sections.Books.Title;
                page.Id = Guid.NewGuid().ToString();
                page.PhotoPath = diskPathJpeg;
                page.BigPhotoPath = diskPathTiff;

                // insert page.
                result = this.Dc.Pages_Insert(page) > -1;

                // if we have filters.
                if(forms["Filters"] != null) {
                    string[] filters = forms["Filters"].TrimStart(';').TrimEnd(';').Split(';');
                    foreach(string f in filters) {
                        // add filter.
                        Filters filter = new Filters {
                            Id = Guid.NewGuid().ToString(),
                            Name = f.Trim(),
                            Page = page.Id
                        };
                        if(this.Dc.Filters_Insert(filter) == -1) {
                            // set log and return
                            Log_Insert(page.Id, "Filters", "INSERT", false, "Errore nell'inserimento dell'entità.");
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                // all ok.
                if(result) {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }                
            }
            catch(Exception e) {

            }
            Log_Insert(page.SectionName, "Books", "UPDATE", false, "Errore nel salvataggio");
            OpEsitoModel op = new OpEsitoModel() { idReturn = "", riuscita = false, msg = "Errore nell'inserimento di una pagina." };
            return Json(op, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string id) {
            // delete a book by id.

            // set default value for esito.
            bool esito = false;

            // delete book.
            Books book = this.Dc.Books_Get(id);
            esito = this.Dc.Books_Delete(book) > -1;
           
            // Delete all photos of a book.
            foreach(Sections s in book.Sections) {
                foreach(SubSections ss in s.SubSections) {
                    foreach(Pages p in ss.Pages) {
                        // delete photo.
                        try {
                            // small photo.
                            if(System.IO.File.Exists(p.PhotoPath)) {
                                System.IO.File.Delete(p.PhotoPath);
                            }
                            // big photo.
                            if(System.IO.File.Exists(p.BigPhotoPath)) {
                                System.IO.File.Delete(p.BigPhotoPath);
                            }
                        }
                        catch(Exception e) {
                            Log_Insert(p.Id, "BOOK", "DELETE FILE", false, "Errore nella cancellazione del file: " + e.Message);
                        }
                    }
                }
            }

            //save and update the esito value.
            return Json(esito, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult DeleteAttachment(string id) {
            // delete an attachment by id.

            // set default value for esito.
            bool esito = false;

            // attachment.
            Attachments a = this.Dc.Attachments_Get(id);
            esito = this.Dc.Attachments_Delete(a) > -1;

            try {
                // delete file.
                if(System.IO.File.Exists(a.PhotoPath)) {
                    System.IO.File.Delete(a.PhotoPath);
                }
            }
            catch (Exception e) {
                Log_Insert(a.Id, "Attachments", "DELETE FILE", false, "Errore nella cancellazione del file: " + e.Message );
            }

            //save and update the esito value.
            return Json(esito, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult DeletePage(string id) {
            // delete a page     by id.

            // set default value for esito.
            bool esito = false;

            // page book.
            Pages p = this.Dc.Pages_Get(id);
            esito = this.Dc.Pages_Delete(p) > -1;

            try {
                // small photo.
                if (System.IO.File.Exists(p.PhotoPath)) {
                    System.IO.File.Delete(p.PhotoPath);
                }
                // big photo.
                if (System.IO.File.Exists(p.BigPhotoPath)) {
                    System.IO.File.Delete(p.BigPhotoPath);
                }
            }
            catch (Exception e) {
                Log_Insert(p.Id, "Pages", "DELETE FILE", false, "Errore nella cancellazione del file: " + e.Message);
            }

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

        [AllowAnonymous]
        public ActionResult Fonti() {
            // get the antinori sources.
            // retrieve Antinori.
            Books antinori = this.Dc.Books_GetByTitle("Manoscritti antinoriani");
            // retrieve sources.
            List<Books> books = this.Dc.Books_GetsBySource(antinori.Id);
            return View("P_Sources", books);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult GetBooksInfo() {
            // return user info.

            // create a model. return the model.
            DashboardModelBooks model = new DashboardModelBooks();
            model.numberOfBooks = this.Dc.Books_Gets().Count;
            model.numberOfTranscriptionsToCheck = this.Dc.Transcriptions_GetAllNotApproved().Count;
            // return the partial view .
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetFirstSubsectionInfo(string sectionId) {
            // return first subsection info.

            SubSections s = this.Dc.Sections_GetFirstBySectionId(sectionId);

            // return the partial view .
            return Json(new { id = s.Id, name = s.Name }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Index() {
            // return the opere list view.           

            return View(this.Dc.Books_Gets());
        }

        [Authorize(Roles = "Admin, Editor")]
        public JsonResult List() {
            // return the list of the users.

            List<Books> books = this.Dc.Books_Gets();
            // return the partial view containing all the users.
            return Json(GetRenderPartialView(this, "UC_Books_List", books), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult P_AddAttachment(string pageId) {
            // return the edit Attachments page.

            // create Attachment.
            Attachments a = new Attachments {
                PageId = pageId
            };

            // return the partial view containing the P_Create page.
            return Json(GetRenderPartialView(this, "UC_Attachment", a), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult P_AddPage(string subSectionId) {
            // return the add page view.

            Pages page = new Pages();
            page.SubSection = subSectionId;

            // return the partial view containing the P_AddPage page.
            return Json(GetRenderPartialView(this, "P_AddPage", page), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult P_BookSections(string bookId) {
            // return the Subsection list page.

            List<Sections> sections = this.Dc.Sections_GetsByBookId(bookId);

            // return the partial view containing the BookSections page.
            return Json(GetRenderPartialView(this, "UC_BookSections", sections), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult P_Create() {
            // open p create page.
            Books s = new Books();

            return View(s);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult P_EditAttachment(string id) {
            // return the edit Attachments page.

            // retrieve Attachment.
            Attachments a = this.Dc.Attachments_Get(id);

            // return the partial view containing the P_Create page.
            return Json(GetRenderPartialView(this, "UC_Attachment", a), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult P_EditPage(string id) {
            // return the edit page.

            // retrieve page
            Pages p = this.Dc.Pages_Get(id);
            ViewBag.Filters = this.Dc.Filters_GetByPageId(id);

            ViewBag.ListOfPlaces = this.Dc.Places_Gets();
            // return the partial view containing the P_Create page.
            return Json(GetRenderPartialView(this, "P_AddPage", p), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult P_EditTranscription(string id) {
            // return the edit page.

            // retrieve transcription
            Transcriptions t = this.Dc.Transcriptions_Get(id);

            // return the partial view containing the P_Create page.
            return Json(GetRenderPartialView(this, "UC_Transcription", t), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult  P_PagesByFilters(string filters, string page, int numberOfPageToShow, string selectedSection) {
            // return the subsections page list view.

            // split filters.
            string[] fs = filters.Split(',');

            List<Pages> pages = null;

            if (selectedSection == null || selectedSection == "") {
                // when we clicked on the pagination. 
                pages = this.Dc.Pages_GetFirstNByFilterNameListAndIndex(fs, numberOfPageToShow, Convert.ToInt32(page));
            }
            else {
                // when we clicked on the pagination. 
                pages = this.Dc.Pages_GetFirstNByFilterNameListAndIndexAndSection(fs, numberOfPageToShow, Convert.ToInt32(page), selectedSection);
            }  

            // return the partial view containing the UC_SectionSubsections page.
            return Json(GetRenderPartialView(this, "UC_SubsectionPages", pages), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult P_PagesNumberByFilters(string filters, double numberOfPageToShow, string selectedSection) {
            // return the number of page to show for advanced search.

            int tabs = 0;
            int p = 0;
            // split filters.
            string[] fs = filters.Split(',');
            if(selectedSection == null || selectedSection == "") {
                p = this.Dc.Pages_GetByFilterNameListNumber(fs);
            }
            else {
                p = this.Dc.Pages_GetByFilterNameListNumberAndSection(fs, selectedSection);
            }

            if (p > 0) {
                double temp = ((double)p) / numberOfPageToShow;
                tabs = (int)Math.Ceiling(temp);
            }

            // return the number of pages to show.
            return Json(tabs, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult P_ReadBooks() {
            // set the leggi opera view. 
            // retrieve books.
            List<Books> books = this.Dc.Books_Gets(includeSources: false);
            return View(books);
        }

        [AllowAnonymous]
        public JsonResult P_SearchedPages(int pageNumber) {
            // return the searched pages (by number).

            List<Pages> pages = null;

            // when we clicked on the pagination. 
            pages = this.Dc.Pages_GetByNumberOrderedBySectionName(pageNumber);

            // return the partial view containing the UC_SectionSubsections page.
            return Json(GetRenderPartialView(this, "UC_SubsectionPagesSearched", pages), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult P_SearchedPagesByNumberAndName(int pageNumber, string subSectionName) {
            // return the searched pages (by number) and subsection name.

            subSectionName = subSectionName.Replace("\\", "");

            List<Pages> pages = new List<Pages>();
            Pages p = null;

            try {
                p = this.Dc.Pages_GetBySubSectionNameAndNumber(subSectionName, Convert.ToInt32(pageNumber));
                // if is different from null: add it.
                if(p!= null) {
                    pages.Add(p);
                }
            }
            catch (Exception e) {
                Log_Insert("", "PAGES", "SELECT", false, e.Message);
            }

            // return the partial view containing the UC_SectionSubsections page.
            return Json(GetRenderPartialView(this, "UC_SubsectionPagesSearched", pages), JsonRequestBehavior.AllowGet);
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

        [AllowAnonymous]
        public JsonResult P_SubsectionPages(string subSectionId, string page, int numberOfPageToShow) {
            // return the subsections page list view.

            List<Pages> pages = null;

            // when we clicked on the pagination. 
            pages = this.Dc.Pages_GetFirstNBySubSectionAndIndex(subSectionId, (int)numberOfPageToShow, Convert.ToInt32(page));

            // return the partial view containing the UC_SectionSubsections page.
            return Json(GetRenderPartialView(this, "UC_SubsectionPages", pages), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult P_SubsectionPagesNumber(string subSectionId, double numberOfPageToShow) {
            // return the number of page to show.
            
            int tabs = 0; 

            SubSections s = this.Dc.SubSectionss_Get(subSectionId);

            if(s != null) {
                double temp = ((double)s.Pages.Count) / numberOfPageToShow;
                tabs = (int) Math.Ceiling(temp);
            }

            // return the number of pages to show.
            return Json(tabs, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult P_SectionSubsections(string sectionId) {
            // return the subsections list view.

            List<SubSections> subSections = this.Dc.Sections_GetsBySectionId(sectionId);
            ViewBag.Sections = this.Dc.Sections_GetsNamesByBookId(subSections.First().Sections.Books.Id);
            ViewBag.ShowSearch = subSections.First().Sections.Books.Title != "Manoscritti antinoriani" ? false : true;
            // return the partial view containing the UC_SectionSubsections page.
            return Json(GetRenderPartialView(this, "UC_SectionSubsections", subSections), JsonRequestBehavior.AllowGet);
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
        
        [Authorize(Roles = "Admin,Editor")]
        public ActionResult P_TranscriptionsList() {
            // return the Transcriptions page.

            // return the view.
            return View();
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult P_TranscriptionsListContent() {
            // return the Subsection list page.

            List<Transcriptions> transcriptions = this.Dc.Transcriptions_Gets();

            // return the partial view containing the P_Create page.
            return Json(GetRenderPartialView(this, "UC_TranscriptionsList", transcriptions), JsonRequestBehavior.AllowGet);
        }

        public ActionResult P_TranslatePage(string pageId) {
            // return the p translation page.

            Pages p = this.Dc.Pages_Get(pageId);

            ViewBag.previous = p.SubSections.Pages.FirstOrDefault(page => page.NumericOrder == p.NumericOrder-1);

            ViewBag.after = p.SubSections.Pages.FirstOrDefault(page => page.NumericOrder == p.NumericOrder + 1);


            // return the partial view containing the P_TranslatePage page.
            return View( p);
        }

        [Authorize(Roles = "Admin,Editor,User")]
        public ActionResult P_TranslatePageWithAuth(string pageId) {
            // return the translation page with needed auth.

            Pages p = this.Dc.Pages_Get(pageId);

            ViewBag.previous = p.SubSections.Pages.FirstOrDefault(page => page.NumericOrder == p.NumericOrder - 1);

            ViewBag.after = p.SubSections.Pages.FirstOrDefault(page => page.NumericOrder == p.NumericOrder + 1);

            ViewBag.OxygenUrl = ConfigurationManager.AppSettings["Oxygen_URL"];
            // return the partial view containing the P_TranslatePage page.
            return View("P_TranslatePage", p);
        }       

        [Authorize(Roles = "Admin,Editor,User")]
        public ActionResult P_TranslateSearchedPageWithAuth(string pageNumber, string subsectionId) {
            // return the translation page with needed auth.

            Pages p = new Pages();

            try {
                p = this.Dc.Pages_GetBySubSectionAndNumber(subsectionId, Convert.ToInt32(pageNumber));
                if(p != null) {
                    ViewBag.previous = p.SubSections.Pages.FirstOrDefault(page => page.NumericOrder == p.NumericOrder - 1);
                    ViewBag.after = p.SubSections.Pages.FirstOrDefault(page => page.NumericOrder == p.NumericOrder + 1);
                }
                
            }
            catch(Exception e) {
                Log_Insert("", "PAGES", "SELECT", false, e.Message);
            }
            
            // return the partial view containing the P_TranslatePage page.
            return View("P_TranslatePage", p);
        }

        [Authorize(Roles = "Admin,Editor,User")]
        public JsonResult PageExists(string pageNumber, string subsectionId) {
            // checkif the page exists

            Pages p = new Pages();
            bool res = false;
            try {
                p = this.Dc.Pages_GetBySubSectionAndNumber(subsectionId, Convert.ToInt32(pageNumber));
                if (p != null) {
                    res = true;
                }
            }
            catch (Exception e) {
                Log_Insert("", "PAGES", "SELECT", false, e.Message);
            }
            // return the partial view .
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpPost]
        public JsonResult Save(Books book, FormCollection forms, HttpPostedFileBase PreviewImagePath) {
            // save. 
            OpEsitoModel op;

            if(book.Id != null) {
                // we are editing user.
                Books daDb = this.Dc.Books_Get(book.Id);

                if(PreviewImagePath != null) {
                    string relativePathJpeg = "~/Pics/JPEG/";
                    string diskPathJpeg = ControllerContext.HttpContext.Server.MapPath(relativePathJpeg) + PreviewImagePath.FileName;

                    // delete small photo.
                    if (System.IO.File.Exists(book.PreviewImage)) {
                        System.IO.File.Delete(book.PreviewImage);
                    }
                    // save.
                    PreviewImagePath.SaveAs(diskPathJpeg);
                    daDb.PreviewImage = diskPathJpeg;
                    //this.Dc.Pages_Save();
                }
               

                //map the two objects: it updates the DB object.
                if (TryUpdateModel(daDb)) {
                    try {

                        int numberOfSection = Convert.ToInt16(forms["numberOfSection"]);
                        if(numberOfSection > 0) {
                            // create sections.
                            int index = 0;
                            while(index < numberOfSection) {

                                // retrive section.
                                Sections temp = daDb.Sections.FirstOrDefault(s => s.Id == forms["id" + index].ToString());
                                
                                if(temp!= null) {
                                    temp.Name = forms["name" + index];
                                    temp.Description = forms["description" + index];
                                    temp.Description2 = forms["2description" + index];
                                    temp.Description3 = forms["3description" + index];

                                }

                                // retrieve number of sub sections.
                                int numberOfSubSection = Convert.ToInt16(forms["numberOfSubSection" + index]);

                                int index2 = 0;
                                while(index2 < numberOfSubSection) {

                                    SubSections temp2 = temp.SubSections.FirstOrDefault(s => s.Id == forms[temp.Id + "id" + index2].ToString());
                                    if(temp2 != null) {
                                        temp2.Name = forms[temp.Id + "subName" + index2];
                                        temp2.Description = forms[temp.Id + "subDescription" + index2];
                                        temp2.RopeNumber = Convert.ToInt16(forms[temp.Id + "subRope" + index2]);

                                    }

                                    index2++;
                                }
                                index += 1;
                            }
                            this.Dc.Sections_Save();

                        }
                        // save.
                        int esito = this.Dc.Books_Save();

                        // is we couldn't save.
                        if(esito == -1) {                            

                            Log_Insert(book.Id, "Books", "UPDATE", false, "Errore nel salvataggio");
                            op = new OpEsitoModel() { idReturn = "", riuscita = false, msg = "Errore nel salvataggio" };
                            return Json(op, JsonRequestBehavior.AllowGet);
                        }
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
                            Description2 = forms["2description" + index],
                            Description3 = forms["3description" + index],

                            Book = newBook.Id
                        };
                        int numberOfSubSection = Convert.ToInt16(forms["numberOfSubSection" + index]);
                        int index2 = 0;
                        while(index2 < numberOfSubSection) {
                            SubSections sub = new SubSections {
                                Id = Guid.NewGuid().ToString(),
                                Name = forms[index + "subName" + index2],
                                Description = forms[index + "subDescription" + index2],
                                RopeNumber = Convert.ToInt16(forms[index + "subRope" + index2]),
                                Section = sec.Id                               
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
                    Log_Insert(newBook.Id, "BOOKS", "INSERT", false, "Operazione non conclusa", "", "", "", "");
                    op = new OpEsitoModel() { idReturn = newBook.Id, riuscita = false };
                    return Json(op, JsonRequestBehavior.AllowGet);
                }
                
            }
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpPost]
        public JsonResult SavePage(Pages page, FormCollection forms, HttpPostedFileBase PhotoPath1, HttpPostedFileBase BigPhotoPath1) {
            // save. 
            OpEsitoModel op;

            Pages fromDb = this.Dc.Pages_Get(page.Id);

            // if we have filters.
            if(forms["Filters2"] != null && forms["Filters2"].ToString() != "") {
                // remove items.
                foreach(Filters ff in this.Dc.Filters_GetByPageId(fromDb.Id)) {
                    this.Dc.Filters_Delete(ff);
                }
                fromDb.Filters.Clear();

                string[] filters = forms["Filters2"].TrimStart(';').TrimEnd(';').Split(';');
                foreach(string f in filters) {
                    if(f.Trim() != "") {
                        // add filter.
                        Filters filter = new Filters {
                            Id = Guid.NewGuid().ToString(),
                            Name = f.Trim(),
                            Page = page.Id
                        };
                        fromDb.Filters.Add(filter);
                        if (this.Dc.Filters_Insert(filter) == -1) {
                            // set log and return
                            Log_Insert(page.Id, "Filters", "INSERT", false, "Errore nell'inserimento dell'entità.");
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else {
                            // set log.
                            Log_Insert(page.Id, "Filters", "INSERT", true, "Inserito filtro con nome: " + filter.Name);
                       }
                    }
                    
                }
            }
            
            //map the two objects: it updates the DB object.
            if(TryUpdateModel(fromDb)) {
                try {
                    if(PhotoPath1 != null) {
                        string relativePathJpeg = "~/Pics/JPEG/";
                        string diskPathJpeg = ControllerContext.HttpContext.Server.MapPath(relativePathJpeg) + PhotoPath1.FileName;
                        // if is different.
                        if(diskPathJpeg != page.BigPhotoPath) {
                            // delete small photo.
                            if(System.IO.File.Exists(page.PhotoPath)) {
                                System.IO.File.Delete(page.PhotoPath);
                            }
                            // save.
                            PhotoPath1.SaveAs(diskPathJpeg);
                            fromDb.PhotoPath = diskPathJpeg;
                            this.Dc.Pages_Save();
                        }
                    }
                    if(BigPhotoPath1 != null) {
                        string relativePathTiff = "~/Pics/Tiff/";
                        string diskPathTiff = ControllerContext.HttpContext.Server.MapPath(relativePathTiff) + BigPhotoPath1.FileName;
                        // if is different.
                        if(diskPathTiff != page.BigPhotoPath) {
                            // delete old one.
                            if(System.IO.File.Exists(page.BigPhotoPath)) {
                                System.IO.File.Delete(page.BigPhotoPath);
                            }
                            // save.
                            BigPhotoPath1.SaveAs(diskPathTiff);
                            fromDb.BigPhotoPath = diskPathTiff;
                            this.Dc.Pages_Save();

                        }
                    }

                    // update places.
                    if(forms["selectedPlaces"] != null && forms["selectedPlaces"].ToString() != "") {
                        fromDb.Places.Clear();
                        foreach(string place in forms["selectedPlaces"].ToString().Split(',')) {
                            Places p = this.Dc.Places_GetByName(place);
                            fromDb.Places.Add(p);
                        }
                    }


                    // save.
                    int esito = this.Dc.Pages_Save();

                    // is we couldn't save.
                    if(esito == -1) {

                        Log_Insert(page.Id, "Pages", "UPDATE", false, "Errore nel salvataggio");
                        op = new OpEsitoModel() { idReturn = "", riuscita = false, msg = "Errore nel salvataggio" };
                        return Json(op, JsonRequestBehavior.AllowGet);
                    }
                }
                catch(Exception ex) {
                        Log_Insert(page.Id, "Pages", "UPDATE", false, "Errore:" + ex.Message);
                        op = new OpEsitoModel() { idReturn = "", riuscita = false, msg = ex.Message };
                        return Json(op, JsonRequestBehavior.AllowGet);
                    }
                }
            // set log.
            Log_Insert(page.Id, "Pages", "UPDATE", true, "Operazione conclusa con successo", "", "", "", "");
            op = new OpEsitoModel() { idReturn = page.Id, riuscita = true };
            return Json(op, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpPost]
        public JsonResult SaveTranscription(Transcriptions transcription, FormCollection forms) {
            // save. 
            OpEsitoModel op;


            if (ModelState.IsValid) {
                Transcriptions fromDb = this.Dc.Transcriptions_Get(transcription.Id);

                //map the two objects: it updates the DB object.
                if (TryUpdateModel(fromDb)) {

                        Log_Insert(transcription.Id, "Transcriptions", "UPDATE", false, "Errore nel salvataggio");
                        op = new OpEsitoModel() { idReturn = "", riuscita = false, msg = "Errore nel salvataggio" };
                        return Json(op, JsonRequestBehavior.AllowGet);
                }
            }
           
            // set log.
            Log_Insert(transcription.Id, "Transcriptions", "UPDATE", true, "Operazione conclusa con successo", "", "", "", "");
            op = new OpEsitoModel() { idReturn = transcription.Id, riuscita = true };
            return Json(op, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult SaveAttachment(Attachments attachment, HttpPostedFileBase PhotoPath) {
            // save function,

            OpEsitoModel op;

            if (attachment.Id != null) {
                // edit case.

                Attachments daDb = this.Dc.Attachments_Get(attachment.Id);

                if (PhotoPath != null) {
                    string relativePathJpeg = "~/Attachments/";
                    string dir = "/" + Guid.NewGuid().ToString() + "/";

                    if(!Directory.Exists(ControllerContext.HttpContext.Server.MapPath(relativePathJpeg) + dir)) {
                        Directory.CreateDirectory(ControllerContext.HttpContext.Server.MapPath(relativePathJpeg) + dir);
                    }
                    string diskPathJpeg = ControllerContext.HttpContext.Server.MapPath(relativePathJpeg) + dir + PhotoPath.FileName;

                    // delete small photo.
                    if (System.IO.File.Exists(daDb.PhotoPath)) {
                        System.IO.File.Delete(daDb.PhotoPath);
                    }
                    // save.
                    PhotoPath.SaveAs(diskPathJpeg);
                    daDb.PhotoPath = diskPathJpeg;
                }

                //map the two objects: it updates the DB object.
                if (TryUpdateModel(daDb)) {
                    // save context.
                    int esito = this.Dc.Attachments_Save();
                    // check if all is ok.
                    if (esito > -1) {
                        Log_Insert(attachment.Id, "ATTACHMENT", "EDIT", true, "Operazione conclusa con successo", "", "", "", "");
                        op = new OpEsitoModel() { idReturn = attachment.Id, riuscita = true };
                        return Json(op, JsonRequestBehavior.AllowGet);
                    }
                    else {
                        Log_Insert(attachment.Id, "BOOKS", "EDIT", false, "Operazione non conclusa", "", "", "", "");
                        op = new OpEsitoModel() { idReturn = attachment.Id, riuscita = false };
                        return Json(op, JsonRequestBehavior.AllowGet);
                    }
                }
                Log_Insert(attachment.Id, "BOOKS", "EDIT", false, "Operazione non conclusa", "", "", "", "");
                op = new OpEsitoModel() { idReturn = attachment.Id, riuscita = false };
                return Json(op, JsonRequestBehavior.AllowGet);
            }
            else {
                // create case.

                attachment.Id = Guid.NewGuid().ToString();

                if (PhotoPath != null) {
                    string relativePathJpeg = "~/Attachments/";
                    string dir = "/" + Guid.NewGuid().ToString() + "/";
                    if(!Directory.Exists(ControllerContext.HttpContext.Server.MapPath(relativePathJpeg) + dir )) {
                        Directory.CreateDirectory(ControllerContext.HttpContext.Server.MapPath(relativePathJpeg) + dir);
                    }
                    string diskPathJpeg = ControllerContext.HttpContext.Server.MapPath(relativePathJpeg) + dir  + PhotoPath.FileName;

                    // save.
                    PhotoPath.SaveAs(diskPathJpeg);
                    attachment.PhotoPath = diskPathJpeg;
                }
                else {
                    Log_Insert(attachment.Id, "BOOKS", "INSERT", false, "Operazione non conclusa: file mancante", "", "", "", "");
                    op = new OpEsitoModel() { idReturn = attachment.Id, riuscita = false };
                    return Json(op, JsonRequestBehavior.AllowGet);
                }

                // save context.
                int esito = this.Dc.Attachments_Insert(attachment);

                // check if all is ok.
                if (esito > -1) {
                    Log_Insert(attachment.Id, "ATTACHMENT", "INSERT", true, "Operazione conclusa con successo", "", "", "", "");
                    op = new OpEsitoModel() { idReturn = attachment.Id, riuscita = true };
                    return Json(op, JsonRequestBehavior.AllowGet);
                }
                else {
                    Log_Insert(attachment.Id, "BOOKS", "INSERT", false, "Operazione non conclusa", "", "", "", "");
                    op = new OpEsitoModel() { idReturn = attachment.Id, riuscita = false };
                    return Json(op, JsonRequestBehavior.AllowGet);
                }
            }

        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult ShowAttachments(string pageId) {
            // return the Attachment list page.

            List<Attachments> att = this.Dc.Attachments_GetsByPageId(pageId);
 
            // return the partial view containing the pages.
            return Json(GetRenderPartialView(this, "UC_AttachmentsListContent", att), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult ShowPages(string subSectionId) {
            // return the Subsection list page.

            List<Pages> pages = this.Dc.Pages_GetBySubSection(subSectionId);

            // set viewbag.
            if (pages.Count > 0) {
                Pages p = pages.First();
                if (p.SubSections != null) {
                    ViewBag.SubTitle = p.SubSections.Name;
                }
            }

            // return the partial view containing the pages.
            return Json(GetRenderPartialView(this, "UC_PageListContent", pages), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult SubmitTranscription(string PageId) {
            OpEsitoModel op;
            string idUser = this.Dc.AspNetUsers_Get_ByName(User.Identity.Name).Id;
            string docRootFolderOxygen = ConfigurationManager.AppSettings["Oxygen_Folder"];
            string docRootFolder = "~/" + ConfigurationManager.AppSettings["Doc_Folder"];
            try {
                Pages page = this.Dc.Pages_Get(PageId);
                if(page == null) {
                    op = new OpEsitoModel() { riuscita = false };                    
                    return Json(op, JsonRequestBehavior.AllowGet);
                }

                Log_Insert(idUser, "BOOKS", "SUBMIT TRANSCRIPTION", true, "Inizio salvataggio trascrizione della pagina", page.Id,"", "", "");
                // set page as not available to other transcription.

                Transcriptions newTrascription = new Transcriptions() {
                    Id = Guid.NewGuid().ToString(),
                    IsApproved = false,
                    ProposedBy = idUser,
                    ProposedDate = DateTime.Now,
                    FileName = ""
                };
                page.Transcriptions.Add(newTrascription);
                int result = this.Dc.Transcriptions_Save();
                Log_Insert(idUser, "BOOKS", "SUBMIT TRANSCRIPTION", true, "Salvataggio EF trascrizione della pagina", page.Id, "", "", "");

                DirectoryInfo d = new DirectoryInfo(docRootFolderOxygen);
                string currentDir = ControllerContext.HttpContext.Server.MapPath(docRootFolder);

                if(result > 0 && d.Exists) {
                    //move file.                        
                    FileInfo[] files = d.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
                    if(files != null && files.Count ()> 0) {
                        files = files.Where(f => !f.Name.Contains("sample")).OrderByDescending(f => f.LastWriteTime).ToArray();
                        
                        FileInfo sourceFile = files.FirstOrDefault();

                        if(sourceFile != null) {
                            Log_Insert(idUser, "BOOKS", "SUBMIT TRANSCRIPTION", true, "Trovata trascrizione: " + sourceFile.FullName, page.Id, "", "", "");

                            string newFileName = Guid.NewGuid().ToString() +  sourceFile.Name;                             
                            string newFilePath = Path.Combine(currentDir, newFileName);
                            Log_Insert(idUser, "BOOKS", "SUBMIT TRANSCRIPTION", true, "Sposto la trascrizione su: " + newFilePath, page.Id, "", "", "");

                            //  move a file or folder to a new location.
                            System.IO.File.Move(sourceFile.FullName, newFilePath);
                            Log_Insert(idUser, "BOOKS", "SUBMIT TRANSCRIPTION", true, "Trascrizione spostata", page.Id, "", "", "");

                            // update transcription
                            newTrascription = this.Dc.Transcriptions_Get(newTrascription.Id);
                            newTrascription.FileName = newFileName;
                            this.Dc.Transcriptions_Save();
                            Log_Insert(idUser, "BOOKS", "SUBMIT TRANSCRIPTION", true, "Fine salvataggio trascrizione della pagina", page.Id, "", "", "");
                            op = new OpEsitoModel() { riuscita = true };
                            return Json(op, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                Log_Insert(idUser, "BOOKS", "SUBMIT TRANSCRIPTION", false, "Salvataggio non andato a buon fine", page.Id, "", "", "");
            }
            catch(Exception ex) {
                Log_Insert(idUser, "BOOKS", "SUBMIT TRANSCRIPTION ", false, "Operazione non conclusa", ex.Message, "", "", "");
            }
            op = new OpEsitoModel() { riuscita = false };
            return Json(op, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult UpdateFilterView(string filterName) {
            // return the UpdateFilterView page.

            Filters f = this.Dc.Filters_GetByName(filterName);

            // return the partial view containing the Uc_filter page.
            return Json(GetRenderPartialView(this, "UC_Filter", f), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UploadSection(string bookName, string path) {
            // upload a section content.
            OpEsitoModel op;


            Books b = this.Dc.Books_GetByTitle(bookName);
            if (b != null) {
                // open directories
                List<string> sections = Directory.GetDirectories(path).ToList();

                foreach (string section in sections) {
                    string sectionName = section.Split('\\').Last();
                    
                    if(b.Sections.Where(s => s.Name == sectionName).Count() > 0) {
                        Sections sec = b.Sections.FirstOrDefault(s => s.Name == sectionName);

                        // update it.
                        // find subsections.
                        List<string> subSections = Directory.GetDirectories(path + '\\' + sectionName).ToList();
                        // check if we have subsections.
                        if (subSections.First().Split('\\').Last() != "JPEG" && subSections.First().Split('\\').Last() != "MASTER") {
                            foreach (string subSection in subSections) {
                                string subSectionName = subSection.Split('\\').Last();

                                if(b.Sections.FirstOrDefault(s => s.Name == sectionName) != null && b.Sections.FirstOrDefault(s => s.Name == sectionName).SubSections.Where(ss => ss.Name== subSectionName).Count() == 0) {
                                    // create subsection.
                                    SubSections subSec = new SubSections() {
                                        Id = Guid.NewGuid().ToString(),
                                        Name = subSectionName,
                                        Description = "",
                                        Section = sec.Id
                                    };

                                    // find page. 
                                    var pages = Directory.GetFiles(path + '\\' + sectionName + '\\' + subSectionName + '\\' + "JPEG");

                                    foreach (String page in pages) {
                                        string pageName = page.Split('\\').Last();
                                        pageName = pageName.Substring(0, pageName.Length - 4).Split('.').Last(); // removing .jpg

                                        // ignore first (is the subsetion number)
                                        string[] data = pageName.Split('_');

                                        string numericOrder = data[1];
                                        string completeOrder = "";
                                        string title = "";

                                        if (data.Length > 2 &&Int32.TryParse(data[2][0].ToString(), out int res)) {
                                            completeOrder = data[2];
                                        }
                                        else {
                                            if(data.Length > 2) {
                                                title = data[2];
                                                completeOrder = data[2];
                                            }                                        
                                            if (data.Length > 3) {
                                                completeOrder = data[3];
                                                if (data.Length == 5) {
                                                    completeOrder = data[2] + " " + data[3] + " " + data[4];
                                                }

                                            }
                                        }
                                        if (data.Length == 6) {

                                        }

                                        //string a = "I-1_0001_PiattoAnteriore";
                                        //string b = "I-1_0002_ControguardiaAnteriore";
                                        //string aa = "I-1_0003_CartaDiGuardiaAnteriore_r";
                                        //string c = "II_0028_5v";
                                        //string e = "I-1_0016_Indice_bis_v";
                                        //string d = "LIII_0194_Taglio";

                                        string tiffPage = Directory.GetFiles(path + '\\' + sectionName + '\\' + subSectionName + '\\' + "MASTER").FirstOrDefault(pages_ => pages_.Contains(numericOrder));
                                        if (tiffPage == null) {
                                            tiffPage = "";
                                        }

                                        Pages p = new Pages() {
                                            Id = Guid.NewGuid().ToString(),
                                            BookTitle = b.Title,
                                            Description = "",
                                            Editor = "",
                                            PhotoPath = page,
                                            SectionName = sec.Name,
                                            SubSection = subSec.Id,
                                            NumericOrder = Convert.ToInt32(numericOrder),
                                            CompleteOrder = completeOrder,
                                            Title = title,
                                            BigPhotoPath = tiffPage,
                                        };
                                        // add page.
                                        subSec.Pages.Add(p);
                                    }

                                    // add sub-section to section.
                                    sec.SubSections.Add(subSec);
                                }

                                
                            }
                        }

                        // add section to book.
                        int esito = this.Dc.SaveChanges();
                    }                

                }
            }

            Log_Insert("", "BOOKS", "UPDATE", false, "Operazione non conclusa", "", "", "", "");
            op = new OpEsitoModel() { riuscita = false };
            return Json(op, JsonRequestBehavior.AllowGet);
        }
    }
}