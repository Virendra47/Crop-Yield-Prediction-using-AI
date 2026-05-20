using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Linq;
using CropYieldPredictor.Models;

namespace CropYieldPredictor.Services
{
    public class UserService
    {
        private readonly string _filePath;
        private static readonly object _lock = new object();

        public UserService()
        {
            // Place it in the app directory
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
        }

        private List<User> LoadUsers()
        {
            lock (_lock)
            {
                if (!File.Exists(_filePath))
                {
                    return new List<User>();
                }

                try
                {
                    var json = File.ReadAllText(_filePath);
                    return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                }
                catch
                {
                    return new List<User>();
                }
            }
        }

        private void SaveUsers(List<User> users)
        {
            lock (_lock)
            {
                try
                {
                    var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(_filePath, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving users database: {ex.Message}");
                }
            }
        }

        public bool Register(string username, string email, string password)
        {
            var users = LoadUsers();

            // Check if username already exists
            if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                return false; // Username taken
            }

            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password)
            };

            users.Add(newUser);
            SaveUsers(users);
            return true;
        }

        public User? ValidateUser(string username, string password)
        {
            var users = LoadUsers();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return null;
            }

            var hash = HashPassword(password);
            if (user.PasswordHash == hash)
            {
                return user;
            }

            return null;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
