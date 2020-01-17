using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Antinori.Models;

namespace Antinori.Controllers {

    public class ApplicationController : Controller {   

        private AntinoriEntities _dc = new AntinoriEntities();

        public AntinoriEntities Dc {
            get { return _dc; }
        }
      
        
        #region Utility_Generali
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior) {
            return new JsonResult() {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public List<string> GetRenderPartialView(Controller controller, string view, object oggetto) {
            List<string> html = new List<string>();
            html.Add(RenderPartialViewToString(controller, view, oggetto));
            return html;
        }


        public static string RenderPartialViewToString(Controller thisController, string viewName, object model)
        {
            // assign the model of the controller from which this method was called to the instance of the passed controller (a new instance, by the way)
            thisController.ViewData.Model = model;

            // initialize a string builder
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(thisController.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(thisController.ControllerContext, viewResult.View, thisController.ViewData, thisController.TempData, sw);

                viewResult.View.Render(viewContext, sw);
                return sw.ToString();
            }
        }


        protected string salvaAllegato(HttpPostedFileBase file, string folderAbsolute, string folderRelative)
        {
            if (file != null && file.FileName != null)
            {
                DirectoryInfo di = new DirectoryInfo(folderAbsolute);
                di.Create();
                string nomeFile = file.FileName;
                file.SaveAs(di.FullName + "/" + nomeFile);
                return ";" + folderRelative + nomeFile;
            }
            return "";
        }


        public DateTime ProssimaData_Get(DateTime data, int ripetiDopo)
        {
            switch (ripetiDopo)
            {
                case 30:
                    return data.AddMonths(1);
                case 90:
                    return data.AddMonths(3);
                case 180:
                    return data.AddMonths(6);
                case 365:
                    return data.AddYears(1);
                case 730:
                    return data.AddYears(2);
                default:
                    return data.AddDays(ripetiDopo);
            }
        }

        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public bool StringNotNull(string campo)
        {
            return campo != null && campo.Length > 0 && campo.Trim().Length > 0;
        }

        public string gestisciErrore(string str, Exception ex)
        {
            string esito = str;
            if (ex != null)
                str += " :" + ex.Message;
            return "Attenzione: si è verificato un errore durante l'operazione: " + str;
        }

        public string gestisciErrore(string str)
        {
            return gestisciErrore(str, null);
        }

        


        #endregion

        #region IMAGE

        protected byte[] imageToByteArray(System.Drawing.Image imageIn) {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public Image byteArrayToImage(byte[] byteArrayIn) {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public byte[] Base64ToByteArray(string base64String) {
            // Convert Base64 String to byte[]
            base64String = base64String.Replace("data:image/png;base64,", "");
            base64String = base64String.Replace("data:image/jpeg;base64,", "");
            byte[] imageBytes = Convert.FromBase64String(base64String);
            return imageBytes;
        }

        public string Base64ToStringUTF8(string encodedstring) {
            byte[] Data = Base64ToByteArray(encodedstring);
            return Encoding.UTF8.GetString(Data);
        }

        public MemoryStream Base64ToStream(string base64String) {
            byte[] imageBytes = Base64ToByteArray(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            return ms;
        }

        public Image Base64ToImage(string base64String) {
            MemoryStream ms = Base64ToStream(base64String);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        public string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format) {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        #endregion

       
        #region Logs
        public void Log_Insert(string rif_Id, string rif_Tabella, string operazioneTipo, bool operazioneRiuscita, string operazioneEsito, string dettagli1, string dettagli2, string dettagli3, string dettagli4) {
            Logs log = new Logs();
            log.Operazione_Data = DateTime.Now;
            log.Operazione_Username = User.Identity.Name;
            log.Operazione_Dettagli1 = dettagli1;
            log.Operazione_Dettagli2 = dettagli2;
            log.Operazione_Dettagli3 = dettagli3;
            log.Operazione_Dettagli4 = dettagli4;
            log.Operazione_Tipo = operazioneTipo;
            log.Operazione_Riuscita = operazioneRiuscita;
            log.Operazione_Esito = operazioneEsito;
            log.Rif_Id = rif_Id;
            log.Rif_Tabella = rif_Tabella;
            this.Dc.Logs_Insert(log);
        }

        public void Log_Insert(string rif_Id, string rif_Tabella, string operazioneTipo, bool operazioneRiuscita, string operazioneEsito){
            Log_Insert(rif_Id, rif_Tabella, operazioneTipo, operazioneRiuscita, operazioneEsito, "", "", "", "");
        }
        #endregion
    }
}