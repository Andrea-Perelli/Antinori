using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Antinori.Models {

    public partial class AspNetUsers{

        // only reading property that specifies is the account is enabled 
        // (now refers to the lockoutenabled property but could change).
        public bool Attivo { get {
                return !(this.LockoutEnabled && this.LockoutEndDateUtc>DateTime.Now); }
        }

        
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Conferma Password")]
        [Compare("Password", ErrorMessage = "La password e la password di conferma non corrispondono.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nuova Password")]
        public string NewPassword { get; set; }

    }
}