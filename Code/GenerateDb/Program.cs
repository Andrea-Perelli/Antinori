﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GenerateDb {

    public class Program {

        static void Main(string[] args) {

            //******** CONFIG DATA ********
            string photoPath = @"C:\Users\g\Desktop\Biblioteca Tommasi\Antinori";
            string author = "Antinori";
            string bookName = "Antinori";
            string description = "";
            string endDate = "";

            // data context.
            AntinoriEntities dc = new AntinoriEntities();

            // create new opera.
            Books book = new Books() {
                Id = Guid.NewGuid().ToString(),
                Author = author,
                Title = bookName,
                Description = description,
                EndDate = endDate,
            };

            

            // open directories
            List<string> sections = Directory.GetDirectories(photoPath).ToList();

            foreach(string section in sections) {
                string sectionName = section.Split('\\').Last();

                // create section.
                Sections sec = new Sections() {
                    Id = Guid.NewGuid().ToString(),
                    Name = sectionName,
                    Description = "",
                    Book = book.Id
                };

                // find subsections.
                List<string> subSections = Directory.GetDirectories(photoPath + '\\' + sectionName).ToList();

                foreach(string subSection in subSections) {
                    string subSectionName = subSection.Split('\\').Last();

                    // create subsection.
                    SubSections subSec = new SubSections() {
                        Id = Guid.NewGuid().ToString(),
                        Name = subSectionName,
                        Description = "",
                        Section = sec.Id
                    };

                    // find page. 
                    var pages = Directory.GetFiles(photoPath + '\\' + sectionName + '\\' + subSectionName + '\\' +"JPEG");

                    foreach(String page in pages) {
                        string pageName = page.Split('\\').Last();
                        pageName = pageName.Substring(0, pageName.Length - 4).Split('.').Last(); // removing .jpg

                        // ignore first (is the subsetion number)
                        string[] data = pageName.Split('_');

                        string numericOrder = data[1];
                        string completeOrder = "";
                        string title = "";
                        if(Int32.TryParse(data[2][0].ToString(),out int res)) {
                            completeOrder = data[2];
                        }
                        else {
                            title = data[2];
                            completeOrder = data[2];
                            if(data.Length > 3) {
                                completeOrder = data[3];
                                if(data.Length == 5) {
                                    completeOrder = data[2] + " " + data[3] + " " + data[4];
                                }
                                
                            }
                        }
                        if(data.Length == 6) {

                        }

                        string a = "I-1_0001_PiattoAnteriore";
                        string b = "I-1_0002_ControguardiaAnteriore";
                        string aa = "I-1_0003_CartaDiGuardiaAnteriore_r";
                        string c = "II_0028_5v";
                        string e = "I-1_0016_Indice_bis_v";
                        string d = "LIII_0194_Taglio";

                        Pages p = new Pages() {
                            Id = Guid.NewGuid().ToString(),
                            BookTitle = book.Title,
                            Description = "",
                            Editor = "",
                            PhotoPath = page,
                            SectionName = sec.Name,
                            SubSection = subSec.Id,
                            NumericOrder = Convert.ToInt32(numericOrder),
                            CompleteOrder = completeOrder,
                            Title = title,                  
                            BigPhotoPath = page.Replace("JPEG","Tiff"),
                        };
                        // add page.
                        subSec.Pages.Add(p);
                    }

                    // add sub-section to section.
                    sec.SubSections.Add(subSec);
                }
                // add section to book.
                book.Sections.Add(sec);
            }

            // add book.
            dc.Books.Add(book);
            int esito = dc.SaveChanges();
        }
    }
}