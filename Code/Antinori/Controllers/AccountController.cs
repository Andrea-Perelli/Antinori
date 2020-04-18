using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Antinori.Models;

namespace Antinori.Controllers {

    public class AccountController : ApplicationController {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private MailController mailController;

        public AccountController() {
            this.mailController = new MailController();

        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager) {
            UserManager = userManager;
            SignInManager = signInManager;
            this.mailController = new MailController();

        }

        public ApplicationSignInManager SignInManager {
            get {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager {

            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set {
                _userManager = value;
            }
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Add() {
            AspNetUsers s = new AspNetUsers();

            // set the sezioni list.
            ViewBag.RuoliList = this.Dc.Ruoli_Nome_Gets();

            return Json(GetRenderPartialView(this, "UC_AspNetUsers", s), JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin,Editor,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ChangePassword(AspNetUsers aspNetUsers) {
            // change the user password.

            OpEsitoModel op;

            // retrieving user id from session.
            string id = this.User.Identity.GetUserId();
            // retrieve user.
            AspNetUsers user = this.Dc.AspNetUsers_Get(id);
            // change password.
            var changePasswordEsito = this.UserManager.ChangePassword(id, aspNetUsers.Password, aspNetUsers.NewPassword);
            if(changePasswordEsito.Succeeded) {
                var u = Dc.AspNetUsers_Get(id);
                Dc.AspNetUsers_Save();

                string corpo = "Gentile <b>" + user.Name + " " + user.Surname + "</b>,"
                           + "<br><br>la tua password è stata modificata: la tua nuova password è: <b>" + aspNetUsers.NewPassword + "</b>.<br>"
                           + "Ti consigliamo di proteggere questi dati. <br><br><br><br>Saluti,<br> il team Antinori.";

                // send mail.
                string email = System.Configuration.ConfigurationManager.AppSettings["SMTP_From"];
                this.mailController.SendEmail("Cambio Password", corpo, u.Email);

                // set log.
                this.Log_Insert(id, "AspNetUsers", "CHANGE PASSWORD", true, "Operazione conclusa con successo", "", "", "", "");
                op = new OpEsitoModel() { idReturn = id, riuscita = true };
            }
            else {
                Log_Insert(id, "AspNetUsers", "CHANGE PASSWORD", false, "Errore:" + changePasswordEsito.Errors.ToString());
                op = new OpEsitoModel() { idReturn = id, riuscita = false, msg = changePasswordEsito.Errors.ToString() };
            }
            return Json(op, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string id) {
            // delete an account by id.

            // set default value for esito.
            bool esito = false;

            // retrieve user.
            var user = this.Dc.AspNetUsers_Get(id);

            // "delete" user.
            user.Cancellato = true;
            user.LockoutEnabled = true;
            user.LockoutEndDateUtc = DateTime.MaxValue;
            //save and update the esito value.
            esito = this.Dc.AspNetUsers_Save() > -1;
            return Json(esito, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Edit(string id) {
            // retrieve the user with name mobile phone etc...
            var user = this.Dc.AspNetUsers_Get(id);

            // set the sezioni list.
            ViewBag.RuoliList = this.Dc.Ruoli_Nome_Gets();

            // return the partial view containing the user.
            return Json(GetRenderPartialView(this, "UC_AspNetUsers", user), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ForgotPassword(string Username) {
            // reset the user password.

            // set default value for esito.
            bool esito = false;

            // retrieve user.
            var User = this.Dc.AspNetUsers_Get_ByUsername(Username);
            if(User != null) {
                // retrieve configuration file email.
                string email = System.Configuration.ConfigurationManager.AppSettings["SMTP_From"];

                // reset password
                string password = this.CreatePassword(Int16.Parse(System.Configuration.ConfigurationManager.AppSettings["minPasswordLenght"]));

                var removePasswordEsito = this.UserManager.RemovePassword(User.Id);
                if(removePasswordEsito.Succeeded) {
                    var addPasswordEsito = this.UserManager.AddPassword(User.Id, password);
                    if(addPasswordEsito.Succeeded) {
                        // set corpo.
                        string corpo = "Gentile <b>" + User.Name + " " + User.Surname
                            + "</b>,<br><br>La tua password è stata resettata: la tua nuova password è: <b>" + password + "</b>.<br>"
                            + "Ti consigliamo di cambiare la password al più presto. <br><br><br><br>Saluti,<br> il team Antinori.";

                        // send mail.
                        this.mailController.SendEmail("Reset Password", corpo, User.Email);

                        //the esito value.
                        esito = true;
                        Log_Insert(User.UserName, "AspNetUsers", "UPDATE", true, "Operazione di reset password conclusa con successo");
                    }
                    else { //could not change password.
                        Log_Insert(User.UserName, "AspNetUsers", "UPDATE", false, "Operazione di reset password conclusa con errore: non è stato possibile cambiare la password.");
                    }
                }
                else { //could not change password.
                    Log_Insert(User.UserName, "AspNetUsers", "UPDATE", false, "Operazione di reset password conclusa con errore: non è stato possibile cambiare la password.");
                }
            }
            else { //user == null
                Log_Insert(User.UserName, "AspNetUsers", "UPDATE", false, "Operazione di reset password conclusa con errore: utente non trovato.");
            }
            // return the partial view .
            return Json(esito, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Editor")]
        public JsonResult GetUsersInfo(){
            // return user info.

            // create a model. return the model.
            DashboardModelUsers model = new DashboardModelUsers();
            model.numberOfUsers = this.Dc.AspNetUsers_Gets().Count;
            model.numberOfNormalUsers = this.Dc.AspNetUsers_Get_ByRole("User").Count;
            // return the partial view .
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {

            // open login page.
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl) {
            //login response.

            if(!ModelState.IsValid) {
                return View(model);
            }

            // Questa opzione non calcola il numero di tentativi di accesso non riusciti per il blocco dell'account
            // Per abilitare il conteggio degli errori di password per attivare il blocco, impostare shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch(result) {
                case SignInStatus.Success:
                    var u = Dc.AspNetUsers_Get_ByUsername(model.Email);
                    if(u.AspNetRoles.FirstOrDefault().Name.Equals("User")) {
                        return RedirectToLocal("/Home/Home");                        
                    }
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "Account disattivato, contattare l'amministratore.");
                    return View(model);
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Username o Password errata.");
                    return View(model);
            }
        }

        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Index() {
            // return the account list view.           

            return View();
        }

        [Authorize(Roles = "Admin")]
        public JsonResult List() {
            // return the list of the users.

            List<AspNetUsers> users = this.Dc.AspNetUsers_Gets();
            // return the partial view containing all the users.
            return Json(GetRenderPartialView(this, "UC_AspNetUsers_List", users), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOff() {
            // logout.
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Home", "Home");
        }

        public ActionResult P_ChangePassword() {
            // set the change password view. own password.

            // retrieve the user.
            var user = this.Dc.AspNetUsers_Get(this.User.Identity.GetUserId());

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult P_Create() {
            AspNetUsers s = new AspNetUsers();

            // set the sezioni list.
            ViewBag.RuoliList = this.Dc.Ruoli_Nome_Gets();

            return View(s);
        }

        public ActionResult P_ForgotPassword() {
            // return the forgot password user.

            return View();
        }

        [Authorize(Roles = "Admin, Editor")]
        public ActionResult P_Profile() {
            // set the profile view. 

            // retrieve the user with name mobile phone etc.
            var user = this.Dc.AspNetUsers_Get(this.User.Identity.GetUserId());

            return View(user);
        }

        [Authorize(Roles = "Admin, Editor, User")]
        public ActionResult P_PublicChangePassword() {
            // set the public profile view. 

            // retrieve the user with name mobile phone etc.
            var user = this.Dc.AspNetUsers_Get(this.User.Identity.GetUserId());

            return View("P_PublicProfile", user);
        }

        [Authorize(Roles = "Admin, Editor, User")]
        public ActionResult P_PublicProfile() {
            // set the public profile view. 

            // retrieve the user with name mobile phone etc.
            var user = this.Dc.AspNetUsers_Get(this.User.Identity.GetUserId());

            return View("P_PublicProfile",user);
        }

        [Authorize(Roles = "Admin,Editor")]
        [HttpPost]
        public JsonResult Save(AspNetUsers user, FormCollection forms, string username) {
            // save. 
            OpEsitoModel op;

            if(user.Id != null) {
                // we are editing user.
                AspNetUsers daDb = this.Dc.AspNetUsers_Get(user.Id);

                //map the two objects: it updates the DB object.
                if(TryUpdateModel(daDb)) {
                    try {
                        // set again attivo field (for some reason is set always to to true).
                        //daDb.LockoutEnabled = !forms["Attivo"].ToString().ToLower().Equals("true");
                        //daDb.LockoutEndDateUtc = DateTime.MaxValue;

                        // delete roles
                        daDb.AspNetRoles.Clear();
                        this.Dc.AspNetUsers_Save();
                        //update (add) roles.
                        foreach(var key in forms) {
                            if(key.ToString().Equals("Role")) {
                                // add roles: the entity framework will create the relationship.
                                daDb.AspNetRoles.Add(this.Dc.Ruoli_Get(forms[key.ToString()].Replace("R_", "")));
                            }
                        }
                        this.Dc.AspNetUsers_Save();

                        // set log.
                        Log_Insert(user.Id, "AspNetUsers", "UPDATE", true, "Operazione conclusa con successo", "", "", "", "");
                        op = new OpEsitoModel() { idReturn = user.Id, riuscita = true };
                        return Json(op, JsonRequestBehavior.AllowGet);
                    }
                    catch(Exception ex) {
                        Log_Insert(user.Id, "AspNetUsers", "UPDATE", false, "Errore:" + ex.Message);
                        op = new OpEsitoModel() { idReturn = "", riuscita = false, msg = ex.Message };
                        return Json(op, JsonRequestBehavior.AllowGet);
                    }
                }
                // I couldn't map the object.
                Log_Insert(user.Id, "AspNetUsers", "UPDATE", false, "Errore mappatura su DB");
                op = new OpEsitoModel() { idReturn = "", riuscita = false, msg = "Errore mappatura su DB" };
                return Json(op, JsonRequestBehavior.AllowGet);
            }
            // if we are creating a new object.
            else {
                // generate new password.
                string password = this.CreatePassword(Convert.ToInt16(ConfigurationManager.AppSettings["minPasswordLenght"]));

                // create new user.
                var newUser = new ApplicationUser { Email = user.Email, PhoneNumber = user.PhoneNumber, Cancellato = false, LockoutEnabled = user.Attivo };
                if(username != null && username.Length > 0) {
                    newUser.UserName = username;
                }
                else {
                    newUser.UserName = newUser.Email;
                }

                var result = UserManager.CreateAsync(newUser, password);
                // OK.
                if(result.Result.Succeeded) {

                    // retrieve user.
                    AspNetUsers u = this.Dc.AspNetUsers_Get(newUser.Id);

                    // set again attivo field (for some reason is set always to to true).
                    u.LockoutEnabled = false;
                    u.LockoutEndDateUtc = DateTime.MaxValue;
                    u.Name = user.Name;
                    u.Surname = user.Surname;
                    u.Profession = user.Profession;
                    // set roles
                    foreach(var key in forms) {
                        if(key.ToString().Equals("Role")) {
                            // add role: the entity framework will create the relationship.
                            u.AspNetRoles.Add(this.Dc.Ruoli_Get(forms[key.ToString()].Replace("R_", "")));
                        }
                    }
                    // save context (roles).
                    this.Dc.AspNetUsers_Save();

                    string msg = "Gentile <b>" + newUser.UserName + ", "
                       + "</b>,<br><br>Benvenuto nel nostro Team! <br/><br/>I dati per accedere ad Antinori sono i seguenti:<br>Username: <b>" + newUser.UserName + "</b><br>Password: " + password + "<br/><br/>"
                       + "Ti consigliamo di modificare la password al più presto. <br><br><br><br>Saluti,<br> il team Antinori.";

                    this.mailController.SendEmail("Benvenuto su Antinori", msg, user.Email);

                    Log_Insert(newUser.Id, "Account", "INSERT", true, "Operazione conclusa con successo", "", "", "", "");
                    op = new OpEsitoModel() { idReturn = newUser.Id, riuscita = true };
                    return Json(op, JsonRequestBehavior.AllowGet);
                }
                else { // PROBLEMS.
                    string error_msg = "";
                    foreach(var er in result.Result.Errors) {
                        error_msg += er + ";";
                    }
                    Log_Insert(newUser.Email, "Account", "INSERT", false, "Errore:" + error_msg);
                    op = new OpEsitoModel() { idReturn = "", riuscita = false, msg = error_msg };
                    return Json(op, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Authorize(Roles = "Editor,User")]
        [HttpPost]
        public JsonResult SavePublic(AspNetUsers user, FormCollection forms) {
            // this save gives us the possibility to save from a public profile. 
            // we removed the creation section to improve security.
            OpEsitoModel op;

            if(user.Id != null) {
                // we are editing user.
                AspNetUsers daDb = this.Dc.AspNetUsers_Get(user.Id);

                //map the two objects: it updates the DB object.
                if(TryUpdateModel(daDb)) {
                    try {
                        // save
                        this.Dc.AspNetUsers_Save();                        

                        // set log.
                        Log_Insert(user.Id, "AspNetUsers", "UPDATE", true, "Operazione conclusa con successo", "", "", "", "");
                        op = new OpEsitoModel() { idReturn = user.Id, riuscita = true };
                        return Json(op, JsonRequestBehavior.AllowGet);
                    }
                    catch(Exception ex) {
                        Log_Insert(user.Id, "AspNetUsers", "UPDATE", false, "Errore:" + ex.Message);
                        op = new OpEsitoModel() { idReturn = "", riuscita = false, msg = ex.Message };
                        return Json(op, JsonRequestBehavior.AllowGet);
                    }
                }
                // I couldn't map the object.
                Log_Insert(user.Id, "AspNetUsers", "UPDATE", false, "Errore mappatura su DB");
                op = new OpEsitoModel() { idReturn = "", riuscita = false, msg = "Errore mappatura su DB" };
                return Json(op, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        // ************************ NOT CHECKED ************************


        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe){
            // Impostare come condizione che l'utente abbia già eseguito l'accesso con nome utente/password o account di accesso esterno
            if (!await SignInManager.HasBeenVerifiedAsync()){
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model){
            if (!ModelState.IsValid){
                return View(model);
            }

            // La parte di codice seguente protegge i codici di autenticazione a due fattori dagli attacchi di forza bruta. 
            // Se un utente immette codici non corretti in un intervallo di tempo specificato, l'account dell'utente 
            // viene bloccato per un intervallo di tempo specificato. 
            // Si possono configurare le impostazioni per il blocco dell'account in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Codice non valido.");
                    return View(model);
            }
        }

        
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code){
            if (userId == null || code == null){
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation(){
            return View();
        }
        
        
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Richiedere un reindirizzamento al provider di accesso esterno
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generare il token e inviarlo
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Se l'utente ha già un account, consentire l'accesso dell'utente a questo provider di accesso esterno
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Se l'utente non ha un account, chiedere all'utente di crearne uno
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Recuperare le informazioni sull'utente dal provider di accesso esterno
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }


        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing){
            if (disposing){
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helper
        // Usato per la protezione XSRF durante l'aggiunta di account di accesso esterni
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }

}