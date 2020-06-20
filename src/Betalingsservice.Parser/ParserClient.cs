using System;
using System.Collections.Generic;
using System.Linq;

namespace Betalingsservice.Parser
{
    public static class ParserClient
    {
        public static IEnumerable<MandateInformation> GetMandateInformation(string[] fileLines)
        {
            var mandateInfoList = fileLines
                .Where(x => !string.IsNullOrEmpty(x))
                .Where(x => x.Substring(2, 3) == "042") // Data record type
                .Select(line => new MandateInformation
                {
                    PbsCreditorNo = line.Substring(5, 8),
                    TransactionCode = line.Substring(13, 4),
                    DebtorGroupNo = line.Substring(20, 5),
                    DebtorCustomerNo = line.Substring(25, 15),
                    MandateNo = line.Substring(40, 9),
                    Date = ParseDate(line.Substring(49, 6)),
                    EndDate = ParseNullableDate(line.Substring(55, 6))
                });

            return mandateInfoList;
        }

        public static IEnumerable<PaymentInformation> GetPaymentInformation(string[] fileLines)
        {
            var paymentInfoList = fileLines
                .Where(x => !string.IsNullOrEmpty(x))
                .Where(x => x.Substring(2, 3) == "042") // Data record type
                .Select(line => new PaymentInformation
                {
                    PbsCreditorNo = line.Substring(5, 8),
                    TransactionCode = line.Substring(13, 4),
                    DebtorGroupNo = line.Substring(20, 5),
                    DebtorCustomerNo = line.Substring(25, 15),
                    MandateNo = line.Substring(40, 9),
                    Date = ParseDate(line.Substring(49, 6)),
                    PaymentDate = ParseNullableDate(line.Substring(103, 6)),
                    BookkeepingEntryDate = ParseNullableDate(line.Substring(109, 6)),
                    CreditorReference = line.Substring(69, 30),
                    Amount = decimal.Parse(line.Substring(56, 13)) / 100,
                    AmountPayment = decimal.Parse(line.Substring(115, 13)) / 100
                });

            return paymentInfoList;
        }

        internal static DateTime? ParseNullableDate(string text)
        {
            if (text == "000000")
                return null;

            return ParseDate(text);
        }

        internal static DateTime ParseDate(string text)
        {
            return DateTime.ParseExact(text, "ddMMyy", null);
        }
    }
}
