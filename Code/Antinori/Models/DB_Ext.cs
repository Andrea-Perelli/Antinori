using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Antinori.Models {

    public partial class AntinoriEntities {

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

        public Books Books_Get(string id) {
            // return a Books by Id.
            Books book = Books.FirstOrDefault(it => it.Id == id);
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

        #region Pages

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

        public List<Pages> Pages_GetBySubSection(string subSectionId) {
            // return all Pages of a subsection.
            List<Pages> pages = Pages.Where(p => p.SubSection.Equals(subSectionId)).ToList();
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

        public SubSections SubSectionss_Get(string id) {
            // return a SubSectionsr by Id.
            return SubSections.FirstOrDefault(it => it.Id == id);
        }

        #endregion

        #region Transcriptions

        public List<Transcriptions> Transcriptions_Gets() {
            // return the list of all Transcriptions.
            return Transcriptions.ToList();
        }

        public Transcriptions Transcriptions_Get(string id) {
            // return a Transcriptions by Id.
            return Transcriptions.FirstOrDefault(it => it.Id == id);
        }

        #endregion
    }
}