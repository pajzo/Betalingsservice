using Azure.Storage.Blobs;
using Betalingsservice.Parser;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Betalingsservice.TestConsole
{
    class Program
    {
        /// <summary>
        /// This is only demo program
        /// </summary>
        /// <param name="args"></param>
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
                            var paymentInfo = ParserClient.GetPaymentInformation(lines);
                            break;
                        case "603": // Mandate info
                            var mandateInfo = ParserClient.GetMandateInformation(lines);
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
