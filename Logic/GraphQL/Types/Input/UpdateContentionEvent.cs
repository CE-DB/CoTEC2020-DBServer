using System;

namespace CoTEC_Server.Logic.GraphQL.Types.Input
{
    public class UpdateContentionEvent
    {
        public string measure { get; set; }
        public DateTime? startDate { get; set; } = null;
        public DateTime? endDate { get; set; } = null;
        public string country { get; set; }
    }
}
