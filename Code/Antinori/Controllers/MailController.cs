using Antinori.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Antinori.Controllers {

    // this class has to contain the code to send emails.
    public class MailController : ApplicationController {

        public MailController() {

        }

        public void SendEmail(string subject, string msg, string to) {
            // send an email.

            string server = System.Configuration.ConfigurationManager.AppSettings["SMTP_Username"];
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
            client.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["SMTP_From"], System.Configuration.ConfigurationManager.AppSettings["SMTP_Password"]);
            client.EnableSsl = true;
            
            try {
                client.Send(message);
            }
            catch (Exception ex) {
                string id = "";
                if(User != null) {
                    id = this.Dc.AspNetUsers_Get_ByName(User.Identity.Name).Id;
                }
                // set log.
                this.Log_Insert(id, "AspNetUsers", "SEND EMAIL", true, ex.Message, "", "", "", "");
           }
        }

    }
}
