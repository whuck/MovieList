using System;
using System.Collections;
using System.IO;
using NLog.Web;
using System.Text.RegularExpressions;

namespace movielist
{
    class Program 
    {
        private static string path;
        private static string fileName;
        private static ArrayList movieList;
        private static ArrayList idList;
        private static ArrayList genreList;
        private static StreamReader sr;
        private static StreamWriter sw;
            //Create movie Console Application to 
            //see all movies in the file and to 
            //add movies to the file 
            //Check for duplicate values!
            //•Implement Exception Handling
            //•Implement NLog framework
        static void Main(string[] args)
        {
            //fire up logger obj, instantiate lists, filename
            path = Directory.GetCurrentDirectory() + "\\nlog.config";
            var logger = NLog.Web.NLogBuilder.ConfigureNLog(path).GetCurrentClassLogger();
            fileName = "movies.csv";
            movieList = new ArrayList();
            idList = new ArrayList();
            genreList = new ArrayList();

            Console.WriteLine("Movies!!");

            //parse file
            ParseFile();
            
            Console.WriteLine("Enter 1 to show movie list");
            Console.WriteLine("Enter 2 to add movie");
            Console.WriteLine("Enter anything else to quit");
            string userInput = Console.ReadLine();
            if (userInput == "1") { //show list
                for (int i = 0; i < idList.Count; i++)
                {
                    Console.WriteLine($"ID:{idList[i]} Title:{movieList[i]} Genres:{genreList[i]}");
                }
            }//show list
            else if(userInput == "2") {//add to list
                Console.WriteLine("Enter movie ID:");
                string id = Console.ReadLine();
                Console.WriteLine("Enter movie title:");
                string title = Console.ReadLine();
                Console.WriteLine("Enter movie genres separated by |:");
                string genres = Console.ReadLine();
                // if there was no empty input AND no duplicate infos, add line to file
                if(!id.Equals("") && !title.Equals("") && !genres.Equals("") && !FindDupe("id",id) && !FindDupe("title",title))
                {
                    WriteLine(id,title,genres);
                }
                else {
                    Console.WriteLine("empty/duplicate value for id/title!");
                    logger.Error("empty/duplicate value for id/title!");
                }
            }//add to list
        }
        static bool FindDupe(string arg, string value)
        {
            switch(arg){
                case "id":
                    return idList.Contains(value);
                case "title":
                    return movieList.Contains(value);
                default: return true;
            }
        }
        static void ParseFile() 
        {
            Console.WriteLine("Reading file...");
            var logger = NLog.Web.NLogBuilder.ConfigureNLog(path).GetCurrentClassLogger();
            try {
                sr = new StreamReader(fileName);
            } catch {
                Console.WriteLine($"ParseFile({fileName}) failed!");
                logger.Error($"ParseFile({fileName}) failed!");
            }
            

            while(!sr.EndOfStream)
            {
                string line = "";                
                line = sr.ReadLine();
                //regex to grab title ... everything in between the first and last comma
                //as long as there is an id of digits before the first comma
                string rx = "(?<=[0-9]+,).+(?=,.+)";
                var title = Regex.Match(line,rx);
                //if the line has a title match i.e. skip file's first line
                if(!title.ToString().Equals(""))
                {
                    movieList.Add(title.ToString());
                    //regex to grab digits before first comma for movie id
                    string idrx = "[0-9]+(?=,)";
                    string id = Regex.Match(line,idrx).ToString();
                    //ugly regex to grab everything after id,title,   ie the genres
                    //there's probably a better regex to do this
                    string genreRx = "(?<=(?<=[0-9]+,).+(?=,.+),).+";
                    string genres = Regex.Match(line,genreRx).ToString();
                    idList.Add(id);
                    genreList.Add(genres);
                }
            }//while e.o.stream
            sr.Close();
            logger.Debug($"Added:{movieList.Count} titles.");
        }//parseFile()
        static void WriteLine(string id, string title, string genres)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog(path).GetCurrentClassLogger();
            try {
                sw = new StreamWriter(fileName,true);
            } catch {
                Console.WriteLine($"streamWriter: {fileName} load failed!");
                logger.Error($"streamWriter: {fileName} load failed!");
            }

            sw.WriteLine(id+","+title+","+genres);
            sw.Close();
        }
    }//class
}
