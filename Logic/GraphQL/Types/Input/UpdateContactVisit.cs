using System;

namespace CoTEC_Server.Logic.GraphQL.Types.Input
{
    public class UpdateContactVisit
    {
        public string? patientId { get; set; }
        public string? contactId { get; set; }
        public DateTime? visitDate { get; set; } = null;
    }
}
