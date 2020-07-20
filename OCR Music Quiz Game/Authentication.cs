using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace OCR_Music_Quiz_Game
{
    public class Authentication
    {
        private string _username;
        private string _password;

        public Authentication(string username, string password) 
        {
            _username = username;

            using (var crypt = new SHA256Managed()) //hash the password using sha256
            {
                var hash = new StringBuilder();
                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
                foreach (byte theByte in crypto)
                {
                    hash.Append(theByte.ToString("x2"));
                }
                _password = hash.ToString();
            }
            
        }

        public bool LoginUser() 
        {
            AccountsJSON accountsJSON = JsonConvert.DeserializeObject<AccountsJSON>(File.ReadAllText(Program.Accounts));
           
            foreach (AccountJSON account in accountsJSON.accounts) 
            {
                if (account.username == _username && account.password == _password) //if the username and password are both correct, log in
                {
                    return true;
                } 
            }

            return false;
        }

        public bool RegisterUser()
        {
            AccountsJSON accountsJSON = JsonConvert.DeserializeObject<AccountsJSON>(File.ReadAllText(Program.Accounts));

            foreach (AccountJSON account in accountsJSON.accounts) //Check if the username exists already
            {
                if (account.username == _username)
                {
                    return false;
                }
            }

            AccountJSON newAccount = new AccountJSON
            {
                username = _username,
                password = _password
            };

            accountsJSON.accounts.Add(newAccount);

            string _json = JsonConvert.SerializeObject(accountsJSON); 
            File.WriteAllText(Program.Accounts, _json); 

            return true;
        }
    }

    class AccountsJSON 
    {
        [JsonProperty("accounts")]
        public List<AccountJSON> accounts = new List<AccountJSON>();
    }

    class AccountJSON
    {
        [JsonProperty("username")]
        public string username { get; set; }

        [JsonProperty("password")]
        public string password { get; set; }
    }
}

//System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location + \Files