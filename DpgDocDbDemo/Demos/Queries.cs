﻿using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DpgDocDbDemo
{
    public class Queries : DemoBase<Queries>
    {
        private const string COLLECTIONID = "Test";

        protected override async Task DoRunAsync()
        {
            await CreateDocuments();

            QueryAllDocuments();
            QueryWithEquality();
            QueryWithInequality();
            QueryWithRangeOperators();
            QueryWithSubdocuments();
            QueryWithJoins();
            await QueryWithPaging();
        }

        private void RunQuery<T>(string queryKind, int expected,
            IQueryable<T> query)
        {
            const string BADCOUNT =
                "The query returned {0} documents instead of the expected {1}!";

            Console.Write(string.Format(
                "Querying documents via {0}...", queryKind));

            var items = query.ToList();

            if (items.Count != expected)
            {
                Console.WriteLine("FAILED!");

                throw new Exception(
                    string.Format(BADCOUNT, items.Count, expected));
            }

            Console.WriteLine("SUCCESS!");
        }

        private void QueryAllDocuments()
        {
            WriteSeparator();

            Console.WriteLine("QueryAllDocuments:");

            RunQuery<Family>("LINQ Query", 2,
                from f in Client.CreateDocumentQuery<Family>(Collection.SelfLink)
                select f);

            ///////////////////////////////////////////////////////////////////

            RunQuery<Family>("LINQ Lambda", 2,
                Client.CreateDocumentQuery<Family>(Collection.SelfLink));

            ///////////////////////////////////////////////////////////////////

            RunQuery<Family>("SQL", 2, Client.CreateDocumentQuery<Family>(
                Collection.SelfLink, "SELECT * FROM Families"));
        }

        private void QueryWithEquality()
        {
            WriteSeparator();

            Console.WriteLine("QueryWithEquality (\"AndersenFamily\" Only!):");

            RunQuery<Family>("LINQ Query", 1,
                from f in Client.CreateDocumentQuery<Family>(Collection.SelfLink)
                where f.Id == "AndersenFamily"
                select f);

            RunQuery<Family>("LINQ Lambda", 1,
                Client.CreateDocumentQuery<Family>(Collection.SelfLink).
                Where(f => f.Id == "AndersenFamily"));

            RunQuery<Family>("SQL", 1,
                Client.CreateDocumentQuery<Family>(Collection.SelfLink,
                "SELECT * FROM Families f WHERE f.id='AndersenFamily'"));

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.WriteLine("QueryWithEquality (AndersenFamily AND Seattle):");

            RunQuery<Family>("LINQ Query", 1,
                from f in Client.CreateDocumentQuery<Family>(Collection.SelfLink)
                where f.Id == "AndersenFamily" && f.Address.City == "Seattle"
                select f);

            RunQuery<Family>("LINQ Lambda", 1,
                Client.CreateDocumentQuery<Family>(Collection.SelfLink).Where(f =>
                    f.Id == "AndersenFamily" && f.Address.City == "Seattle"));

            RunQuery<Family>("SQL", 1,
                Client.CreateDocumentQuery<Family>(Collection.SelfLink,
                "SELECT * FROM Families f WHERE f.id='AndersenFamily' AND f.Address.City='Seattle'"));

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.WriteLine("QueryWithEquality (LINQ Query, AndersenFamily OR NY):");

            var q1 = from f in Client.CreateDocumentQuery<Family>(Collection.SelfLink)
                     where f.Id == "AndersenFamily" || f.Address.City == "NY"
                     select new
                     {
                         Name = f.LastName,
                         City = f.Address.City
                     };

            foreach (var item in q1.ToList())
                Console.WriteLine("The {0} family lives in {1}",
                    item.Name, item.City);

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.WriteLine("QueryWithEquality (LINQ Lambda, AndersenFamily OR NY):");

            var q2 = Client.CreateDocumentQuery<Family>(Collection.SelfLink).
                Where(f => f.Id == "AndersenFamily" || f.Address.City == "NY").
                Select(f => new { Name = f.LastName, City = f.Address.City });

            foreach (var item in q2.ToList())
                Console.WriteLine("The {0} family lives in {1}",
                    item.Name, item.City);

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.WriteLine("QueryWithEquality (SQL, AndersenFamily OR NY):");

            var q3 = Client.CreateDocumentQuery(Collection.SelfLink,
                "SELECT f.LastName AS Name, f.Address.City AS City " +
                "FROM Families f " +
                "WHERE f.id='AndersenFamily' OR f.Address.City='NY'");

            foreach (var item in q3.ToList())
                Console.WriteLine("The {0} family lives in {1}",
                    item.Name, item.City);
        }

        private void QueryWithInequality()
        {
            WriteSeparator();

            Console.WriteLine("QueryWithInequality (!= \"AndersenFamily\"):");

            RunQuery<Family>("LINQ Query", 1,
                from f in Client.CreateDocumentQuery<Family>(Collection.SelfLink)
                where f.Id != "AndersenFamily"
                select f);

            RunQuery<Family>("LINQ Lambda", 1,
                Client.CreateDocumentQuery<Family>(Collection.SelfLink).
                Where(f => f.Id != "AndersenFamily"));

            RunQuery<Family>("SQL", 1,
                Client.CreateDocumentQuery<Family>(Collection.SelfLink,
                "SELECT * FROM Families f WHERE f.id='AndersenFamily'"));

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.WriteLine("QueryWithEquality (LINQ Query, Wakefield != NY):");

            RunQuery<Family>("LINQ Query", 0,
                from f in Client.CreateDocumentQuery<Family>(Collection.SelfLink)
                where f.Id == "Wakefield" && f.Address.City != "NY"
                select f);

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.WriteLine("QueryWithEquality (SQL, AndersenFamily != NY):");

            RunQuery<Family>("SQL", 1,
                Client.CreateDocumentQuery<Family>(Collection.SelfLink,
                "SELECT * FROM Families f WHERE f.id = 'AndersenFamily' " +
                "AND f.Address.City != 'NY'"));
        }

        private void QueryWithRangeOperators()
        {
            // EnableScanInQuery enables range queries on Collections that only
            // have hash indexes (the default).  Consider adding a Range index 
            // on paths where you will often perform Range queries

            WriteSeparator();

            Console.WriteLine("QueryWithRangeOperators (Children.Grade > 5)");

            RunQuery<Family>("LINQ Query", 1,
                from f in Client.CreateDocumentQuery<Family>(Collection.SelfLink,
                               new FeedOptions { EnableScanInQuery = true })
                where f.Children[0].Grade > 5
                select f);

            RunQuery<Family>("LINQ Lambda", 1,
                Client.CreateDocumentQuery<Family>(Collection.SelfLink,
                new FeedOptions { EnableScanInQuery = true })
                       .Where(f => f.Children[0].Grade > 5));

            RunQuery<Family>("SQL", 1,
                Client.CreateDocumentQuery<Family>(Collection.SelfLink,
                "SELECT * FROM Families f WHERE f.Children[0].Grade = 5",
                new FeedOptions { EnableScanInQuery = true }));
        }

        private void QueryWithSubdocuments()
        {
            WriteSeparator();

            Console.WriteLine("QueryWithSubdocuments (SQL)");

            var children = Client.CreateDocumentQuery<Child>(Collection.SelfLink,
                "SELECT c FROM c IN f.Children").ToList();

            foreach (var child in children)
                Console.WriteLine(child);

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.WriteLine("QueryWithSubdocuments (LINQ Query)");

            var cc = Client.CreateDocumentQuery<Family>(Collection.SelfLink).
                SelectMany(family => family.Children.Select(c => c));

            foreach (var item in cc.ToList())
                Console.WriteLine(item);
        }

        private void QueryWithJoins()
        {
            WriteSeparator();

            Console.WriteLine("QueryWithJoins (Family => Children; LINQ Query):");

            var aa = Client.CreateDocumentQuery(Collection.SelfLink,
                "SELECT f.id FROM Families f JOIN c IN f.Children");

            foreach (var item in aa.ToList())
                Console.WriteLine(item);

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.WriteLine("QueryWithJoins (Family => Children; LINQ Lambda):");

            var bb = Client.CreateDocumentQuery<Family>(Collection.SelfLink).
                SelectMany(family => family.Children.Select(c => family.Id));

            foreach (var item in bb.ToList())
                Console.WriteLine(item);

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.WriteLine("QueryWithJoins (Family => Children => Pets; SQL):");

            var cc = Client.CreateDocumentQuery<dynamic>(Collection.SelfLink,
                "SELECT f.id, c.FirstName AS child, p.GivenName AS pet " +
                "FROM Families f JOIN c IN f.Children JOIN p IN c.Pets ");

            foreach (var item in cc.ToList())
                Console.WriteLine(item);

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.WriteLine("QueryWithJoins (Family => Children => Pets; Projected; LINQ Lambda):");

            var dd = Client.CreateDocumentQuery<Family>(Collection.SelfLink).
                SelectMany(family => family.Children.SelectMany(children =>
                    children.Pets.Select(pets => new
                    {
                        family = family.Id,
                        child = children.FirstName,
                        pet = pets.GivenName
                    })));

            foreach (var item in dd.ToList())
                Console.WriteLine(item);

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.WriteLine("QueryWithJoins (Family => Children => Pets; Filtered; LINQ Query):");

            var ee = Client.CreateDocumentQuery<dynamic>(Collection.SelfLink,
                    "SELECT f.id, c.FirstName AS child, p.GivenName AS pet " +
                    "FROM Families f JOIN c IN f.Children JOIN p IN c.Pets " +
                    "WHERE p.GivenName = 'Fluffy'");

            foreach (var item in ee.ToList())
                Console.WriteLine(item);

            ///////////////////////////////////////////////////////////////////

            WriteSeparator();

            Console.WriteLine(
                "QueryWithJoins (Family => Children => Pets; Projected & Filtered; LINQ Query):");

            var ff = Client.CreateDocumentQuery<Family>(Collection.SelfLink).SelectMany(
                family => family.Children.SelectMany(children => children.Pets.Where(
                    pets => pets.GivenName == "Fluffy").Select(pets => new
                    {
                        family = family.Id,
                        child = children.FirstName,
                        pet = pets.GivenName
                    })));

            foreach (var item in ff.ToList())
                Console.WriteLine(item);
        }

        private async Task QueryWithPaging()
        {
            WriteSeparator();

            Console.Write("QueryWithPaging...");

            // The .NET Client automatically iterates through all the pages of 
            // query results. Developers can explicitly control paging by 
            // creating an IDocumentQueryable using the IQueryable object, then 
            // by reading the ResponseContinuationToken values and passing them 
            //back as RequestContinuationToken in FeedOptions.

            var families = new List<Family>();

            var options = new FeedOptions { MaxItemCount = 1 };

            // using AsDocumentQuery you get access to whether or not the query 
            // HasMoreResults If it does, just call ExecuteNextAsync until there 
            // are no more results.  No need to supply a continuation token here 
            // as the server keeps track of progress

            var query = Client.CreateDocumentQuery<Family>(
                Collection.SelfLink, options).AsDocumentQuery();

            while (query.HasMoreResults)
            {
                foreach (Family family in await query.ExecuteNextAsync())
                    families.Add(family);
            }

            // The above sample works fine whilst in a loop as above, but 
            // what if you load a page of 1 record and then in a different 
            // session at a later stage want to continue from where you were?
            // well, now you need to capture the continuation token 
            // and use it on subsequent queries
            query = Client.CreateDocumentQuery<Family>(Collection.SelfLink, 
                new FeedOptions { MaxItemCount = 1 }).AsDocumentQuery();

            var feedResponse = await query.ExecuteNextAsync<Family>();

            var continuation = feedResponse.ResponseContinuation;
            
            foreach (var f in feedResponse.AsEnumerable().OrderBy(f => f.Id))
            {
                if (f.Id != "AndersenFamily") 
                    throw new ApplicationException("Should only be the first family");
            }

            // Now the second time around use the contiuation token you got
            // and start the process from that point
            query = Client.CreateDocumentQuery<Family>(Collection.SelfLink, new FeedOptions { 
                MaxItemCount = 1, RequestContinuation = continuation }).AsDocumentQuery();

            feedResponse = await query.ExecuteNextAsync<Family>();
            
            foreach (var f in feedResponse.AsEnumerable().OrderBy(f => f.Id))
            {
                if (f.Id != "WakefieldFamily") 
                    throw new ApplicationException("Should only be the second family");
            }

            Console.WriteLine("SUCCESS!");
        }

        private async Task CreateDocuments()
        {
            Console.Write("Creating the AndersenFamily document...");

            var AndersonFamily = new Family
            {
                Id = "AndersenFamily",
                LastName = "Andersen",
                Parents = new Parent[] {
                    new Parent { FirstName = "Thomas" },
                    new Parent { FirstName = "Mary Kay"}
                },
                Children = new Child[] {
                    new Child
                    { 
                        FirstName = "Henriette Thaulow", 
                        Gender = "female", 
                        Grade = 5, 
                        Pets = new [] {
                            new Pet { GivenName = "Fluffy" } 
                        }
                    } 
                },
                Address = new Address { State = "WA", County = "King", City = "Seattle" },
                IsRegistered = true
            };

            Console.WriteLine("CREATED!");

            Console.Write("Creating the WakefieldFamily document...");

            await Client.CreateDocumentAsync(Collection.SelfLink, AndersonFamily);

            var WakefieldFamily = new Family
            {
                Id = "WakefieldFamily",
                LastName = "Wakefield",
                Parents = new[] {
                    new Parent { FamilyName= "Wakefield", FirstName= "Robin" },
                    new Parent { FamilyName= "Miller", FirstName= "Ben" }
                },
                Children = new Child[] {
                    new Child
                    {
                        FamilyName= "Merriam", 
                        FirstName= "Jesse", 
                        Gender= "female", 
                        Grade= 8,
                        Pets= new Pet[] {
                            new Pet { GivenName= "Goofy" },
                            new Pet { GivenName= "Shadow" }
                        }
                    },
                    new Child
                    {
                        FamilyName= "Miller", 
                        FirstName= "Lisa", 
                        Gender= "female", 
                        Grade= 1
                    }
                },
                Address = new Address { State = "NY", County = "Manhattan", City = "NY" },
                IsRegistered = false
            };

            await Client.CreateDocumentAsync(Collection.SelfLink, WakefieldFamily);

            Console.WriteLine("CREATED!");
        }
    }
}