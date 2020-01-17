using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Antinori.Models {

    public partial class AntinoriEntities {

        public string validaProprieta(DbEntityValidationException ex) {
            string validationErrors = "";
            foreach (var failure in ex.EntityValidationErrors) {
                foreach (var error in failure.ValidationErrors)  {
                    validationErrors += error.PropertyName + "  " + error.ErrorMessage;
                }
            }
            return validationErrors;
        }

        #region AspNetUsers
        
        public AspNetUsers AspNetUsers_Get(string id) {
            // return a AspNet User by Id.
            return AspNetUsers.FirstOrDefault(it => it.Id == id);
        }

        public AspNetUsers AspNetUsers_Get_ByName(string username)
        {
            // return a AspNet User by Name.
            return AspNetUsers.FirstOrDefault(it => it.UserName == username);
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
            catch
            {

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


        public List<string> Ruoli_Nome_Gets()
        {
            // return the list of the asp net roles names.

            var query = this.AspNetRoles.Select(it => it.Name).OrderBy(it => it).ToList();
            return query;
        }

        #endregion

        #region "Logs"

        public int Logs_Count(){
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
            catch (DbEntityValidationException ex) {
                validaProprieta(ex);
            }
            return esito;
        }

        public int Logs_Save(){
            int esito = -1;
            try
            {
                esito = SaveChanges();
            }
            catch
            {

            }
            return esito;
        }

        public int Logs_Delete(Logs c){
            int esito = -1;
            try
            {
                Logs.Remove(c);
                esito = SaveChanges();
            }
            catch
            {

            }
            return esito;
        }

        #endregion

        
    }
}