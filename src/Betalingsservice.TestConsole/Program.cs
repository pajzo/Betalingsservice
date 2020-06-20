using Azure.Storage.Blobs;
using Betalingsservice.Parser;
using System;
using System.IO;
using System.Linq;
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
                            var informationEvents = ParserClient.GetInformationEvents(lines);
                            break;
                        case "602": // Payment info
                            var paymentInfo = ParserClient.GetPaymentInformation(lines).ToList();
                            break;
                        case "603": // Mandate info
                            var mandateInfo = ParserClient.GetMandateInformation(lines).ToList();
                            break;
                        default:
                            Console.WriteLine("> Unsupported file type");
                            break;
                    }
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
    }
}
