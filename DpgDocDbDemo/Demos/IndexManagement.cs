﻿using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpgDocDbDemo.Demos
{
    public class IndexManagement:DemoBase
    {
        private const string COLLECTIONID = "Test";

        protected override async Task DoRunAsync()
        {
            // The default behavior when creating a DocumentDollection is 
            // creating a Hash index for all string and numeric fields. 
            // Hash indexes are compact and offer efficient performance for 
            // equality queries.  Let's have a look at some of the options 
            // available for controlling the indexing behavior of a collection

            var collection = await Client.
                GetOrCreateCollectionAsync(Database, COLLECTIONID);

            //await ExplicitlyExcludeFromIndex(Database.SelfLink);

            //await UseManualIndexing(collection);

            //await UseLazyIndexing(collection);

            //await UseRangeIndexes(collection);

            //await ExcludePathsFromIndex(collection);

            //await RangeScanOnHashIndex(collection);

            //await Client.DeleteDatabaseAsync(collection);
        }

        //private async Task ExplicitlyExcludeFromIndex(DocumentCollection collection)
        //{
        //    //There may be scenarios where you want to exclude a specific doc from the index even though all other 
        //    //documents are being indexed automatically. You can use an index directive to control this when you
        //    //create a document

        //    //Create a document collection with the default indexing policy (Automatically index everything)
        //    DocumentCollection collection = await Client.CreateDocumentCollectionAsync(Database.SelfLink, new DocumentCollection
        //    {
        //        Id = ConfigurationManager.AppSettings["CollectionId"]
        //    });

        //    //Create a document and query on it immediately, should work as this Collection is set to automatically index everyting
        //    Document created = await Client.CreateDocumentAsync(collection.SelfLink, new { id = "doc1", orderId = "order1" });

        //    //Query for document, should find it
        //    bool found = Client.CreateDocumentQuery(collection.SelfLink, "SELECT * FROM root r WHERE r.orderId='order1'").AsEnumerable().Any();

        //    //Now, create a document but this time explictly exclude it from the collection using IndexingDirective
        //    created = await Client.CreateDocumentAsync(collection.SelfLink, new { id = "doc2", orderId = "order2" }, new RequestOptions
        //    {
        //        IndexingDirective = IndexingDirective.Exclude
        //    });

        //    //Query for document, should not find it
        //    found = Client.CreateDocumentQuery(collection.SelfLink, "SELECT * FROM root r WHERE r.orderId='order2'").AsEnumerable().Any();

        //    //Read on document, should still find it
        //    Document document = await Client.ReadDocumentAsync(created.SelfLink);

        //    //Cleanup
        //    await Client.DeleteDocumentCollectionAsync(collection.SelfLink);
        //}

        //private async Task UseManualIndexing(string Database.SelfLink)
        //{
        //    //The default behavior for DocumentDB DocumentCollections is to automatically index every document written to it.
        //    //There are cases where you can want to turn-off automatic indexing on the collection
        //    //and selectively add only specific documents to the index. 

        //    var collection = new DocumentCollection
        //    {
        //        Id = ConfigurationManager.AppSettings["CollectionId"]
        //    };

        //    collection.IndexingPolicy.Automatic = false;

        //    collection = await Client.CreateDocumentCollectionAsync(Database.SelfLink, collection);

        //    //Create a dynamic document, with just a single property for simplicity, 
        //    //then query for document using that property and we should find nothing
        //    Document created = await Client.CreateDocumentAsync(collection.SelfLink, new { id = "doc1", orderId = "order1" });

        //    //This should be false as the document won't be in the index
        //    bool found = Client.CreateDocumentQuery(collection.SelfLink, "SELECT * FROM root r WHERE r.orderId='order1'").AsEnumerable().Any();

        //    //If we do a specific Read on the Document we will find it because it is in the collection
        //    Document doc = await Client.ReadDocumentAsync(created.SelfLink);

        //    //Now create a document, passing in an IndexingDirective saying we want to specifically index this document
        //    created = await Client.CreateDocumentAsync(collection.SelfLink, new { id = "doc2", orderId = "order2" }, new RequestOptions
        //    {
        //        IndexingDirective = IndexingDirective.Include
        //    });

        //    //Query for the document again and this time we should find it because we manually included the document in the index
        //    found = Client.CreateDocumentQuery(collection.SelfLink, "SELECT * FROM root r WHERE r.orderId='order2'").AsEnumerable().Any();

        //    //Cleanup collection
        //    await Client.DeleteDocumentCollectionAsync(collection.SelfLink);
        //}

        //private async Task UseLazyIndexing(string Database.SelfLink)
        //{
        //    //DocumentDB offers synchronous (consistent) and asynchronous (lazy) index updates. 
        //    //By default, the index is updated synchronously on each insert, replace or delete of a document to the collection. 
        //    //There are times when you might want to configure certain collections to update their index asynchronously. 
        //    //Lazy indexing boosts the write performance further and is ideal for bulk ingestion scenarios for primarily read-heavy collections
        //    //It is important to note that you might get inconsistent reads whilst the writes are in progress,
        //    //However once the write volume tapers off and the index catches up, then the reads continue as normal

        //    var collection = new DocumentCollection
        //    {
        //        Id = ConfigurationManager.AppSettings["CollectionId"]
        //    };

        //    collection.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
        //    collection = await Client.CreateDocumentCollectionAsync(Database.SelfLink, collection);

        //    await Client.DeleteDocumentCollectionAsync(collection.SelfLink);
        //}

        //private async Task UseRangeIndexes(string Database.SelfLink)
        //{
        //    //This example configures a collection to enable efficient comparison queries 
        //    //by setting the default index type to Range for all numeric values. 

        //    //The default precision for indexes is 5 bytes which works well for numeric fields like 
        //    //age, ids, hours, etc. If your number spans a large range of values (millions to billions), 
        //    //then consider increasing the precision to the maximum of 7 bytes. 
        //    //This is useful for fields like epoch timestamps, which are commonly used to represent datetimes in JSON

        //    var collection = new DocumentCollection
        //    {
        //        Id = ConfigurationManager.AppSettings["CollectionId"]
        //    };

        //    collection.IndexingPolicy.IncludedPaths.Add(new IndexingPath
        //    {
        //        IndexType = IndexType.Hash,
        //        Path = "/",
        //    });

        //    collection.IndexingPolicy.IncludedPaths.Add(new IndexingPath
        //    {
        //        IndexType = IndexType.Range,
        //        Path = @"/""shippedTimestamp""/?",
        //        NumericPrecision = 7
        //    });

        //    collection = await Client.CreateDocumentCollectionAsync(Database.SelfLink, collection);

        //    await Client.CreateDocumentAsync(collection.SelfLink, new
        //    {
        //        id = "doc1",
        //        shippedTimestamp = ConvertDateTimeToEpoch(DateTime.UtcNow)
        //    });
        //    await Client.CreateDocumentAsync(collection.SelfLink, new
        //    {
        //        id = "doc2",
        //        shippedTimestamp = ConvertDateTimeToEpoch(DateTime.UtcNow.AddDays(-7))
        //    });
        //    await Client.CreateDocumentAsync(collection.SelfLink, new
        //    {
        //        id = "doc3",
        //        shippedTimestamp = ConvertDateTimeToEpoch(DateTime.UtcNow.AddDays(-14))
        //    });
        //    await Client.CreateDocumentAsync(collection.SelfLink, new
        //    {
        //        id = "doc4",
        //        shippedTimestamp = ConvertDateTimeToEpoch(DateTime.UtcNow.AddDays(-30))
        //    });

        //    //now with our DateTime converted to an Epoch number and 
        //    //the IndexPath for createTimestamp set to Range with a higher precision
        //    //querying for items created in the last 10 days should be effecient
        //    var docs = Client.CreateDocumentQuery(collection.SelfLink, string.Format("SELECT * FROM root r WHERE r.shippedTimestamp >= {0} AND r.shippedTimestamp <= {1}",
        //            ConvertDateTimeToEpoch(DateTime.UtcNow.AddDays(-10)),
        //            ConvertDateTimeToEpoch(DateTime.UtcNow)
        //    ));

        //    foreach (var doc in docs)
        //    {
        //        Console.WriteLine(doc);
        //    }

        //    //Cleanup
        //    await Client.DeleteDocumentCollectionAsync(collection.SelfLink);
        //}

        //private async Task ExcludePathsFromIndex(string Database.SelfLink)
        //{
        //    bool found;
        //    dynamic dyn = new
        //    {
        //        id = "doc1",
        //        metaData = "meta",
        //        subDoc = new
        //        {
        //            searchable = "searchable",
        //            subSubDoc = new
        //            {
        //                someProperty = "value"
        //            }
        //        }
        //    };

        //    //The default behavior is for DocumentDB to index every attribute in every document.
        //    //There are times when a document contains large amounts of information, in deeply nested structures
        //    //that you know you will never search on. In extreme cases like this, you can exclude paths from the 
        //    //index to save on storage cost, improve write performance because there is less work that needs to 
        //    //happen on writing and also improve read performance because the index is smaller

        //    var collection = new DocumentCollection
        //    {
        //        Id = ConfigurationManager.AppSettings["CollectionId"]
        //    };

        //    //special manadatory path of "/" required to denote include entire tree
        //    collection.IndexingPolicy.IncludedPaths.Add(new IndexingPath { Path = "/" });

        //    collection.IndexingPolicy.ExcludedPaths.Add("/\"metaData\"/*");
        //    collection.IndexingPolicy.ExcludedPaths.Add("/\"subDoc\"/\"subSubDoc\"/\"someProperty\"/*");
        //    collection = await Client.CreateDocumentCollectionAsync(Database.SelfLink, collection);

        //    var created = await Client.CreateDocumentAsync(collection.SelfLink, dyn);

        //    //Querying for a document on either metaData or /subDoc/subSubDoc/someProperty > fail because they were excluded
        //    try
        //    {
        //        Client.CreateDocumentQuery(collection.SelfLink, String.Format("SELECT * FROM root r WHERE r.metaData='{0}'",
        //            "meta")).AsEnumerable().Any();
        //    }
        //    catch (Exception e)
        //    {
        //        var baseEx = (DocumentClientException)e.GetBaseException();
        //        if (baseEx.StatusCode != HttpStatusCode.BadRequest) { throw; }
        //    }

        //    try
        //    {
        //        found = Client.CreateDocumentQuery(collection.SelfLink, String.Format("SELECT * FROM root r WHERE r.subDoc.subSubDoc.someProperty='{0}'",
        //            "value")).AsEnumerable().Any();
        //    }
        //    catch (Exception e)
        //    {
        //        var baseEx = (DocumentClientException)e.GetBaseException();
        //        if (baseEx.StatusCode != HttpStatusCode.BadRequest) { throw; }
        //    }

        //    //Querying for a document using id, or even subDoc/searchable > succeed because they were not excluded
        //    found = Client.CreateDocumentQuery(collection.SelfLink, String.Format("SELECT * FROM root r WHERE r.id='{0}'", "doc1")).AsEnumerable().Any();

        //    if (!found) throw new ApplicationException("Should've found the document");

        //    found = Client.CreateDocumentQuery(collection.SelfLink, String.Format("SELECT * FROM root r WHERE r.subDoc.searchable='{0}'",
        //        "searchable")).AsEnumerable().Any();

        //    if (!found) throw new ApplicationException("Should've found the document");

        //    //Cleanup collection
        //    await Client.DeleteDocumentCollectionAsync(collection.SelfLink);

        //    //To exclude subDoc and anything under it add an ExcludePath of "/\"subDoc\"/*"
        //    collection = new DocumentCollection
        //    {
        //        Id = ConfigurationManager.AppSettings["CollectionId"]
        //    };

        //    //special manadatory path of "/" required to denote include entire tree
        //    collection.IndexingPolicy.IncludedPaths.Add(new IndexingPath { Path = "/" });

        //    collection.IndexingPolicy.ExcludedPaths.Add("/\"subDoc\"/*");
        //    collection = await Client.CreateDocumentCollectionAsync(Database.SelfLink, collection);

        //    //Query for /subDoc/searchable > fail because we have excluded the whole subDoc, and all its children.
        //    try
        //    {
        //        Client.CreateDocumentQuery(collection.SelfLink, String.Format("SELECT * FROM root r WHERE r.subDoc.searchable='{0}'",
        //            "searchable")).AsEnumerable().Any();
        //    }
        //    catch (Exception e)
        //    {
        //        var baseEx = (DocumentClientException)e.GetBaseException();
        //        if (baseEx.StatusCode != HttpStatusCode.BadRequest) { throw; }
        //    }

        //    //Cleanup
        //    await Client.DeleteDocumentCollectionAsync(collection.SelfLink);
        //}

        //private async Task RangeScanOnHashIndex(string Database.SelfLink)
        //{
        //    //When a range index is not available (i.e. Only hash or no index found on the path), comparisons queries can still 
        //    //can still be performed as scans using AllowScanInQuery request option using the .NET SDK
        //    //Warning: This was made an opt-in model by design. Scanning is an expensive operation and doing this 
        //    //         will have an impact on your RequstUnits and could result in other queries not being throttled.

        //    var collection = new DocumentCollection
        //    {
        //        Id = ConfigurationManager.AppSettings["CollectionId"]
        //    };

        //    collection.IndexingPolicy.IncludedPaths.Add(new IndexingPath { Path = "/" });
        //    collection.IndexingPolicy.ExcludedPaths.Add("/\"length\"/*");
        //    collection = await Client.CreateDocumentCollectionAsync(Database.SelfLink, collection);

        //    var doc1 = await Client.CreateDocumentAsync(collection.SelfLink, new { id = "dyn1", length = 10, width = 5, height = 15 });
        //    var doc2 = await Client.CreateDocumentAsync(collection.SelfLink, new { id = "dyn2", length = 7, width = 15 });
        //    var doc3 = await Client.CreateDocumentAsync(collection.SelfLink, new { id = "dyn3", length = 2 });

        //    //query for length > 5 - fail, this is a range based query on a Hash index only document
        //    try
        //    {
        //        bool found = Client.CreateDocumentQuery(collection.SelfLink, "SELECT * FROM root r WHERE r.length > 5").AsEnumerable().Any();
        //    }
        //    catch (Exception e)
        //    {
        //        var baseEx = (DocumentClientException)e.GetBaseException();
        //        if (baseEx.StatusCode != HttpStatusCode.BadRequest) { throw; }
        //    }

        //    //now add IndexingDirective and repeat query - expect success because now we are explictly allowing scans in a query 
        //    //using the EnableScanInQuery directive
        //    Client.CreateDocumentQuery(collection.SelfLink, "SELECT * FROM root r WHERE r.length > 5", new FeedOptions
        //    {
        //        EnableScanInQuery = true
        //    }).AsEnumerable().Any();
        //}

        //private long ConvertDateTimeToEpoch(DateTime datetime)
        //{
        //    DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
        //    TimeSpan unixTimeSpan = datetime - unixEpoch;

        //    long epoch = (long)unixTimeSpan.TotalSeconds;
        //    return epoch;
        //}
    }
}