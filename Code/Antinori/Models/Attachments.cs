//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Antinori.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Attachments
    {
        public string Id { get; set; }
        public string PublicId { get; set; }
        public string TopicDate { get; set; }
        public Nullable<System.DateTime> ChronicDate { get; set; }
        public string PhotoPath { get; set; }
        public string Nature { get; set; }
        public string Regesto { get; set; }
        public string Seals { get; set; }
        public string ConservationInstitute { get; set; }
        public string Note { get; set; }
        public string PageId { get; set; }
    
        public virtual Pages Pages { get; set; }
    }
}
