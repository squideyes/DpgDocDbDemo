using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DpgDocDbDemo
{
    public class CollectionManagement : DemoBase<CollectionManagement>
    {
        protected override async Task DoRunAsync()
        {
            const string WEREFOUND =
                "Collections found using \"{0}\" (via {1}):";

            Console.WriteLine(string.Format(
                WEREFOUND, "ReadDocumentCollectionFeedAsync", "Feed"));

            var collections = await GetItemsAsync(options =>
                Client.ReadDocumentCollectionFeedAsync(
                Database.CollectionsLink, options));

            foreach (var col in collections)
                Console.WriteLine(" - {0}", col.Id);

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

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

            WriteSeparator();

            Console.Write(
                "Deleting the \"{0}\" collection (for demo purposes, only)...",
                Collection.Id);

            await Client.DeleteDocumentCollectionAsync(Collection.SelfLink);

            Console.WriteLine("DELETED!");
        }
    }
}
