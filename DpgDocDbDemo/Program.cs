﻿using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DpgDocDbDemo
{
    class Program
    {
        private const string DATABASEID = "DpgDocDbDemo";

        public static void Main(string[] args)
        {
            try
            {
                using (var client = new DocumentClient(
                    Properties.Settings.Default.Uri,
                    Properties.Settings.Default.AuthKey))
                {
                    while (true)
                    {
                        Console.Clear();

                        Console.WriteLine("(1) Database Management Demo");
                        Console.WriteLine("(2) Collection Management Demo");
                        Console.WriteLine("(3) Document Management Demo");

                        Console.WriteLine();
                        Console.Write("Run a demo by number, or (Q)uit...");

                        var cki = Console.ReadKey(true);

                        Console.Clear();

                        switch (cki.Key)
                        {
                            case ConsoleKey.D1:
                                new DatabaseManagement().RunAsync().Wait();
                                break;
                            case ConsoleKey.D2:
                                new CollectionManagement().RunAsync().Wait();
                                break;
                            case ConsoleKey.D3:
                                new DocumentManagement().RunAsync().Wait();
                                break;
                            case ConsoleKey.Q:
                                return;
                            case ConsoleKey.E:
                                throw new Exception("Ooops!");
                            default:
                                break;
                        }
                    }
                }
            }
            catch (DocumentClientException dce)
            {
                var baseException = dce.GetBaseException();

                Console.WriteLine(
                    "Message: {0}, BaseMessage: {1}, StatudCode: {2}",
                    dce.Message, baseException.Message, dce.StatusCode);

                AlertThenTerminate();
            }
            catch (Exception e)
            {
                var baseException = e.GetBaseException();

                Console.WriteLine("Message: {0}, BaseMessage: {1}",
                    e.Message, baseException.Message);

                AlertThenTerminate();
            }
        }

        private static void AlertThenTerminate()
        {
            Console.WriteLine();
            Console.Write("Press any key to terminate the program...");

            Console.ReadKey(true);
        }

        private static async Task RunAsync(DocumentClient client)
        {
            var databaseId = "Test";

            //Try get a database
            var database = client.CreateDatabaseQuery().Where(
                db => db.Id == databaseId).AsEnumerable().FirstOrDefault();

            // Check if a database was returned, If not then it was not found. 
            // Then create the database
            if (database == null)
            {
                database = await client.CreateDatabaseAsync(
                    new Database { Id = databaseId });

                Console.WriteLine("Created Database: Id - {0}; SelfLink:  {1}",
                    database.Id, database.SelfLink);
            }

            //List databases for an account
            var databases = await ListDatabasesAsync(client);

            foreach (var db in databases)
                Console.WriteLine(db);

            //Cleanup using the SelfLink property of the Database which we either retrieved or created
            //If you do not have this SelfLink property accessible and populated you would need to get the Database using the id, 
            //then read the SelfLink property from that. This SelfLink value never changes for a Database once created;
            //so it would be perfectly acceptable practice to cache the value or store this in your configuratiom files
            await client.DeleteDatabaseAsync(database.SelfLink);
        }

        /// <summary>
        /// This method uses a ReadDatabaseFeedAsync method to read a list of all databases on the account.
        /// It demonstrates a pattern for how to control paging and deal with continuations
        /// This should not be needed for reading a list of databases as there are unlikely to be many hundred,
        /// but this same pattern is introduced here and can be used on other ReadFeed methods.
        /// </summary>
        /// <returns>A List of Database entities</returns>
        private static async Task<List<Database>> ListDatabasesAsync(DocumentClient client)
        {
            string continuation = null;
            List<Database> databases = new List<Database>();

            do
            {
                FeedOptions options = new FeedOptions
                {
                    RequestContinuation = continuation,
                    MaxItemCount = 50
                };

                FeedResponse<Database> response =
                    await client.ReadDatabaseFeedAsync(options);

                foreach (Database db in response)
                {
                    databases.Add(db);
                }

                continuation = response.ResponseContinuation;
            }
            while (!string.IsNullOrEmpty(continuation));

            return databases;
        }
    }
}
