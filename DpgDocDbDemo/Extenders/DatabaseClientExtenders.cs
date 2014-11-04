using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DpgDocDbDemo
{
    public static partial class Extenders
    {
        public static async Task<Database> GetOrCreateDatabaseAsync(
            this DocumentClient client, string id)
        {
            Console.Write("Does the \"{0}\" database exists...", id);

            IEnumerable<Database> query =
                from db in client.CreateDatabaseQuery()
                where db.Id == id
                select db;

            var database = query.FirstOrDefault();

            if (database == null)
            {
                Console.WriteLine("NO!");

                Console.Write("Creating the \"{0}\" database...", id);

                database = await client.CreateDatabaseAsync(
                    new Database { Id = id });

                Console.WriteLine("CREATED!");
            }
            else
            {
                Console.WriteLine("YES!");
            }

            return database;
        }

        public static async Task DeleteDatabaseAsync(
            this DocumentClient client, Database database)
        {
            Console.WriteLine();

            Console.Write("Deleting the \"{0}\" database...", database.Id);
            
            await client.DeleteDatabaseAsync(database.SelfLink);

            Console.WriteLine("DELETED!");
        }

        public static async Task<DocumentCollection> GetOrCreateCollectionAsync(
            this DocumentClient client, Database database, string id)
        {
            Console.Write("Does the \"{0}\" collection exists...", id);

            IEnumerable<DocumentCollection> query =
                from col in client.CreateDocumentCollectionQuery(
                    database.CollectionsLink)
                where col.Id == id
                select col;

            var collection = query.FirstOrDefault();

            if (collection == null)
            {
                Console.WriteLine("NO!");

                Console.Write("Creating the \"{0}\" collection...", id);

                collection = await client.CreateDocumentCollectionAsync(
                    database.SelfLink, new DocumentCollection { Id = id });

                Console.WriteLine("CREATED!");
            }
            else
            {
                Console.WriteLine("YES!");
            }

            return collection;
        }
    }
}
