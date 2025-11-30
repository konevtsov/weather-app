using System.Data.SQLite;
using System.IO;

namespace weather_app
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;
        private readonly string _dbPath;

        public DatabaseHelper()
        {
            string appFolder = AppDomain.CurrentDomain.BaseDirectory;
            _dbPath = Path.Combine(appFolder, "weather.db");
            _connectionString = $"Data Source={_dbPath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(_dbPath))
            {
                SQLiteConnection.CreateFile(_dbPath);
            }

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Settings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    DefaultCity TEXT NOT NULL
                )";

            using var command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();
        }

        public string? GetDefaultCity()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string query = "SELECT DefaultCity FROM Settings ORDER BY Id DESC LIMIT 1";
            using var command = new SQLiteCommand(query, connection);
            var result = command.ExecuteScalar();

            return result?.ToString();
        }

        public void SaveDefaultCity(string city)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            // Use INSERT OR REPLACE with a fixed ID for atomicity
            string upsertQuery = "INSERT OR REPLACE INTO Settings (Id, DefaultCity) VALUES (1, @city)";
            using var command = new SQLiteCommand(upsertQuery, connection);
            command.Parameters.AddWithValue("@city", city);
            command.ExecuteNonQuery();
        }
    }
}
