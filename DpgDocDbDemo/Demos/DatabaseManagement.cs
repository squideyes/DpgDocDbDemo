using System;
using System.Threading.Tasks;

namespace DpgDocDbDemo
{
    public class DatabaseManagement : DemoBase
    {
        protected override async Task DoRunAsync()
        {
            Console.WriteLine("The following databases were found:");

            var databases = await GetItemsAsync(options => 
                Client.ReadDatabaseFeedAsync(options));

            foreach (var db in databases)
                Console.WriteLine("  - {0}", db.Id);
        }
    }
}
