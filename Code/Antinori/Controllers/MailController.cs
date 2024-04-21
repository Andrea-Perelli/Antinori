using Antinori.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Antinori.Controllers {

    // this class has to contain the code to send emails.
    public class MailController : ApplicationController {

        public MailController() {

        }

        public void SendEmail(string subject, string msg, string to) {
            /*
             * Generate app key: sjgc mtjg ecic frgq

             * https://accounts.google.com/v3/signin/challenge/pwd?TL=AEzbmxx1yyEy7VFhcZdduLrwnlcq6qZnqpGIr9ZE875olcNdU1_Wf-5JBGHRiUoK&cid=2&continue=https%3A%2F%2Fmyaccount.google.com%2Fapppasswords&flowName=GlifWebSignIn&followup=https%3A%2F%2Fmyaccount.google.com%2Fapppasswords&ifkv=ARZ0qKJtkL8guK7LZObOjJ6AwO_lFUNGVoDnsWb0a0fYJy3GpR8C7UAO7VtTHRpQZXcJwB7qG9XO&osid=1&rart=ANgoxccUOgvlDaZdVbcSShhHRP3cMSQSps2Ic26c-2teVftWpV6fsUXPwoR-tfRdl_FQFzjo0OMwB_K0fMt4Dlb9ExWZSU0F063AywAiAVnBLPaESL89__U&rpbg=1&service=accountsettings&theme=mn
             */

            string userId = this.Dc.AspNetUsers_Get_ByName(System.Web.HttpContext.Current.User.Identity.Name) != null ?
                   this.Dc.AspNetUsers_Get_ByName(System.Web.HttpContext.Current.User.Identity.Name).Id :
                   "";
            Log_Insert(userId, "AspNetUsers", "SEND EMAIL", true, "Inizio invio email a: " + to, "", "", "", "");

            // send an email.
            string server = System.Configuration.ConfigurationManager.AppSettings["SMTP_Server"];
            string from = System.Configuration.ConfigurationManager.AppSettings["SMTP_From"];

            msg = "<html><body>" + msg + "</body></html>";

            // create new message.
            MailMessage message = new MailMessage(from, to, subject, msg);
            message.IsBodyHtml = true;


            SmtpClient client = new SmtpClient(server);
            // Credentials are necessary if the server requires the client 
            // to authenticate before it will send email on the client's behalf.
            client.Port = Int16.Parse(System.Configuration.ConfigurationManager.AppSettings["SMTP_Port"]);

            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(
                from,
                System.Configuration.ConfigurationManager.AppSettings["SMTP_Password"]
                //"zadyxikfsutihqgi"
                );
            client.EnableSsl = true;

            try {

                client.Send(message);
                Log_Insert(userId, "AspNetUsers", "SEND EMAIL", true, "Operazione conclusa con successo", "", "", "", "");
            }
            catch (Exception ex) {
                
                // set log.
                Log_Insert(userId, "AspNetUsers", "SEND EMAIL", false, ex.Message  + " " + ex.StackTrace  , "", "", "", "");
                string msgEx = "null";
                if(ex.InnerException != null) {
                    msgEx = ex.InnerException.Message;
                    Log_Insert(userId, "AspNetUsers", "SEND EMAIL", false, msgEx, "", "", "", "");

                }

                Log_Insert(userId, "AspNetUsers", "SEND EMAIL", false, ex.Message  + " " + ex.InnerException.Message  , "", "", "", "");
                //Log_Insert(userId, "AspNetUsers", "SEND EMAIL", false, ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : "" ) + " " + ex.StackTrace  , "", "", "", "");

            }
        }

    }
}
