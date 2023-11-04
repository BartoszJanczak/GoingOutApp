using GoingOutApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace GoingOutApp.Services
{
    public class UserService
    {
        public static User LoggedInUser { get; private set; }

        public static bool ValidateSignIn(string username, string password)
        {
            using (DataContext context = new DataContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    byte[] salt = Convert.FromBase64String(user.Password);
                    byte[] hash = Convert.FromBase64String(user.Key);

                    using (var deriveBytes = new Rfc2898DeriveBytes(password, salt))
                    {
                        byte[] enteredPasswordHash = deriveBytes.GetBytes(hash.Length);

                        bool passwordIsValid = hash.SequenceEqual(enteredPasswordHash);

                        if (passwordIsValid) 
                        {
                            LoggedInUser = user;
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public static void Logout()
        {
            LoggedInUser = null;
        }
    }
}
