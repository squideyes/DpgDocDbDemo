using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DpgDocDbDemo
{
    public abstract class DemoBase
    {
        private const string DATABASENAME = "DpgDocDbDemo";
        private const string COLLECTIONID = "Demo";

        public DocumentClient Client { get; private set; }
        public Database Database { get; private set; }
        public DocumentCollection Collection { get; private set; }

        public async Task RunAsync()
        {
            try
            {
                using (Client = new DocumentClient(
                    Properties.Settings.Default.Uri,
                    Properties.Settings.Default.AuthKey))
                {
                    Database = await Client.
                        GetOrCreateDatabaseAsync(DATABASENAME);

                    try
                    {
                        Console.WriteLine();

                        Collection = await Client.
                            GetOrCreateCollectionAsync(
                            Database, COLLECTIONID);

                        Console.WriteLine();

                        DoRunAsync().Wait();
                    }
                    finally
                    {
                        Client.DeleteDatabaseAsync(Database).Wait();
                    }
                }
            }
            catch (DocumentClientException e)
            {
                Console.WriteLine();

                Console.WriteLine(
                    "Message: {0}, BaseMessage: {1}, StatudCode: {2}",
                    e.Message, e.GetBaseException().Message, e.StatusCode);
            }
            catch (Exception e)
            {
                Console.WriteLine();

                Console.WriteLine("Message: {0}, BaseMessage: {1}",
                    e.Message, e.GetBaseException().Message);
            }
            finally
            {
                Console.WriteLine();
                Console.Write("End of demo; press any key to continue...");

                Console.ReadKey(true);
            }
        }

        protected abstract Task DoRunAsync();

        protected async Task<List<T>> GetItemsAsync<T>(
            Func<FeedOptions, Task<FeedResponse<T>>> func)
        {
            string continuation = null;

            var items = new List<T>();

            do
            {
                var options = new FeedOptions
                {
                    MaxItemCount = 50,
                    RequestContinuation = continuation
                };

                var response = await func(options);

                foreach (var db in response)
                    items.Add(db);

                continuation = response.ResponseContinuation;
            }
            while (!string.IsNullOrEmpty(continuation));

            return items;
        }
    }
}
