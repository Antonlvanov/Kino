using System;

namespace Kino.UserControl
{
    public class User
    {
        public User(string username, string passwordHash, string salt, string email, Role role, int? klient_id)
        {
            this.UserName = username;
            this.PasswordHash = passwordHash;
            this.Salt = salt;
            this.Email = email;
            this.Role = role;
            this.Klient_ID = klient_id;
        }
        public User(Role role) 
        { 
            this.Role = role;
        }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public int? Klient_ID { get; set; }

        public bool IsKlient => Klient_ID.HasValue;
    }
    public enum Role
    {
        Admin,
        Klient,
        Guest
    }
}
