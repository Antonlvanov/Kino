using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Kino.Database;

namespace Kino.UserControl
{
    public class UserManager
    {
        private List<User> Users = new List<User>();
        private dbHelper dbHelper = new dbHelper();
        public User CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;

        public UserManager()
        {
            ReadUsersFromDatabase();
            CurrentUser = new User(Role.Guest);
        }

        public bool AddUser(string userName, string password, string email, Role role, string klient_id = null)
        {
            Console.WriteLine("Попытка добавить пользователя: " + userName);

            if (UserExists(userName, email))
            {
                Console.WriteLine("Пользователь с таким именем или email уже существует.");
                return false;
            }

            if (role != Role.Klient)
            {
                klient_id = null;
            }

            var (hashedPassword, salt) = HashPassword(password);
            int? klientId = string.IsNullOrEmpty(klient_id) ? (int?)null : Convert.ToInt32(klient_id);

            User newUser = new User(userName, hashedPassword, salt, email, role, klientId);
            Console.WriteLine($"Добавлен новый пользователь: {newUser.UserName} с ролью: {newUser.Role}");

            Users.Add(newUser);
            AddUserToDatabase(newUser);

            return true;
        }

        public bool LoginUser(string username, string password)
        {
            User user = this.Users.Find(u => u.UserName == username);
            if (user != null && VerifyPassword(password, user.PasswordHash, user.Salt))
            {
                CurrentUser = user;
                Console.WriteLine($"Пользователь вошел: {CurrentUser.UserName}, роль: {CurrentUser.Role}");
                return true;
            }
            return false;
        }
        public void Logout()
        {
            CurrentUser = new User(Role.Guest);
        }

        private (string hash, string salt) HashPassword(string password)
        {
            Console.WriteLine("Генерация соли и хеширование пароля.");
            // generating salt
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            string salt = Convert.ToBase64String(saltBytes);

            // combining password and salt
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes;
            using (var sha256 = SHA256.Create())
            {
                hashBytes = sha256.ComputeHash(passwordBytes);
            }
            string hash = Convert.ToBase64String(hashBytes);

            return (hash, salt);
        }

        private bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            Console.WriteLine("Проверка пароля.");
            byte[] passwordBytes = Encoding.UTF8.GetBytes(enteredPassword + storedSalt);
            byte[] hashBytes;
            using (var sha256 = SHA256.Create())
            {
                hashBytes = sha256.ComputeHash(passwordBytes);
            }
            string computedHash = Convert.ToBase64String(hashBytes);

            return storedHash == computedHash;
        }

        public void ReadUsersFromDatabase()
        {
            try
            {
                Console.WriteLine("Чтение пользователей из базы данных.");
                DataTable userTable = dbHelper.ExecuteQuery(Queries.GetAllUsers());

                Users.Clear();
                foreach (DataRow row in userTable.Rows)
                {
                    string userName = row["userName"].ToString();
                    string passwordHash = row["passwordHash"].ToString();
                    string salt = row["salt"].ToString();
                    string email = row["email"].ToString();
                    Role role = Enum.TryParse(row["role"].ToString(), out Role parsedRole) ? parsedRole : Role.Guest;
                    int? klient_id = row["klient_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["klient_id"]);
                    Console.WriteLine($"Загружен пользователь: {userName}, роль: {role}, соль: {salt}");

                    Users.Add(new User(userName, passwordHash, salt, email, role, klient_id));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении пользователей из базы данных: {ex.Message}");
                throw;
            }
        }

        public void AddUserToDatabase(User user)
        {
            try
            {
                Console.WriteLine($"Добавление пользователя в базу данных: {user.UserName}");
                Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@userName", user.UserName },
                { "@passwordHash", user.PasswordHash },
                { "@salt", user.Salt },
                { "@email", user.Email },
                { "@role", user.Role.ToString() },
                { "@klient_id", user.Klient_ID.HasValue ? (object)user.Klient_ID.Value : DBNull.Value }
            };

                dbHelper.ExecuteNonQuery(Queries.InsertUser(), parameters);
                Console.WriteLine("Пользователь успешно добавлен в базу данных.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении пользователя в базу данных: {ex.Message}");
                throw;
            }
        }

        public bool UserExists(string userName, string email)
        {
            return Users.Any(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) ||
                                  u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsUserKlient(string userName)
        {
            User user = Users.FirstOrDefault(u => u.UserName == userName);
            return user != null && user.IsKlient;
        }

    }

}
