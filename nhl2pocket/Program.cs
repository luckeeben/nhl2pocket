using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using PocketSharp;
using PocketSharp.Models;

namespace nhl2pocket
{
    class Program
    {
        static void Main()
        {
            GetGameIds();
            
        }

        static async Task PocketAdd(string url)
        {
            const string consumerKey = "[YOUR_CONSUMER_KEY]";
            const string accessCode = "[YOUR_ACCESS_CODE]";

            PocketClient client = new PocketClient(consumerKey, accessCode);

            PocketItem newItem = await client.Add(
                new Uri(url),
                new string[] { "boxscore" }
            );

        }

        static void GetGameIds()
        {
            var web = new HtmlWeb();
            var document = web.Load("http://www.nhl.com/ice/gamestats.htm?season=20142015&gameType=2&team=&viewName=summary&pg=1");
            var trlist = document.DocumentNode.QuerySelectorAll("table.data.stats>tbody>tr");
            //var gameids = new string[];

            foreach (var tr in trlist)
            {
                DateTime thisDay = DateTime.Today.AddDays(-1);
                var currentMonth = thisDay.ToString("MMM");
                var currentDay = thisDay.ToString("dd");
                var currentYear = thisDay.ToString("yy");

                var link = tr.QuerySelector("td>a");
                var datetext = link.InnerText;
                string[] datesplit = datetext.Split(' ');

                //if (datesplit[0].Equals(currentMonth) && datesplit[1].Equals(currentDay) && datesplit[2].Equals(currentYear))
                if (datesplit[1].Equals(currentDay))
                {
                    var linkText = link.GetAttributeValue("href", "ERROR");
                    string[] lines = Regex.Split(linkText, "http://www.nhl.com/scores/htmlreports/20142015/GS0");
                    var gameid = lines[1].TrimEnd('.', 'H', 'T', 'M');

                    var gameUrl = "http://www.nhl.com/scores/htmlreports/20142015/ES0" + gameid + ".HTM";
                    //Console.WriteLine(gameUrl);
                    PocketAdd(gameUrl).Wait();

                }

                //Console.WriteLine(datetext);

            }

            //Console.ReadLine();
        }
    }
}
