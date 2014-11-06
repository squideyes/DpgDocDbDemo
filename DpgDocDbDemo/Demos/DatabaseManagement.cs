using System;
using System.Threading.Tasks;

namespace DpgDocDbDemo
{
    public class DatabaseManagement : DemoBase<DatabaseManagement>
    {
        protected override async Task DoRunAsync()
        {
            var databases = await GetItemsAsync(options => 
                Client.ReadDatabaseFeedAsync(options));

            Console.WriteLine("The following databases were found:");

            foreach (var db in databases)
                Console.WriteLine(" - {0}", db.Id);
        }
    }
}
