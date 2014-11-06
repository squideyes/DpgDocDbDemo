using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DpgDocDbDemo
{
    public abstract class DemoBase<T> where T : DemoBase<T>
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
                WriteHeader(typeof(T), "Setup", false, true);

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

                        WriteHeader(typeof(T), "Demo", true, true);

                        DoRunAsync().Wait();
                    }
                    finally
                    {
                        WriteHeader(typeof(T), "Cleanup", true, true);

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
                WriteSeparator(true, 3);

                WriteColor("END-OF-DEMO", ConsoleColor.Cyan);

                Console.Write(": ");

                WriteColor("Press any key to continue...",
                    ConsoleColor.Yellow);

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

        protected void WriteHeader(Type type, string header,
            bool blankLineBefore = false, bool blankLineAfter = false)
        {
            var typeName = typeof(T).Name;

            var title = typeof(T).Name + " / " + header;

            if (blankLineBefore)
                Console.WriteLine();

            WriteSeparator(false);

            Console.Write("== ");

            WriteColor(typeName, ConsoleColor.Cyan);

            Console.Write(" / ");

            WriteColor(header, ConsoleColor.Yellow);

            Console.Write(" ");

            Console.WriteLine(new string('=', 79 -
                typeName.Length - header.Length - 7));

            WriteSeparator(false);

            if (blankLineAfter)
                Console.WriteLine();
        }

        public void WriteSeparator(
            bool addBlankLines = true, int count = 1)
        {
            if (addBlankLines)
                Console.WriteLine();

            for (int i = 0; i < count; i++)
                Console.WriteLine(new string('=', 79));

            if (addBlankLines)
                Console.WriteLine();
        }

        private void WriteColor(string text, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;

            Console.ForegroundColor = color;

            Console.Write(text);

            Console.ForegroundColor = oldColor;
        }
    }
}
