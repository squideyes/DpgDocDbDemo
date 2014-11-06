using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;

namespace DpgDocDbDemo
{
    class Program
    {
        private const string DATABASEID = "DpgDocDbDemo"; 

        public static void Main(string[] args)
        {
            Console.SetWindowSize(80, 75);

            try
            {
                using (var client = new DocumentClient(
                    Properties.Settings.Default.Uri,
                    Properties.Settings.Default.AuthKey))
                {
                    while (true)
                    {
                        Console.Clear();

                        Console.WriteLine("(1) Database Management");
                        Console.WriteLine("(2) Collection Management");
                        Console.WriteLine("(3) Document Management");
                        Console.WriteLine("(4) Queries");
                        Console.WriteLine("(5) Index Management");

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
                            case ConsoleKey.D4:
                                new Queries().RunAsync().Wait();
                                break;
                            case ConsoleKey.D5:
                                new IndexManagement().RunAsync().Wait();
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
    }
}
