using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Betalingsservice.Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            var blobServiceClient = new BlobServiceClient("UseDevelopmentStorage=true");

            var blobContainerClient = blobServiceClient.GetBlobContainerClient("betex");
            blobContainerClient.CreateIfNotExists();

            var pendingFiles = blobContainerClient.GetBlobs(); // This is only for demo/testing - in real life we got triggerede for every new file in the blob container
            foreach (var file in pendingFiles)
            {
                var fileInfo = new FileInfo(file.Name);

                Console.WriteLine(fileInfo.Name);

                // Data file
                if (fileInfo.Name.StartsWith("D"))
                {
                    var fileTypeRegex = Regex.Match(fileInfo.Name, @"-(\d{3}).(BS\d)");
                    var fileType = fileTypeRegex.Groups[1].Value;
                    var bsSubSystem = fileTypeRegex.Groups[2].Value;

                    Console.WriteLine($"> File type {fileType} for {bsSubSystem}");

                    var blobClient = blobContainerClient.GetBlobClient(file.Name);

                    using var downloadStream = new MemoryStream();
                    blobClient.DownloadTo(downloadStream);
                    var content = Encoding.UTF8.GetString(downloadStream.ToArray());
                    var lines = content.Split(Environment.NewLine);

                    switch (fileType)
                    {
                        case "621": // Info file
                            Parse621(lines);
                            break;
                        case "602": // Payment info
                            var paymentInfo = GetPaymentInformation(lines);
                            break;
                        case "603": // Mandate info
                            var mandateInfo = GetMandateInformation(lines);
                            break;
                        default:
                            Console.WriteLine("> Unsupported file type");
                            break;
                    }

                    // blobClient.SetAccessTier(AccessTier.Cool);
                }
                else
                {
                    Console.WriteLine("> Skipping file");
                }

                Console.WriteLine(" ");

            }

            Console.WriteLine("- The end -");
            Console.ReadKey();
        }

        static IEnumerable<MandateInformation> GetMandateInformation(string[] fileLines)
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

        static IEnumerable<PaymentInformation> GetPaymentInformation(string[] fileLines)
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

        static DateTime? ParseNullableDate(string text)
        {
            if (text == "000000")
                return null;

            return ParseDate(text);
        }

        static DateTime ParseDate(string text)
        {
            return DateTime.ParseExact(text, "ddMMyy", null);
        }

        /// <summary>
        /// TODO REFACTOR!
        /// </summary>
        /// <param name="fileLines"></param>
        static void Parse621(string[] fileLines)
        {
            foreach (var line in fileLines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var dataRecordType = line.Substring(2, 3);
                Console.WriteLine($"{dataRecordType}");

                if (dataRecordType == "022")
                {
                    var pbsCreditorNo = line.Substring(5, 8);
                    var transactionCode = line.Substring(13, 4);

                    var debtorGroup = line.Substring(20, 5);
                    var debtorCustomerNo = line.Substring(25, 15);
                    var debtorMandateNo = line.Substring(40, 9);
                    var effectiveDate = line.Substring(49, 6);
                    var textNumber = line.Substring(114, 6); // Note: Maybe only parse error 

                    Console.WriteLine($"{pbsCreditorNo} - {transactionCode} - {debtorGroup} - {debtorCustomerNo} - {debtorMandateNo} - {effectiveDate} - {textNumber}");
                }

            }

        }
    }
}
