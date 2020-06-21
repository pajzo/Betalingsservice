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

        public static string CancellationOfMandates(GeneralInfo generalInfo, IEnumerable<CancellationOfMandate> mandates)
        {
            var fileText = "";

            var fileHeader = "";
            fileHeader += "BS";
            fileHeader += "002";
            fileHeader += RightAlignedWithZero(generalInfo.DataSupplierCvrNo, 8); // CVR no. of the data supplier.
            fileHeader += LeftAlignedWithSpaces(generalInfo.DataSupplierSubsystemCode, 3); // Data supplier subsystem code
            fileHeader += "0605"; // Data Delivery 0605 (Change and cancellation of mandates)
            fileHeader += LeftAlignedWithSpaces(generalInfo.SerialNumber, 10); // Serial number as chosen
            fileHeader += LeftAlignedWithSpaces("", 19); // Filler
            fileHeader += DateTime.Now.ToString("ddMMyy");
            fileHeader += LeftAlignedWithSpaces("", 73); // Filler

            var sectionStart = "";
            sectionStart += "BS";
            sectionStart += "012";
            sectionStart += RightAlignedWithZero(generalInfo.PbsCreditorNo, 8);
            sectionStart += "0126"; // 0126 (Cancellation of mandates)
            sectionStart += "000";
            sectionStart += LeftAlignedWithSpaces(generalInfo.DataSupplierUserIdentification, 15); // User’s identification with the Data Supplier
            sectionStart += LeftAlignedWithSpaces("", 9); // Filler
            sectionStart += RightAlignedWithZero("", 6); // Filler
            sectionStart += LeftAlignedWithSpaces("", 78); // Filler

            var mandateLines = mandates.Select(x =>
            {
                var line = "";
                line += "BS";
                line += "042";
                line += RightAlignedWithZero(generalInfo.PbsCreditorNo, 8);
                line += "0257"; // Cancellation of mandate due to cessation of customer relationship
                line += "000";
                line += RightAlignedWithZero(x.DebtorGroupNo, 5);
                line += LeftAlignedWithSpaces(x.DebtorCustomerNo.ToUpper(), 15);
                line += RightAlignedWithZero(x.MandateNo, 9);
                line += RightAlignedWithZero("", 6);
                line += RightAlignedWithZero("", 6);
                line += RightAlignedWithZero("", 67);

                return line;
            });

            var sectionEnd = "";
            sectionEnd += "BS";
            sectionEnd += "092";
            sectionEnd += RightAlignedWithZero(generalInfo.PbsCreditorNo, 8);
            sectionEnd += "0126"; // 0126 (Cancellation of mandates)
            sectionEnd += LeftAlignedWithSpaces("", 9); // Filler
            sectionEnd += LeftAlignedWithSpaces(mandateLines.Count().ToString(), 11); // User’s identification with the Data Supplier
            sectionEnd += RightAlignedWithZero("", 26); // Filler
            sectionEnd += LeftAlignedWithSpaces("", 15); // Filler
            sectionEnd += RightAlignedWithZero("", 11); // Filler
            sectionEnd += LeftAlignedWithSpaces("", 39); // Filler

            var fileFooter = "";
            fileFooter += "BS";
            fileFooter += "992";
            fileFooter += RightAlignedWithZero(generalInfo.DataSupplierCvrNo, 8); // CVR no. of the data supplier.
            fileFooter += LeftAlignedWithSpaces(generalInfo.DataSupplierSubsystemCode, 3); // Data supplier subsystem code
            fileFooter += "0605"; // Data Delivery 0605 (Change and cancellation of mandates)
            fileFooter += RightAlignedWithZero("1", 11); // Number of sections in the delivery
            fileFooter += RightAlignedWithZero(mandateLines.Count().ToString(), 11); // Number of prefixed record type 042
            fileFooter += RightAlignedWithZero("", 86); // Filler

            fileText += fileHeader + Environment.NewLine;
            fileText += sectionStart + Environment.NewLine;
            fileText += string.Join(Environment.NewLine, mandateLines);
            fileText += Environment.NewLine;
            fileText += sectionEnd + Environment.NewLine;
            fileText += fileFooter;

            return fileText;
        }

        internal static string LeftAlignedWithSpaces(string text, int length)
        {
            if (text.Length > length)
                throw new Exception("Input text was too long");

            return text.PadRight(length, ' ');
        }

        internal static string RightAlignedWithZero(string text, int length)
        {
            if (text.Length > length)
                throw new Exception("Input text was too long");

            return text.PadLeft(length, '0');
        }
    }
}
