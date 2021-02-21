using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Inlamningsuppgift1
{
    internal class Database
    {
        internal string ConnectionString { get; } = @"Data Source=.\SQLExpress;Integrated Security=true;database={0}";
        internal string DatabaseName { get; set; } = "Genealogy";
        
        /// <summary>
        /// Lägger till en tabell i databasen och lägger in data i tabellen.
        /// </summary>
        public void AddDatabaseData()
        {
            CreateTable("[Family]", @"(
                            [Id] [int] IDENTITY (1,1) NOT NULL,
                            [FirstName] [nvarchar](255) NULL,
                            [LastName] [nvarchar](255) NULL,
                            [DateOfBirth] [int] DEFAULT 0,
                            [Birthplace] [nvarchar](255) NULL,
                            [DateOfDeath] [int] DEFAULT 0,
                            [Deathplace] [nvarchar](255) NULL,
                            [FatherId] [int] DEFAULT 0,
                            [MotherId] [int] DEFAULT 0
                            ) ON [PRIMARY]");

            ExecuteSQL(@"
            INSERT INTO Family(FirstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES('Ivan', 'Winroth', 1920, 'Öttum, Sverige', 1987, 'Kvänum, Sverige', null, null);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES('Stina', 'Winroth', 1925, 'Öttum, Sverige', 1993, 'Kvänum, Sverige', null, null);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES('Bo', 'Linde', 1932, 'Vemdalen, Sverige', 2009, 'Skara, Sverige', null, null);
            INSERT INTO Family(firstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES('Inger', 'Linde', 1936, 'Karlstad, Sverige', 1994, 'Skara, Sverige', null, null);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES('Sture', 'Winroth', 1957, 'Öttum, Sverige', null, null, 1, 2);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES('Ingela', 'Winroth', 1961, 'Karlstad, Sverige', null, null, 3, 4);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES('Fredrik', 'Winroth', 1984, 'Öttum, Sverige', null, null, 5, 6);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES('Charlotte', 'Fransson', 1986, 'Öttum, Sverige',null, null, 5, 6);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES('Sarah', 'Winroth', 1989, 'Öttum, Sverige', null, null, 5, 6);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES('Rebecca', 'Winroth', 1997, 'Öttum, Sverige', null, null, 5, 6);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES('Magnus', 'Fransson', 1985, 'Tråvad, Sverige', null, null, null, null);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES('Wille', 'Fransson', 2013, 'Tråvad, Sverige', null, null, 11, 8);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES('Saga', 'Fransson', 2015, 'Tråvad, Sverige', null, null, 11, 8);
            ");
        }
        /// <summary>
        /// Kollar om det redan finns en databas genom det namnet som skickas in som parameter. 
        /// Informationen som hämtas läggs in i en tabell, om det är fler rader än 0 i tabellen så returneras true.
        /// </summary>
        /// <returns>Bool, Sant eller falskt</returns>
        internal bool CheckIfDatabaseExist(string name)
        {
            var db = GetDataTable("SELECT name FROM sys.databases WHERE name = @name", ("@name", name));
            return db?.Rows.Count > 0;
        }
        /// <summary>
        /// Skapar en databas med namnet som skickas in som parameter
        /// </summary>
        internal void CreateDB(string name)
        {
            ExecuteSQL("CREATE DATABASE " + name);
        }
        /// <summary>
        /// Skapar en tabell och fyller den med information. Namnet på tabellen och informationen skickas in som parametrar.
        /// </summary>
        internal void CreateTable(string name, string fields)
        {
            ExecuteSQL($"CREATE TABLE [dbo].{name} {fields};");
        }
        /// <summary>
        /// Metoden öppnar upp en koppling till databasen med hjälp utav SqlConnection och tar emot sqlkommando/text och värdena(parametrar) som parametrar. 
        /// Skapar ett objekt av klassen SqlCommand som tar emot sqlkommando-text för att kunna skicka kommandon till databasen via kopplingen. 
        /// </summary>
        /// <returns>Long, antalet rader som harpåverkats</returns>
        internal long ExecuteSQL(string sqlString, params (string, string)[] parameters)
        {
            long rowsAffected = 0;
            try
            {
                var connectionString = string.Format(ConnectionString, DatabaseName);
                using (var cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();
                    using (var cmd = new SqlCommand(sqlString, cnn))
                    {
                        SetParameters(parameters, cmd);
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return rowsAffected;
        }
        /// <summary>
        /// Metoden öppnar upp en koppling till databasen med hjälp utav SqlConnection och tar emot sqlkommando/text och värdena(parametrar) som parametrar. 
        /// Skapar ett objekt av klassen SqlCommand som tar emot sqlkommando-text för att kunna skicka kommandon till databasen via kopplingen. Svaret från databasen fylls in i en dataTable
        /// </summary>
        /// <returns>En DataTable</returns>
        internal DataTable GetDataTable(string sqlString, params (string, string)[] parameters)
        {
            var dataTable = new DataTable();
            try
            {
                var connectionString = string.Format(ConnectionString, DatabaseName);
                using (var cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();
                    using (var cmd = new SqlCommand(sqlString, cnn))
                    {
                        SetParameters(parameters, cmd);

                        try
                        {
                            using (var adapter = new SqlDataAdapter(cmd))
                            {
                                adapter.Fill(dataTable);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return dataTable;
        }
        /// <summary>
        /// Metoden lägger in värden (parametrar) i Sqlkommandot/strängen.
        /// </summary>
        private void SetParameters((string, string)[] parameters, SqlCommand cmd)
        {
            foreach (var item in parameters)
            {
                cmd.Parameters.AddWithValue(item.Item1, item.Item2);
            }
        }
    }
}