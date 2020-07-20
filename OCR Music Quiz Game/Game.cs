using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace OCR_Music_Quiz_Game
{
    public class Game
    {
        private string _username; //This will be used to save the points on the leaderboard
        private int points;
        public Game(string username)
        {
            _username = username;
        }
        public void Play()
        {
            SongnamesJSON songNames = JsonConvert.DeserializeObject<SongnamesJSON>(File.ReadAllText(Program.Songnames));
            List<SonginfoJSON> songs = songNames.songinfo;
            Shuffle(songs);

            foreach (SonginfoJSON song in songs)
            {
                Console.Write($"{song.artist} - ");

                string[] words = song.songname.Split(' ');
                foreach (string word in words)
                {
                    Console.Write(word[0] + " ");
                }
                Console.Write("\n");

                int lives = 2;
            guess:
                if (lives > 0)
                {
                    Console.WriteLine($"You have {lives} more tries to guess the song name!");
                    Console.Write(">");
                    string userGuess = Console.ReadLine();

                    if (userGuess.ToLower() == song.songname.ToLower())
                    {
                        Console.WriteLine("Correct!");

                        if (lives == 2) 
                        {
                            points += 3;
                            Console.WriteLine($"You got 3 points! You have {points} points in total!");
                        }
                        else 
                        {
                            points += 1;
                            Console.WriteLine($"You got 1 point! You have {points} points in total!");
                        }
                        System.Threading.Thread.Sleep(2000);
                        Console.Clear();
                    }
                    else
                    {
                        Console.WriteLine("Inorrect! :(");
                        lives--;
                        goto guess;
                    }
                }
                else 
                {
                    break;
                }
            }
            Console.WriteLine($"Game over! You achieved {points} points!");

            LeaderboardJSON leaderboard = JsonConvert.DeserializeObject<LeaderboardJSON>(File.ReadAllText(Program.Leaderboard));

            bool foundUser = false;
            foreach (UserJSON user in leaderboard.users)
            {
                if (user.username == _username)
                {
                    if (user.points < points)
                    {
                        user.points = points;
                    }
                    foundUser = true;
                    break;
                }
            }

            if (!foundUser)
            {
                UserJSON newUser = new UserJSON
                {
                    username = _username,
                    points = points
                };
                leaderboard.users.Add(newUser);
            }
            string _json = JsonConvert.SerializeObject(leaderboard);
            File.WriteAllText(Program.Leaderboard, _json);

            System.Threading.Thread.Sleep(2000);
            Console.Clear();
            Program.Menu(_username);
        }

        private static Random rng = new Random();

        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
    

    class SongnamesJSON
    {
        [JsonProperty("songinfo")]
        public List<SonginfoJSON> songinfo = new List<SonginfoJSON>();
    }

    class SonginfoJSON
    {
        [JsonProperty("artist")]
        public string artist { get; set; }

        [JsonProperty("songname")]
        public string songname { get; set; }
    }

    class LeaderboardJSON
    {
        [JsonProperty("users")]
        public List<UserJSON> users = new List<UserJSON>();
    }

    class UserJSON
    {
        [JsonProperty("username")]
        public string username { get; set; }

        [JsonProperty("points")]
        public int points { get; set; }
    }
}
