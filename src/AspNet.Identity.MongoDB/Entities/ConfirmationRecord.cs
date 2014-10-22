using System;

namespace AspNet.Identity.MongoDB.Entities 
{
    public class ConfirmationRecord
    {
        public ConfirmationRecord()
            : this(DateTime.UtcNow)
        {
        }

        public ConfirmationRecord(DateTime confirmedOn)
        {
            ConfirmedOn = confirmedOn;
        }

        public DateTime ConfirmedOn { get; private set; }
    }
}