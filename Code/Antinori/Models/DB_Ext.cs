using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Antinori.Models {

    public partial class AntinoriEntities {

        #region Attachments

        public int Attachments_Delete(Attachments a) {
            // delete an Attachments.
            int esito = -1;
            try {
                Attachments.Remove(a);
                // Entity framework stores its current status on the db (we don't pass any object to store).
                esito = SaveChanges();
            }
            catch (Exception e) {

            }
            return esito;
        }

        public List<Attachments> Attachments_Gets() {
            // return the list of all Attachments.
            return Attachments.ToList();
        }

        public List<Attachments> Attachments_GetsByPageId(string pageId) {
            // return the list of all Attachments of a page.
            return Attachments.Where(a => a.PageId.Equals(pageId)).ToList();
        }

        public Attachments Attachments_Get(string id) {
            // return a Attachments by Id.
            Attachments att = Attachments.FirstOrDefault(it => it.Id == id);
            return att;
        }

        public int Attachments_Insert(Attachments c) {
            // insert an Attachments.
            int esito = -1;
            try {
                Attachments.Add(c);
                esito = SaveChanges();
            }
            catch (Exception ex) {
            }
            return esito;
        }

        public int Attachments_Save() {
            int esito = -1;
            try {
                // esito is the number of modifications.
                esito = SaveChanges();
            }
            catch (Exception e) {

            }
            return esito;
        }
        #endregion

        #region AspNetUsers

        public List<AspNetUsers> AspNetUsers_Gets() {
            // return the list of all users.
            return AspNetUsers.Where(u => !u.Cancellato).ToList();
        }

        public AspNetUsers AspNetUsers_Get(string id) {
            // return a AspNet User by Id.
            return AspNetUsers.FirstOrDefault(it => it.Id == id);
        }

        public AspNetUsers AspNetUsers_Get_ByName(string username) {
            // return a AspNet User by Name.
            return AspNetUsers.FirstOrDefault(it => it.UserName == username);
        }

        public List<AspNetUsers> AspNetUsers_Get_ByRole(string roleName) {
            // return a AspNet User by Name.
            return AspNetUsers.Where(it => it.AspNetRoles.FirstOrDefault().Name == roleName).ToList();
        }

        public AspNetUsers AspNetUsers_Get_ByUsername(string username) {
            // return a AspNet User by email.
            return AspNetUsers.FirstOrDefault(it => it.UserName == username);
        }

        public int AspNetUsers_Save() {
            int esito = -1;
            try {
                // esito is the number of modifications.
                esito = SaveChanges();
            }
            catch (Exception e){

            }
            return esito;
        }

        public int AspNetUsers_Delete_Definitivo(AspNetUsers c) {
            // it is NOT used by Antinori ADMINISTRATORS.
            int esito = -1;
            try {
                AspNetUsers.Remove(c);
                // Entity framework stores its current status on the db (we don't pass any object to store).
                esito = SaveChanges();
            }
            catch {

            }
            return esito;
        }

        public int AspNetUsers_Delete(AspNetUsers c) {
            // it is used by Antinori ADMINISTRATORS.
            int esito = -1;
            try {
                // set as true: it is deleted.
                c.Cancellato = true;
                // Entity framework stores its current status on the db (we don't pass any object to store).
                esito = SaveChanges();
            }
            catch (Exception e){

            }
            return esito;
        }

        #endregion

        #region AspNetRoles

        public AspNetRoles Ruoli_Get(string name){
            // return a role by id.

            return this.AspNetRoles.FirstOrDefault(it => it.Name == name);
        }
        public List<AspNetRoles> Ruoli_Gets(){
            // return the list of the asp net roles.

            var query = this.AspNetRoles.OrderBy(it => it.Name).ToList();
            return query;
        }


        public List<string> Ruoli_Nome_Gets() {
            // return the list of the asp net roles names.

            var query = this.AspNetRoles.Select(it => it.Name).OrderBy(it => it).ToList();
            return query;
        }

        #endregion

        #region Books

        public int Books_Count() {
            // count number of Books.
            int res = Books_Gets().Count;
            return res;
        }

        public int Books_Delete(Books b) {
            // delete a book.
            int esito = -1;
            try {
                Books.Remove(b);
                // Entity framework stores its current status on the db (we don't pass any object to store).
                esito = SaveChanges();
            }
            catch (Exception e){

            }
            return esito;
        }

        public List<Books> Books_Gets() {
            // return the list of all books.
            return Books.ToList();
        }
        public List<Books> Books_Gets(bool includeSources) {
            // return the list of all books.
            if (includeSources) {
                return Books.ToList();
            }
            return Books.Where( b => b.IsSource == false).ToList();
        }

        public List<Books> Books_GetsBySource(string sourceId) {
            // return the list of all books source of the given one.
            return Books.Where(b => b.Source != null && b.Source == sourceId).ToList();
        }

        public Books Books_Get(string id) {
            // return a Books by Id.
            Books book = Books.FirstOrDefault(it => it.Id == id);
            return book;
        }

        public Books Books_GetByTitle(string title) {
            // return a Books by Title.
            Books book = Books.FirstOrDefault(it => it.Title == title);
            return book;
        }

        public int Books_Insert(Books c) {
            // insert a book.
            int esito = -1;
            try {
                Books.Add(c);
                esito = SaveChanges();
            }
            catch(Exception ex) {
            }
            return esito;
        }

        public int Books_Save() {
            int esito = -1;
            try {
                // esito is the number of modifications.
                esito = SaveChanges();
            }
            catch(Exception e) {

            }
            return esito;
        }
        #endregion

        #region Filters

        public int Filters_Delete(Filters f) {
            // delete a Filters.
            int esito = -1;
            try {
                Filters.Remove(f);
                // Entity framework stores its current status on the db (we don't pass any object to store).
                esito = SaveChanges();
            }
            catch(Exception e) {

            }
            return esito;
        }

        public Filters Filters_Get(string id) {
            // return a filter by Id.
            Filters filter = Filters.FirstOrDefault(it => it.Id == id);
            return filter;
        }

        public Filters Filters_GetByName(string name) {
            // return a filter by name.
            Filters filter = Filters.FirstOrDefault(f => f.Name.Equals(name));
            return filter;
        }

        public List<Filters> Filters_GetByPageId(string pageId) {
            // return all the page filter.
            List<Filters> filters = Filters.Where(f => f.Page.Equals(pageId)).ToList();
            return filters;
        }

        public List<Filters> Filters_Gets() {
            // return all the filter by Id.
            List<Filters> filters = Filters.ToList();
            return filters;
        }


        public int Filters_Insert(Filters f) {
            // insert a Filter.
            int esito = -1;
            try {
                Filters.Add(f);
                esito = SaveChanges();
            }
            catch(Exception ex) {

            }
            return esito;
        }

        #endregion

        #region FrontSlider

        public FrontSlider FrontSlider_GetFromDate() {
            // return the image of the current date.

            // calculate week number(1,2,3,4).
            int weekNum = 1;
            if(DateTime.Now.Day <= 8) {
                weekNum = 1;
            }
            if (DateTime.Now.Day > 8 && DateTime.Now.Day <= 16) {
                weekNum = 2;
            }
            if (DateTime.Now.Day > 16 && DateTime.Now.Day <= 24) {
                weekNum = 3;
            }
            if (DateTime.Now.Day > 24 && DateTime.Now.Day <= 31) {
                weekNum = 4;
            }
            return FrontSlider.FirstOrDefault(f => f.Month == DateTime.Now.Month && f.Week == weekNum);
        }

        #endregion

        #region Pages

        public int Pages_Count() {
            // count number of pages.
            int res = Pages_Gets().Count;
            return res;
        }

        public int Pages_Delete(Pages p) {
            // delete a Page.
            int esito = -1;
            try {
                Pages.Remove(p);
                // Entity framework stores its current status on the db (we don't pass any object to store).
                esito = SaveChanges();
            }
            catch(Exception e) {

            }
            return esito;
        }

        public Pages Pages_Get(string id) {
            // return a Page by Id.
            Pages page = Pages.FirstOrDefault(it => it.Id == id);
            return page;
        }

        public List<Pages> Pages_Gets() {
            // return all Pages.
            List<Pages> pages = Pages.ToList();
            return pages;
        }
        public List<Pages> Pages_GetByFilterName(string filterName) {
            // return all Pages of a filter name.
            List<Pages> pages = Filters.Where(p => p.Name.Equals(filterName)).Select( p=> p.Pages).ToList();
            return pages;
        }

        public List<Pages> Pages_GetByFilterNameList(string[] filterNames) {
            // return all Pages of a filter list.

            List<Pages> pages = new List<Pages>();
            foreach(var filter in filterNames) {

                pages = pages.Union(this.Pages_GetByFilterName(filter)).ToList();
            }
            return pages;
        }

        public int Pages_GetByFilterNameListNumber(string[] filterNames) {
            // return the number of pages containing a filter inside a list.
     
            return Pages_GetByFilterNameList(filterNames).Count();
        }

        public List<Pages> Pages_GetByNumber(int number) {
            // return a list of pages with the same number.
            return Pages.Where(p => p.NumericOrder == number).ToList();
        }
        public List<Pages> Pages_GetByNumberOrderedBySectionName(int number) {
            // return a list of pages with the same number.
            return Pages_GetByNumber(number).OrderBy(p => p.SectionName).ToList();
        }

        public List<Pages> Pages_GetBySubSection(string subSectionId) {
            // return all Pages of a subsection.
            List<Pages> pages = Pages.Where(p => p.SubSection.Equals(subSectionId)).OrderBy(p =>p.NumericOrder).ToList();
            return pages;
        }

        public Pages Pages_GetBySubSectionAndNumber(string subSectionId, int number) {
            // return a Page of a subsection.

            Pages pages = SubSectionss_Get(subSectionId).Pages.FirstOrDefault(p => p.NumericOrder == number);
            return pages;
        }

        public Pages Pages_GetBySubSectionNameAndNumber(string subSectionName, int number) {
            // return a Page of a subsection.

            Pages pages = SubSectionss_GetByName(subSectionName).Pages.FirstOrDefault(p => p.NumericOrder == number);
            return pages;
        }
        public List<Pages> Pages_GetFirstNByFilterNameList(string[] filterNames, int n) {
            // return first n pages of an advanced search.
            List<Pages> pages = this.Pages_GetByFilterNameList(filterNames).OrderBy(p => p.NumericOrder).Take(n).ToList();
            return pages;
        }
        public List<Pages> Pages_GetFirstNByFilterNameListAndIndex(string[] filterNames, int n, int page) {
            // return first n pages of an advanced search and of an index.

            List<Pages> pages = this.Pages_GetByFilterNameList(filterNames).OrderBy(p => p.NumericOrder).Skip((page - 1) * n).Take(n).ToList();

            return pages;
        }

        public List<Pages> Pages_GetFirstNBySubSection(string subSectionId, int n) {
            // return all Pages of a subsection.
            List<Pages> pages = Pages.Where(p => p.SubSection.Equals(subSectionId)).OrderBy(p => p.NumericOrder).Take(n).ToList();
            return pages;
        }

        public List<Pages> Pages_GetFirstNBySubSectionAndIndex(string subSectionId, int n, int page) {
            // return all Pages of a subsection.
            List<Pages> pages = Pages.Where(p => p.SubSection.Equals(subSectionId)).OrderBy(p => p.NumericOrder).Skip((page-1) * n).Take(n).ToList();
            return pages;
        }

        public int Pages_Insert(Pages c) {
            // insert a book.
            int esito = -1;
            try {
                Pages.Add(c);
                esito = SaveChanges();
            }
            catch(Exception ex) {
               
            }
            return esito;
        }

        public int Pages_Save() {
            int esito = -1;
            try {
                esito = SaveChanges();
            }
            catch (Exception e) {

            }
            return esito;
        }

        #endregion

        #region "Logs"

        public int Logs_Count() {
            var query = Logs.Count();
            return query;
        }

        public List<Logs> Logs_Gets() {

            return Logs.OrderByDescending(l => l.Operazione_Data).ToList();
        }


        public Logs Logs_Get(int id) {
            var query = Logs.FirstOrDefault(c => c.Id == id);
            return query;
        }

        public int Logs_Insert(Logs c) {
            int esito = -1;
            try {
                Logs.Add(c);
                esito = SaveChanges();
            }
            catch(Exception ex) {
            }
            return esito;
        }

        public int Logs_Save() {
            int esito = -1;
            try {
                esito = SaveChanges();
            }
            catch {

            }
            return esito;
        }

        public int Logs_Delete(Logs c) {
            int esito = -1;
            try {
                Logs.Remove(c);
                esito = SaveChanges();
            }
            catch {

            }
            return esito;
        }

        #endregion

        #region Sections

        public Sections Sections_Get(string id) {
            // return a section by Id.
            Sections section = Sections.FirstOrDefault(it => it.Id == id);
            return section;
        }

        public List<Sections> Sections_GetsByBookId(string bookId) {
            // return the list of all SubSections.
            return Sections.Where(s => s.Book.Equals(bookId)).ToList();
        }

        public int Sections_Save() {
            int esito = -1;
            try {
                // esito is the number of modifications.
                esito = SaveChanges();
            }
            catch(Exception e) {

            }
            return esito;
        }

        #endregion

        #region SubSections

        public List<SubSections> SubSections_Gets() {
            // return the list of all SubSections.
            return SubSections.ToList();
        }

        public List<SubSections> Sections_GetsBySectionId(string sectionId) {
            // return the list of all SubSections by section id.
            return SubSections.Where(s => s.Section.Equals(sectionId)).OrderBy(s => s.RopeNumber).ToList();
        } 
        
        public SubSections Sections_GetFirstBySectionId(string sectionId) {
            // return the first subsection by section id.
            return SubSections.Where(s => s.Section.Equals(sectionId)).OrderBy(s => s.RopeNumber).FirstOrDefault();
        }

        public SubSections SubSectionss_Get(string id) {
            // return a SubSectionsr by Id.
            return SubSections.FirstOrDefault(it => it.Id == id);
        }

        public SubSections SubSectionss_GetByName(string name) {
            // return a SubSectionsr by Name.
            return SubSections.FirstOrDefault(it => it.Name == name);
        }

        #endregion

        #region Transcriptions

        public int Transcriptions_Count() {
            // count number of Transcriptions.
            int res = Transcriptions_Gets().Count;
            return res;
        }
        public Transcriptions Transcriptions_Get(string id) {
            // return a Transcriptions by Id.
            return Transcriptions.FirstOrDefault(it => it.Id == id);
        }

        public List<Transcriptions> Transcriptions_Gets() {
            // return the list of all Transcriptions.
            return Transcriptions.ToList();
        }

        public List<Transcriptions> Transcriptions_GetAllNotApproved() {
            // return the list of all not approved Transcriptions.
            return Transcriptions.Where(t => t.IsApproved == false).ToList();
        }

        public int Transcriptions_Save() {
            int esito = -1;
            try {
                esito = SaveChanges();
            }
            catch(Exception e) {

            }
            return esito;
        }


        #endregion
    }
}