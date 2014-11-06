using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpgDocDbDemo
{
    public class DocumentManagement : DemoBase<DocumentManagement>
    {
        private const string COLLECTIONID = "Test";

        protected override async Task DoRunAsync()
        {
            await DemoPOCOs();
            await DemoDynamics();
            await DemoStreams();
            await DemoDocumentExtensions();
            await DemoAttachments();
        }

        private async Task DemoPOCOs()
        {
            var orders = new List<object>();

            orders.Add(new SalesOrder1
            {
                Id = "POCO1",
                PurchaseOrderNumber = "PO18009186470",
                OrderDate = new DateTime(2005, 7, 1),
                AccountNumber = "10-4020-000510",
                SubTotal = 419.4589m,
                TaxAmt = 12.5838m,
                Freight = 472.3108m,
                TotalDue = 985.018m,
                Items = new[]
                {
                    new SalesOrderDetail1
                    {
                        OrderQty = 1,
                        ProductId = 760,
                        UnitPrice = 419.4589m,
                        LineTotal = 419.4589m
                    }
                },
            });

            orders.Add(new SalesOrder2
            {
                Id = "POCO2",
                PurchaseOrderNumber = "PO15428132599",
                OrderDate = new DateTime(2005, 7, 1),
                DueDate = new DateTime(2005, 7, 13),
                ShippedDate = new DateTime(2005, 7, 8),
                AccountNumber = "10-4020-000646",
                SubTotal = 6107.0820m,
                TaxAmt = 586.1203m,
                Freight = 183.1626m,
                DiscountAmt = 1982.872m,            
                TotalDue = 4893.3929m,
                Items = new[]
                {
                    new SalesOrderDetail2
                    {
                        OrderQty = 3,
                        ProductCode = "A-123",      
                        ProductName = "Product 1",  
                        CurrencySymbol = "$",       
                        CurrencyCode = "USD",       
                        UnitPrice = 17.1m,          
                        LineTotal = 5.7m
                    },
                    new SalesOrderDetail2
                    {
                        OrderQty = 1,
                        ProductCode = "B-432",
                        ProductName = "Product 2",
                        CurrencySymbol = "$",
                        CurrencyCode = "NZD",
                        UnitPrice = 2039.994m,
                        LineTotal = 2039.994m
                    },
                    new SalesOrderDetail2
                    {
                        OrderQty = 1,
                        ProductCode = "C-2312S",
                        ProductName = "Product 3",
                        CurrencySymbol = "R",
                        CurrencyCode = "ZAR",
                        UnitPrice = 2024.994m,
                        LineTotal = 2024.994m
                    },
                }
            });

            Console.WriteLine("Created SalesOrder documents:");

            foreach (var order in orders)
            {
                var created = await Client.CreateDocumentAsync(
                    Collection.SelfLink, order);

                Console.WriteLine(" - " + created.Resource.Id);
            }

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.Write("Updating ShippedDate (POCO1)...");

            dynamic doc = Client.CreateDocumentQuery<Document>(
                Collection.SelfLink).Where(d => d.Id == "POCO1").
                AsEnumerable().FirstOrDefault();

            var createdOrder = doc;

            createdOrder.ShippedDate = DateTime.UtcNow;

            var updatedOrder = await Client.
                ReplaceDocumentAsync(doc.SelfLink, createdOrder);

            Console.WriteLine("DONE!");
        }

        private async Task DemoDynamics()
        {
            const string ORDERID = "DYN01";

            WriteSeparator();

            Console.Write("Created dynmaic order document ({0}): ", ORDERID);

            dynamic order = new
            {
                id = ORDERID,
                purchaseOrderNumber = "PO18009186470",
                orderDate = DateTime.UtcNow,
                total = 5.95,
            };

            var original = await Client.CreateDocumentAsync(
                Collection.SelfLink, order);

            Console.WriteLine("DONE!");

            ///////////////////////////////////////////////////////////////////

            Console.WriteLine();
            Console.Write("Updating ShippedDate ({0})...", ORDERID);

            dynamic updated = (await Client.
                ReadDocumentAsync(original.Resource.SelfLink)).Resource;

            updated.ShippedDate = DateTime.UtcNow;

            await Client.ReplaceDocumentAsync(updated);

            Console.WriteLine("DONE!");
        }

        private async Task DemoStreams()
        {
            WriteSeparator();

            foreach (var fileName in Directory.GetFiles(
                @"..\..\Data\DocumentManagement\Data", "*.json"))
            {
                using (var fileStream = new FileStream(
                    fileName, FileMode.Open, FileAccess.Read))
                {
                    var doc = await Client.CreateDocumentAsync(
                        Collection.SelfLink, 
                        Resource.LoadFrom<Document>(fileStream));

                    Console.WriteLine(
                        "Created document from \"{0}\":\n{1}", 
                        fileName, doc.Resource);
                }
            }

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.Write("Reading \"JSON1\" document...");

            var json1 = Client.CreateDocumentQuery(Collection.SelfLink).
                Where(d => d.Id == "JSON1").AsEnumerable().First();

            var content = JsonConvert.SerializeObject(json1);

            Console.WriteLine("DONE; {0:N0} Bytes", content.Length);

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.Write("Replacing \"JSON1\" with a \"Cancelled\" order...");

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(
                "{\"id\": \"JSON1\",\"PurchaseOrderNumber\": \"PO18009186470\",\"Status\": \"Cancelled\"}")))
            {
                await Client.ReplaceDocumentAsync(json1.SelfLink, 
                    Resource.LoadFrom<Document>(stream));
            }

            Console.WriteLine("DONE!");
        }

        private async Task DemoDocumentExtensions()
        {
            const string ID = "DOC01";

            WriteSeparator();

            Console.Write("Creating a new SalesOrderDocument (ID: {0})...", ID);

            var doc01 = new SalesOrderDocument
            {
                Id = ID,
                PurchaseOrderNumber = "PO180091783420",
                OrderDate = new DateTime(2013, 7, 17),
                AccountNumber = "10-4020-000510",
                SubTotal = 419.4589m,
                TaxAmt = 12.5838m,
                Freight = 472.3108m,
                TotalDue = 985.018m,
                Items = new[]
                {
                    new SalesOrderDetail1
                    {
                        OrderQty = 1,
                        ProductId = 760,
                        UnitPrice = 419.4589m,
                        LineTotal = 419.4589m
                    }
                }
            };

            var created = await Client.CreateDocumentAsync(
                Collection.SelfLink, doc01);

            Console.WriteLine("CREATED!");

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.Write("Replacing \"DOC01\" with an updated SalesOrderDocument...");

            var read = (SalesOrderDocument)(dynamic)(
                await Client.ReadDocumentAsync(
                created.Resource.SelfLink)).Resource;

            read.ShipDate = DateTime.UtcNow;

            await Client.ReplaceDocumentAsync(read);

            Console.WriteLine("REPLACED!");
        }

        private async Task DemoAttachments()
        {
            WriteSeparator();

            Console.Write("Creating a new document...");

            dynamic documentWithAttachment = new
            {
                id = "PO1800243243470",
                CustomerId = 1092,
                TotalDue = 985.018m,
            };

            Document doc = await Client.CreateDocumentAsync(
                Collection.SelfLink, documentWithAttachment);

            Console.WriteLine("CREATED!");

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.Write("Attaching \"Text.txt\" to the document...");

            using (FileStream fileStream = new FileStream(
                @"..\..\Data\DocumentManagement\Attachments\Text.txt", FileMode.Open))
            {
                await Client.CreateAttachmentAsync(doc.AttachmentsLink, fileStream, 
                    new MediaOptions { ContentType = "text/plain", Slug = "Text.txt" });
            }

            Console.WriteLine("ATTACHED!");

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.Write("Retrieving attachment...");

            var attachment = Client.CreateAttachmentQuery(doc.SelfLink).
                AsEnumerable().FirstOrDefault();

            var content = await Client.ReadMediaAsync(attachment.MediaLink);

            var bytes = new byte[content.ContentLength];
            
            await content.Media.ReadAsync(bytes, 0, (int)content.ContentLength);
            
            var result = Encoding.UTF8.GetString(bytes);

            Console.WriteLine("RETRIEVED; {0:N0} Bytes", result.Length);
        }
    }
}
