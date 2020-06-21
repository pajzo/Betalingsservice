using System;

namespace Betalingsservice.Writer
{
    public class CancellationOfMandate
    {
        public string DebtorGroupNo { get; set; }
        public string DebtorCustomerNo { get; set; }
        public string MandateNo { get; set; }

        public DateTime CancellationDate { get; set; }
    }
}
