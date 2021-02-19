using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Inlamningsuppgift1
{
    public class Crud
    {
        internal static Database db = new Database();

        public static T GetValue<T>(object value)
        {
            if (value == null || value == DBNull.Value)
                return default;
            else
                return (T)value;
        }

        public bool CheckIfPersonExist(string firstName, string lastName, int fatherId, int motherId)
        {
            var row = db.GetDataTable("USE Genealogy SELECT TOP 1 * from Family Where FirstName LIKE @FirstName AND LastName LIKE @LastName AND FatherId = @FatherId AND MotherId = @MotherId", ("@FirstName", firstName.ToString()), ("@LastName", lastName.ToString()), ("@FatherId", fatherId.ToString()), ("@MotherId", motherId.ToString()));
            if (row.Rows.Count > 0)
            { return true; }
            else
            { return false; }
        }

        public void Create(Person person)
        {
            try
            {
                var connectionString = string.Format(db.ConnectionString, db.DatabaseName);
                using (var cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();
                    var sql = $"USE Genealogy INSERT INTO dbo.Family(FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES(@FirstName, @LastName, @DateOfBirth, @CityOfBirth, @CityOfDeath, @FatherId, @MotherId)";
                    using (var cmd = new SqlCommand(sql, cnn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", person.FirstName ?? null);
                        cmd.Parameters.AddWithValue("@LastName", person.LastName ?? null);
                        cmd.Parameters.AddWithValue("@DateOfBirth", person.DateOfBirth);
                        cmd.Parameters.AddWithValue("@CityOfBirth", person.CityOfBirth ?? null);
                        cmd.Parameters.AddWithValue("@CityOfDeath", person.CityOfDeath ?? null);
                        cmd.Parameters.AddWithValue("@FatherId", person.FatherId);
                        cmd.Parameters.AddWithValue("@MotherId", person.MotherId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Delete(Person person)
        {
            db.ExecuteSQL("USE Genealogy DELETE FROM Family Where Id=@Id", ("@Id", person.Id.ToString()));
        }

        public string GetParentId(string name)
        {
            DataTable dt;
            if (name.Contains(" "))
            {
                var names = name.Split(' ');

                dt = db.GetDataTable("USE Genealogy SELECT TOP 1 * from Family Where FirstName LIKE @FirstName AND LastName LIKE @LastName",
                    ("@FirstName", names[0]),
                    ("@LastName", names[^1])
                    );
                foreach (DataRow row in dt.Rows)
                {
                    return row["Id"].ToString().Trim();
                }
            }
            else
            {
                dt = db.GetDataTable("USE Genealogy SELECT TOP 1 * from Family Where FirstName LIKE @name OR LastName LIKE @name ", ("@name", name));
                foreach (DataRow row in dt.Rows)
                {
                    return row["Id"].ToString().Trim();
                }
            }
            if (dt.Rows.Count == 0)
            { return null; }
            else
            { return null; }
        }

        public string GetParentNameById(int id)
        {
            var dt = db.GetDataTable("USE Genealogy SELECT FirstName, LastName from Family Where Id = @Id", ("@Id", id.ToString()));
            foreach (DataRow row in dt.Rows)
            {
                return $"{row["FirstName"].ToString().Trim()} {row["LastName"].ToString().Trim()}";
            }
            return null;
        }
        public Person GetPersonObject(DataRow row)
        {
            return new Person
            {
                Id = (int)row["Id"],
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                DateOfBirth = GetValue<int>(row["DateOfBirth"]),
                CityOfBirth = row["CityOfBirth"].ToString(),
                CityOfDeath = row["CityOfDeath"].ToString(),
                FatherId = GetValue<int>(row["FatherId"]),
                MotherId = GetValue<int>(row["MotherId"])
            };
        }

        public List<Person> List(DataTable data)
        {
            var lst = new List<Person>();
            foreach (DataRow row in data.Rows)
            {
                lst.Add(GetPersonObject(row));
            }
            return lst;
        }

        public List<Person> ListByCityOfBirth(string input)
        {
            string city = $"{input}%";
            var data = db.GetDataTable("USE Genealogy SELECT * FROM Family WHERE CityOfBirth LIKE @City", ("@City", city));
            return List(data);
        }

        public List<Person> ListByDateOfBirth(string input)
        {
            string year = $"%{input}%";
            var data = db.GetDataTable("USE Genealogy SELECT * FROM Family WHERE DateOfBirth LIKE @Year", ("@Year", year));
            return List(data);
        }

        public List<Person> ListByFather(string name)
        {
            var data = db.GetDataTable("USE Genealogy SELECT * FROM Family WHERE FatherId=@Father", ("@Father", GetParentId(name)));
            return List(data);
        }

        public List<Person> ListByFirstLetter(string letter)
        {
            string parameter = $"{letter.ToUpper()}%";
            var data = db.GetDataTable("USE Genealogy SELECT * FROM Family WHERE FirstName LIKE @Letter", ("@Letter", parameter));
            return List(data);
        }

        public List<Person> ListByMother(string name)
        {
            var data = db.GetDataTable("USE Genealogy SELECT * FROM Family WHERE MotherId=@Mother", ("@Mother", GetParentId(name)));
            return List(data);
        }

        public List<Person> ListByParents(string mother, string father)
        {
            var data = db.GetDataTable("USE Genealogy SELECT * FROM Family WHERE FatherId=@Father AND MotherId=@Mother", ("@Father", GetParentId(father)), ("@Mother", GetParentId(mother)));
            return List(data);
        }

        public List<Person> ListSiblings(Person person)
        {
            var data = db.GetDataTable("USE Genealogy SELECT * FROM Family WHERE MotherId = @Mother OR FatherId = @Father", ("@Mother", person.MotherId.ToString()), ("@Father", person.FatherId.ToString()));
            return List(data);
        }

        public List<Person> PeopleWithoutDateOfBirth()
        {
            var data = db.GetDataTable("USE Genealogy SELECT * FROM Family WHERE IsNull(DateOfBirth, '') = ''");
            return List(data);
        }

        public List<Person> PeopleWithoutParents()
        {
            var data = db.GetDataTable("USE Genealogy SELECT * FROM Family WHERE IsNull(MotherId, '') = '' OR IsNull(FatherId, '') = ''");
            return List(data);
        }

        public void Print(Person person)
        {
            if (person != null)
            {
                Console.WriteLine($"\n> Förnamn: {person.FirstName}\n> Efternamn: {person.LastName}\n> Födelsedatum: {person.DateOfBirth}");
            }
            else
            {
                Logic.PersonDontExist();
            }
        }

        public void PrintAllInfo(Person person)
        {
            if (person != null)
            {
                Console.WriteLine($"\n> Förnamn: {person.FirstName}\n> Efternamn: {person.LastName}\n> Födelsedatum: {person.DateOfBirth}\n> Födelsestad: {person.CityOfBirth}\n> Dödsstad: {person.CityOfDeath} \n> Far: {GetParentNameById(person.FatherId)}\n> Mor: {GetParentNameById(person.MotherId)}\n");
            }
            else
            {
                Logic.PersonDontExist();
            }
        }

        public void PrintWithParents(Person person)
        {
            if (person != null)
            {
                var father = GetParentNameById(person.FatherId);
                var mother = GetParentNameById(person.MotherId);
                Console.WriteLine($"\n> Förnamn: {person.FirstName}\n> Efternamn: {person.LastName}\n> Födelsedatum: {person.DateOfBirth}\n> Far: {father}\n> Mor: {mother}\n");
            }
            else
            {
                Logic.PersonDontExist();
            }
        }

        public Person Read(int id)
        {
            var row = db.GetDataTable("USE Genealogy SELECT * from Family Where firstName LIKE @id", ("@id", id.ToString()));
            if (row.Rows.Count == 0)
            { return null; }
            return GetPersonObject(row.Rows[0]);
        }

        public Person Read(Person person)
        {
            var row = db.GetDataTable("USE Genealogy SELECT * from Family Where firstName LIKE @id", ("@id", person.Id.ToString()));
            if (row.Rows.Count == 0)
            { return null; }
            return GetPersonObject(row.Rows[0]);
        }

        public Person Read(string firstName, string lastName)
        {
            var row = db.GetDataTable("USE Genealogy SELECT * from Family Where firstName LIKE @firstName AND lastName like @lastName", ("@firstName", firstName.ToString()), ("@lastName", lastName.ToString()));
            if (row.Rows.Count == 0)
            { return null; }
            return GetPersonObject(row.Rows[0]);
        }

        public Person Read(string name)
        {
            DataTable dt;
            if (name.Contains(" "))
            {
                var names = name.Split(' ');
                dt = db.GetDataTable("USE Genealogy SELECT * from Family Where FirstName LIKE @FirstName AND LastName LIKE @LastName",
                    ("@FirstName", names[0]),
                    ("@LastName", names[^1])
                    );
            }
            else
            {
                dt = db.GetDataTable("USE Genealogy SELECT * from Family Where firstName LIKE @name OR LastName LIKE @name ", ("@name", name));
            }

            if (dt.Rows.Count == 0)
            { return null; }

            return GetPersonObject(dt.Rows[0]);
        }
        public void Update(Person person)
        {
            db.ExecuteSQL(@"USE Genealogy UPDATE Family SET FirstName=@FirstName, LastName=@LastName, DateOfBirth=@DateOfBirth, FatherId=@FatherId, MotherId=@MotherId
            WHERE Id=@Id",
            ("@FirstName", person.FirstName),
            ("@LastName", person.LastName),
            ("@DateOfBirth", person.DateOfBirth.ToString()),
            ("@CityOfBirth", person.CityOfBirth),
            ("@CityOfDeath", person.CityOfDeath),
            ("@FatherId", person.FatherId.ToString()),
            ("@MotherId", person.MotherId.ToString()),
            ("@Id", person.Id.ToString())
            );
        }
    }
}