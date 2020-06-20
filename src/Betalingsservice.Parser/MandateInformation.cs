using System;

namespace Betalingsservice.Parser
{
    public class MandateInformation
    {
        public string PbsCreditorNo { get; set; }
        public string TransactionCode { get; set; }
        public MandateTransaction Transaction => (MandateTransaction)Enum.Parse(typeof(MandateTransaction), TransactionCode.TrimStart('0'));
        
        public string DebtorGroupNo { get; set; }
        public string DebtorCustomerNo { get; set; }
        public string MandateNo { get; set; }

        public DateTime Date { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
