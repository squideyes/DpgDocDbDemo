using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DpgDocDbDemo
{
    public class BulkUpload : DemoBase<BulkUpload>
    {
        protected override async Task DoRunAsync()
        {
            const string BASEPATH = @"..\..\Data\BulkUpload";
            const string COLLECTION = "Movies";

            Console.Write("Adding JSON documents to the \"Movies\" collection");

            var startedOn = DateTime.UtcNow;

            var fileNames = Directory.GetFiles(BASEPATH, "*.json").ToList();

            var actionBlock = new ActionBlock<string>(
                async fileName =>
                {
                    dynamic movie = JsonConvert.DeserializeObject(
                        File.ReadAllText(fileName));

                    await Client.CreateDocumentAsync(Collection.SelfLink, movie);

                    Console.Write('.');
                },
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount * 8
                });

            fileNames.ForEach(fileName => actionBlock.Post(fileName));

            actionBlock.Complete();

            await actionBlock.Completion;

            Console.WriteLine();
            Console.WriteLine();

            var elapsed = DateTime.UtcNow - startedOn;

            Console.WriteLine(
                "Added {0:N0} JSON documents to the {1} collection in {2}",
                fileNames.Count, COLLECTION, elapsed);
        }
    }
}
