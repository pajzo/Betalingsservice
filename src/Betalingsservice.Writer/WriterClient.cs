using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Betalingsservice.Writer
{
    public static class WriterClient
    {
        public static string RegistrationOfMandates(IEnumerable<RegistrationOfMandate> mandates)
        {
            throw new NotImplementedException();
        }

        public static string CancellationOfMandates(IEnumerable<CancellationOfMandate> mandates)
        {
            var fileText = "";

            // TODO header section

            var mandateLines = mandates.Select(x =>
            {
                var line = "";
                line += "BS";
                line += "042";
                line += RightAlignedWithZero(x.PbsCreditorNo, 8);
                line += "0257"; // Cancellation of mandate due to cessation of customer relationship
                line += "000";
                line += RightAlignedWithZero(x.DebtorGroupNo,5);
                line += LeftAlignedWithSpaces(x.DebtorCustomerNo.ToUpper(), 15);
                line += RightAlignedWithZero(x.MandateNo, 9);
                line += RightAlignedWithZero("", 6);

                return line;
            });

            fileText += string.Join(Environment.NewLine, mandateLines);

            // TODO footer section

            return fileText;
        }

        internal static string LeftAlignedWithSpaces(string text, int length)
        {
            return text.PadLeft(length, ' ');
        }

        internal static string RightAlignedWithZero(string text, int length)
        {
            return text.PadRight(length, '0');
        }
    }
}
