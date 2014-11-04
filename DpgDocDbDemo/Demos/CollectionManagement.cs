using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DpgDocDbDemo
{
    public class CollectionManagement : DemoBase
    {
        private const string COLLECTIONID = "Test";

        protected override async Task DoRunAsync()
        {
            const string WEREFOUND =
                "Collections found using \"{0}\" (via {1}):";

            ///////////////////////////////////////////////////////////////////

            var collection = await Client.
                GetOrCreateCollectionAsync(Database, COLLECTIONID);

            ///////////////////////////////////////////////////////////////////

            Console.WriteLine();

            Console.WriteLine(string.Format(
                WEREFOUND, "ReadDocumentCollectionFeedAsync", "Feed"));

            var collections = await GetItemsAsync(options =>
                Client.ReadDocumentCollectionFeedAsync(
                Database.CollectionsLink, options));

            foreach (var col in collections)
                Console.WriteLine(" - {0}", col.Id);

            ///////////////////////////////////////////////////////////////////

            Console.WriteLine();

            Console.WriteLine(string.Format(
                WEREFOUND, "CreateDocumentCollectionQuery", "Linq to DocDB"));

            collections = Client.CreateDocumentCollectionQuery(
                Database.CollectionsLink).ToList();

            foreach (var col in collections)
            {
                Console.WriteLine();
                Console.WriteLine(col);
            }

            ///////////////////////////////////////////////////////////////////

            Console.WriteLine();

            Console.Write(
                "Deleting the \"{0}\" collection (for demo purposes, only)...", 
                COLLECTIONID);

            await Client.DeleteDocumentCollectionAsync(collection.SelfLink);

            Console.WriteLine("DELETED!");
        }
    }
}
