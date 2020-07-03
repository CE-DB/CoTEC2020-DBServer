using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class ContactPathology
    {
        public string PathologyName { get; set; }
        public string ContactId { get; set; }

        public  Contact Contact { get; set; }
        public  Pathology PathologyNameNavigation { get; set; }
    }
}
