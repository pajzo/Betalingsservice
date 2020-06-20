using System;

namespace Betalingsservice.Parser
{
    public class PaymentInformation
    {
        public string PbsCreditorNo { get; set; }
        public string TransactionCode { get; set; }
        public PaymentTransaction Transaction => (PaymentTransaction)Enum.Parse(typeof(PaymentTransaction), TransactionCode.TrimStart('0'));

        public string DebtorGroupNo { get; set; }
        public string DebtorCustomerNo { get; set; }
        public string MandateNo { get; set; }

        public DateTime Date { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? BookkeepingEntryDate { get; set; }

        public decimal Amount { get; set; }
        public decimal AmountPayment { get; set; }

        public string CreditorReference { get; set; }
    }
}
