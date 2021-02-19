using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Inlamningsuppgift1
{
    internal class Database
    {
        internal string ConnectionString { get; set; } = @"Data Source=.\SQLExpress;Integrated Security=true;database={0}";
        internal string DatabaseName { get; set; } //master Genealogy

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

        private void SetParameters((string, string)[] parameters, SqlCommand cmd)
        {
            foreach (var item in parameters)
            {
                cmd.Parameters.AddWithValue(item.Item1, item.Item2);
            }
        }

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

        internal void Create(Person person)
        {
            try
            {
                var connectionString = string.Format(ConnectionString, DatabaseName);
                using (var cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();
                    var sql = "INSERT INTO Family (FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, Father, Mother) VALUES(@FirstName, @LastName, @DateOfBirth, @CityOfBirth, @CityOfDeath, @Father, @Mother)";
                    using (var command = new SqlCommand(sql, cnn))
                    {
                        command.Parameters.AddWithValue("@FirstName", person.FirstName);
                        command.Parameters.AddWithValue("@LastFirstName", person.LastName);
                        command.Parameters.AddWithValue("@DateOfBirth", person.DateOfBirth);
                        command.Parameters.AddWithValue("@CityOfBirth", person.CityOfBirth);
                        command.Parameters.AddWithValue("@CityOfDeath", person.CityOfDeath);
                        command.Parameters.AddWithValue("@Father", person.FatherId);
                        command.Parameters.AddWithValue("@Mother", person.MotherId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal bool CheckIfDatabaseExist(string name)
        {
            var db = GetDataTable("SELECT name FROM sys.databases Where name = @name", ("@name", name));
            return db?.Rows.Count > 0;
        }

        internal void CreateDB(string name, bool createNewDatabase = false)
        {
            ExecuteSQL("Create database " + name);
            if (createNewDatabase) DatabaseName = name;
        }

        internal void CreateTable(string name, string fields)
        {
            ExecuteSQL($"CREATE TABLE [dbo].{name} {fields};");
        }
    }
}