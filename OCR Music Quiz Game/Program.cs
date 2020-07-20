using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace OCR_Music_Quiz_Game
{
    class Program
    {
        public static string Files = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Files";
        public static string Leaderboard = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Files\\leaderboard.json";
        public static string Songnames = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Files\\songnames.json";
        public static string Accounts = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Files\\accounts.json";
        static void Main(string[] args)
        {
            if (!Directory.Exists(Files)) 
            {
                Directory.CreateDirectory(Files);
                File.Create(Leaderboard).Dispose();
                File.WriteAllText(Leaderboard, "{ }");
                File.Create(Songnames).Dispose();
                File.WriteAllText(Songnames, "{ \"songinfo\":[ {\"artist\":\"Luis Fonsi\",\"songname\":\"Despacito\"}, {\"artist\":\"Ed Sheeran\",\"songname\":\"Shape of You\"}, {\"artist\":\"Wiz Khalifa ft. Charlie Puth\",\"songname\":\"See You Again\"}, {\"artist\":\"Mark Ronson ft. Bruno Mars\",\"songname\":\"Uptown Funk\"}, {\"artist\":\"Psy\",\"songname\":\"Gangam Style\"} ] }");
                File.Create(Accounts).Dispose();
                File.WriteAllText(Accounts, "{ }");
            }
            
            Console.WriteLine("Welcome to the Music Quiz Game!");

        loginPrompt:
            Console.WriteLine("To login to an existing account, use the command 'login'. To register, use the command 'register'.");
            Console.Write(">");

            string userInput = Console.ReadLine();

            if (userInput.ToLower() == "login") 
            {
                Console.WriteLine("Please enter your username");
                Console.Write(">");
                string username = Console.ReadLine();

                Console.WriteLine("Please enter your password");
                Console.Write(">");
                string password = Console.ReadLine();

                Authentication auth = new Authentication(username, password); //use the Authentication class to log in the user
                if (auth.LoginUser()) 
                {
                    Console.WriteLine("Logged in!");
                    System.Threading.Thread.Sleep(1500);
                    Console.Clear();
                    Menu(username);
                }
                else 
                {
                    Console.WriteLine("Wrong username or password!");
                    goto loginPrompt;
                }
            }
            else if (userInput.ToLower() == "register") 
            {
                Console.WriteLine("Please enter your desired username");
                Console.Write(">");
                string username = Console.ReadLine();

                Console.WriteLine("Please enter a password");
                Console.Write(">");
                string password = Console.ReadLine();

                Authentication auth = new Authentication(username, password);  //use the Authentication class to register in the user
                if (auth.RegisterUser()) 
                {
                    Console.WriteLine("Registered sucessfully!");
                    System.Threading.Thread.Sleep(1500);
                    Console.Clear();
                    Menu(username);
                }
                else
                {
                    Console.WriteLine("Account username exists already, please use a different username!");
                    goto loginPrompt;
                }
            } 
            else 
            {
                Console.WriteLine("Invalid input!");
                goto loginPrompt;
            }
        }

        static public void Menu(string username) 
        {
            menuPrompt:
            Console.WriteLine("Use the command 'play' to start the game, 'leaderboard' to show the leaderboard.");
            Console.Write(">");
            string userInput = Console.ReadLine();
            if (userInput.ToLower() == "play") 
            {
                Game game = new Game(username);
                game.Play();
            } 
            else if (userInput.ToLower() == "leaderboard") 
            {
                LeaderboardJSON leaderboardJson = JsonConvert.DeserializeObject<LeaderboardJSON>(File.ReadAllText(Leaderboard));
                List<UserJSON> leaderboard = leaderboardJson.users.OrderBy(user => user.points).Reverse().ToList(); //sort by highest to lowest point amount
               
                for (int i = 0; i < 4; i++) 
                {
                    try
                    {
                        Console.WriteLine($"{i + 1}. Name: {leaderboard[i].username} Points: {leaderboard[i].points}");
                    } catch { break; }
                }
                goto menuPrompt;
            }
            else 
            {
                Console.WriteLine("Invalid input!");
                goto menuPrompt;
            }
        }
    }
}
