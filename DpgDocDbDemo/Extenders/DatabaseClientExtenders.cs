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

            IEnumerable<Database> query = from db in client.CreateDatabaseQuery()
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
    }
}
