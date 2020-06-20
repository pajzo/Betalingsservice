using System;

namespace Betalingsservice.Parser
{
    public class InformationMandateRegistration
    {
        public string PbsCreditorNo { get; set; }
        public string TransactionCode { get; set; }

        public string TextNumber { get; set; }
        public InformationListType InformationListType => (InformationListType)Enum.Parse(typeof(InformationListType), TextNumber.Substring(1, 1));

        public string DebtorGroupNo { get; set; }
        public string DebtorCustomerNo { get; set; }        
        public string MandateNo { get; set; }

        public DateTime? Date { get; set; }
    }
}
