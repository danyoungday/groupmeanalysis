using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace groupmeanalysis
{

    class Analyzer
    {
        public Dictionary<String, String> nameMap;
        public Dictionary<String, int> likeMap;
        public Dictionary<String, int> messageMap;

        String mostLiked;
        int mostLikes;

        JArray messages;

        public Analyzer(String directory)
        {
            //Initialize local variables for later
            likeMap = new Dictionary<String, int>();
            messageMap = new Dictionary<String, int>();
            nameMap = new Dictionary<String, String>();

            //Import messages array
            String messagesString = File.ReadAllText(directory + "/message.json");
            messages = (JArray)JsonConvert.DeserializeObject(messagesString);

            readLikesAndMessages();

        }

        private void readLikesAndMessages()
        {
            mostLikes = 0;
            mostLiked = "";
            foreach(JObject message in messages)
            {
                String id = message["sender_id"].ToString();
                if(!nameMap.ContainsKey(id))
                {
                    nameMap.Add(id, message["name"].ToString());
                }
                int numLikes = ((JArray)message["favorited_by"]).Count;
                if(!likeMap.ContainsKey(id))
                {
                    likeMap.Add(id, numLikes);
                    messageMap.Add(id, 1);
                }
                else
                {
                    likeMap[id] += numLikes;
                    messageMap[id]++;
                }
                
                //See if it's the highest liked message
                if(numLikes > mostLikes)
                {
                    mostLiked = id;
                    mostLikes = numLikes;
                }

            }
        }


        public void printMostLiked()
        {
            Console.WriteLine("Most liked message sent by: " + nameMap[mostLiked] + " with " + mostLikes + " likes");
        }

        public void PrintLikes()
        {
            var sorted = likeMap.ToList();

            sorted.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            int place = 1;

            foreach(KeyValuePair<String, int> p in sorted)
            {
                Console.WriteLine(place.ToString().PadLeft(2) + ": " + nameMap[p.Key] + ": " + p.Value);
                place++;
            }
        }

        public void PrintMessages()
        {
            var sorted = messageMap.ToList();

            sorted.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            int place = 1;

            foreach (KeyValuePair<String, int> p in sorted)
            {
                Console.WriteLine(place.ToString().PadLeft(2) + ": " + nameMap[p.Key] + ": " + p.Value);
                place++;
            }
        }

        public void PrintLikesPerMessage()
        {
            Dictionary<String, double> lpmMap = new Dictionary<String, double>();
            foreach(String id in likeMap.Keys)
            {
                lpmMap.Add(id, ((double)likeMap[id] / (double)messageMap[id]));
            }

            var sorted = lpmMap.ToList();

            sorted.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));


            int place = 1;

            foreach(KeyValuePair<String, double> p in sorted)
            {
                Console.WriteLine(place.ToString().PadLeft(2) + ": " + nameMap[p.Key] + ": " + p.Value);
                place++;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Analyzer a = new Analyzer("C:\\Users\\Daniel\\Documents\\groupmedata\\55250366");
            a.PrintLikesPerMessage();
            a.printMostLiked();
        }
    }
}
