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

            // Clear previous entries and insert new one
            string deleteQuery = "DELETE FROM Settings";
            using (var deleteCommand = new SQLiteCommand(deleteQuery, connection))
            {
                deleteCommand.ExecuteNonQuery();
            }

            string insertQuery = "INSERT INTO Settings (DefaultCity) VALUES (@city)";
            using var insertCommand = new SQLiteCommand(insertQuery, connection);
            insertCommand.Parameters.AddWithValue("@city", city);
            insertCommand.ExecuteNonQuery();
        }
    }
}
