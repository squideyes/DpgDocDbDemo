using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;

namespace GetMoviesJson
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                GetMovies();
            }
            catch (Exception error)
            {
                Console.WriteLine();
                Console.WriteLine("Error: " + error.Message);
            }

            Console.WriteLine();
            Console.Write("Press any key to terminate the program....");

            Console.ReadKey(true);
        }

        private static void GetMovies()
        {
            const string BASEPATH = @"..\..\Data\BulkUpload";

            if (!Directory.Exists(BASEPATH))
                Directory.CreateDirectory(BASEPATH);

            foreach (var file in Directory.GetFiles(BASEPATH))
                File.Delete(file);

            var client = new TMDbClient(Properties.Settings.Default.ApiKey);

            var settings = new JsonSerializerSettings()
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var movies = new List<MovieResult>();

            int count = 0;
            int page = 0;
            var totalPages = 1;

            while (page < totalPages)
            {
                var movieList = client.GetMovieList(
                    MovieListType.NowPlaying &
                    MovieListType.Popular &
                    MovieListType.TopRated &
                    MovieListType.Upcoming, ++page);

                foreach (var result in movieList.Results)
                {
                    var movie = client.GetMovie(result.Id,
                        MovieMethods.AlternativeTitles |
                        MovieMethods.Credits |
                        MovieMethods.Images |
                        MovieMethods.Keywords |
                        MovieMethods.Releases |
                        MovieMethods.Trailers |
                        MovieMethods.Translations);

                    var token = JObject.Parse(
                        JsonConvert.SerializeObject(movie, settings));

                    RemoveEmptyFields(token);

                    var json = token.ToString();

                    var fileName = Path.Combine(BASEPATH,
                        string.Format("Movie{0:000000}.json", movie.Id));

                    using (var writer = new StreamWriter(fileName))
                        writer.Write(json);

                    Console.WriteLine("{0:000} of {1:000} - {2}",
                        ++count, movieList.TotalResults, result.Title);
                }

                totalPages = movieList.TotalPages;
            }
        }

        private static void RemoveEmptyFields(JToken token)
        {
            var container = token as JContainer;
            
            if (container == null) 
                return;

            var removeList = new List<JToken>();

            foreach (var el in container.Children())
            {
                var p = el as JProperty;

                if (p != null && p.Value.ToString() == "")
                    removeList.Add(el);
                
                RemoveEmptyFields(el);
            }

            foreach (var el in removeList)
                el.Remove();
        }
    }
}
