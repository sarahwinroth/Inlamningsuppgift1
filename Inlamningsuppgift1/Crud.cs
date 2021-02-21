using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Inlamningsuppgift1
{
    public class Crud
    {
        internal static Database db = new Database();

        internal Database Database
        {
            get => default;
            set
            {
            }
        }

        public Person Person
        {
            get => default;
            set
            {
            }
        }

        /// <summary>
        /// En generisk-metod som tar emot värdet på ett objekt och kollar om värdet är lika med null eller inte har ett värde.
        /// Om ja så returneras default-typen annars returneras värdet tillbaka som "typkastats" till värdet den "skickas in till"
        /// </summary>
        /// <param name="value">Värdet på objekt</param>
        /// <returns>Ett generisk typvärde</returns>
        public static T GetValue<T>(object value)
        {
            if (value == null || value == DBNull.Value)
                return default;
            else
                return (T)value;
        }
        /// <summary>
        /// Kollar om en person finns i databasen genom att söka på för- och efternamn samt mor och far. Om ja så returneras true om personen inte finns så returneras false
        /// </summary>
        /// <returns>Bool, Sant eller falskt</returns>
        public bool CheckIfPersonExist(string firstName, string lastName, int fatherId, int motherId)
        {
            var row = db.GetDataTable("SELECT TOP 1 * FROM dbo.Family WHERE FirstName LIKE @FirstName AND LastName LIKE @LastName AND FatherId = @FatherId AND MotherId = @MotherId", ("@FirstName", firstName.ToString()), ("@LastName", lastName.ToString()), ("@FatherId", fatherId.ToString()), ("@MotherId", motherId.ToString()));
            if (row.Rows.Count > 0)
            { return true; }
            else
            { return false; }
        }
        /// <summary>
        /// Kollar om en person finns i databasen genom person objekt som tas emot som parameter. Om personen finns så returneras true om personen inte finns så returneras false
        /// </summary>
        /// <returns>Bool, Sant eller falskt</returns>
        public bool CheckIfPersonExist(Person person)
        {
            var row = db.GetDataTable("SELECT TOP 1 * FROM dbo.Family WHERE FirstName LIKE @FirstName AND LastName LIKE @LastName", ("@FirstName", person.FirstName.ToString()), ("@LastName", person.LastName.ToString()));
            if (row.Rows.Count > 0)
            { return true; }
            else
            { return false; }
        }
        /// <summary>
        /// Lägger till en person i databasen med hjälp av person objektet som skickas in som parameter för att lägga in alla värden
        /// </summary>
        public void Create(Person person)
        {
            try
            {
                var connectionString = string.Format(db.ConnectionString, db.DatabaseName);
                using (var cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();
                    var sql = $"INSERT INTO dbo.Family(FirstName, LastName, DateOfBirth, Birthplace, DateOfDeath, Deathplace, FatherId, MotherId) VALUES(@FirstName, @LastName, @DateOfBirth, @Birthplace, @DateOfDeath, @Deathplace, @FatherId, @MotherId)";
                    using (var cmd = new SqlCommand(sql, cnn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", person.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", person.LastName);
                        cmd.Parameters.AddWithValue("@DateOfBirth", person.DateOfBirth);
                        cmd.Parameters.AddWithValue("@Birthplace", person.Birthplace);
                        cmd.Parameters.AddWithValue("@DateOfDeath", person.DateOfDeath);
                        cmd.Parameters.AddWithValue("@Deathplace", person.Deathplace);
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
        /// <summary>
        /// Tar bort en person från databasen med hjälp av person objektet som skickas in som parameter och tar bort personen genom personens Id
        /// </summary>
        public void Delete(Person person)
        {
            db.ExecuteSQL("DELETE FROM dbo.Family WHERE Id=@Id", ("@Id", person.Id.ToString()));
        }
        /// <summary>
        /// Söker efter person/förälder med hjälp av namn-parametern som skickas in i metoden. 
        /// Informationen som hämtas sparas i en DataTable "tabell" och genom att loopa igenom raderna i tabellen får man fram Id för den personen och returnerar det.
        /// </summary>
        /// <returns>String, Id för förälder</returns>
        public string GetParentId(string name)
        {
            DataTable data;
            if (name.Contains(" "))
            {
                var names = name.Split(' ');

                data = db.GetDataTable("SELECT * FROM dbo.Family WHERE FirstName LIKE @FirstName AND LastName LIKE @LastName",
                    ("@FirstName", names[0]),
                    ("@LastName", names[^1])
                    );
                foreach (DataRow row in data.Rows)
                {
                    return row["Id"].ToString().Trim();
                }
            }
            else
            {
                data = db.GetDataTable("SELECT * FROM dbo.Family WHERE FirstName LIKE @name OR LastName LIKE @name ", ("@name", name));
                foreach (DataRow row in data.Rows)
                {
                    return row["Id"].ToString().Trim();
                }
            }
            if (data.Rows.Count == 0)
            { return null; }
            else
            { return null; }
        }
        /// <summary>
        /// Söker efter person/förälder med hjälp av Id-parametern som skickas in i metoden. 
        /// Informationen som hämtas sparas i en DataTable "tabell" och genom att loopa igenom raderna i tabellen får man fram och returnerar För-och efternamn för den personen.
        /// </summary>
        /// <returns>String, för-ochefternamn på föräldern</returns>
        public string GetParentNameById(int id)
        {
            var data = db.GetDataTable("SELECT FirstName, LastName FROM dbo.Family WHERE Id = @Id", ("@Id", id.ToString()));
            foreach (DataRow row in data.Rows)
            {
                return $"{row["FirstName"].ToString().Trim()} {row["LastName"].ToString().Trim()}";
            }
            return null;
        }
        /// <summary>
        /// Hämtar personer från listan som skickats in som parameter och skriver ut dom. 
        /// Om listan är tom så skrivs ett meddelande ut om att ingen person matchar för sökningen.
        /// </summary>

        public void GetPeopleFromList(List<Person> list)
        {
            if (list.Count != 0)
            {
                foreach (var p in list)
                {
                    Crud crud = new Crud();
                    crud.Print(p);
                }
            }
            if (list.Count == 0)
            {
                Console.WriteLine("Det finns ingen person som matchar din sökning, vänligen försök igen!");
            }
        }
        /// <summary>
        /// Hämtar personer från listan som skickats in som parameter och skriver ut dom tillsammans med deras Id. 
        /// Om listan är tom så skrivs ett meddelande ut om att ingen person matchar för sökningen.
        /// </summary>
        public void GetPeopleFromListWithId(List<Person> list)
        {
            if (list.Count != 0)
            {
                foreach (var p in list)
                {
                    Crud crud = new Crud();
                    crud.PrintWithId(p);
                }
            }
            if (list.Count == 0)
            {
                Console.WriteLine("Det finns ingen person som matchar din sökning, vänligen försök igen!");
            }
        }
        /// <summary>
        /// Hämtar person som är i Datarow raden som skickats in som parameter och returnerar person objektet. 
        /// </summary>
        /// <returns>Ett person objekt</returns>
        public Person GetPersonObject(DataRow row)
        {
            return new Person
            {
                Id = (int)row["Id"],
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                DateOfBirth = GetValue<int>(row["DateOfBirth"]),
                Birthplace = row["Birthplace"].ToString(),
                DateOfDeath = GetValue<int>(row["DateOfDeath"]),
                Deathplace = row["Deathplace"].ToString(),
                FatherId = GetValue<int>(row["FatherId"]),
                MotherId = GetValue<int>(row["MotherId"])
            };
        }
        /// <summary>
        /// Hämtar och skriver ut 3 generationer utifrån person objektet som skickas in som parameter. 
        /// metoden utgår från personen och hämtar personens föräldrar och utifrån personens föräldrar hämtar personens mor- och farföräldrar
        /// </summary>
        public void GetThreeGenerations(Person person)
        {
            Crud crud = new Crud();
            Console.WriteLine($"\nPerson/Barn: {person.FirstName} {person.LastName}");

            string nameOfMother = crud.GetParentNameById(person.MotherId);
            Person theMother;
            if (nameOfMother != null)
            {
                theMother = crud.Read(nameOfMother);
                Console.WriteLine($"Mor: {theMother.FirstName} {theMother.LastName}");
                string nameOfMomsMother = crud.GetParentNameById(theMother.MotherId);
                Person momsMother;
                if (nameOfMomsMother != null)
                {
                    momsMother = crud.Read(nameOfMomsMother);
                    Console.WriteLine($"Mormor: {momsMother.FirstName} {momsMother.LastName}");
                }
                else { Console.WriteLine("Mormor: Finns inte lagrad i databasen"); }

                string nameOfMomsFather = crud.GetParentNameById(theMother.FatherId);
                Person momsFather;
                if (nameOfMomsFather != null)
                {
                    momsFather = crud.Read(nameOfMomsFather);
                    Console.WriteLine($"Morfar: {momsFather.FirstName} {momsFather.LastName}");
                }
                else { Console.WriteLine("Morfar: Finns inte lagrad i databasen"); }
            }
            else
            { Console.WriteLine("Mor: Finns inte lagrad i databasen"); }

            string nameOfFather = crud.GetParentNameById(person.FatherId);
            Person theFather;
            if (nameOfFather != null)
            {
                theFather = crud.Read(nameOfFather);
                Console.WriteLine($"Far: {theFather.FirstName} {theFather.LastName}");
                string nameOfDadsMother = crud.GetParentNameById(theFather.MotherId);
                Person dadsMother;
                if (nameOfDadsMother != null)
                {
                    dadsMother = crud.Read(nameOfDadsMother);
                    Console.WriteLine($"Farmor: {dadsMother.FirstName} {dadsMother.LastName}");
                }
                else
                { Console.WriteLine("Farmor: Finns inte lagrad i databasen"); }

                string nameOfDadsFather = crud.GetParentNameById(theFather.FatherId);
                Person dadsFather;
                if (nameOfDadsFather != null)
                {
                    dadsFather = crud.Read(nameOfDadsFather);
                    Console.WriteLine($"Farfar: {dadsFather.FirstName} {dadsFather.LastName}");
                }
                else
                { Console.WriteLine("Farfar: Finns inte lagrad i databasen"); }
            }
            else
            { Console.WriteLine("Far: Finns inte lagrad i databasen"); }
        }
        /// <summary>
        /// Metoden kollar om det existerar fler än en person i datatbasen med samma namn som den parameter som skickas in i metoden.
        /// Om fler personer med samma namn existerar så returneras true, om bara en så returneras false.
        /// </summary>
        /// <returns>Bool, Sant eller falskt</returns>
        public bool IfReadingManyPeople(string name)
        {
            DataTable dt;
            if (name.Contains(" "))
            {
                var names = name.Split(' ');
                dt = db.GetDataTable("SELECT * FROM dbo.Family WHERE FirstName LIKE @FirstName AND LastName LIKE @LastName",
                    ("@FirstName", names[0]),
                    ("@LastName", names[^1])
                    );
            }
            else
            {
                dt = db.GetDataTable("SELECT * FROM dbo.Family WHERE FirstName LIKE @Name OR LastName LIKE @Name", ("@Name", name));
            }

            if (dt.Rows.Count > 1)
            { return true; }
            else
            { return false; }
        }
        /// <summary>
        /// Metoden tar emot en tabell, loopar igenom den och för varje intervall så sparas personen (efter att raden lagrats i ett objekt) i listan som sedan returneras.
        /// </summary>
        /// <returns>En lista med person objekt</returns>
        public List<Person> List(DataTable data)
        {
            var lst = new List<Person>();
            foreach (DataRow row in data.Rows)
            {
                lst.Add(GetPersonObject(row));
            }
            return lst;
        }
        /// <summary>
        /// Metoden listar alla personer som är födda i samma stad. Strängen som metoden tar emot som parameter är staden som sökningen utgår ifrån.
        /// Informationen/personerna som passar in på sökningen hämtas från databasen och sparas i en tabell som skickas till List-metoden som lagrar personerna i en lista.
        /// </summary>
        /// <returns>En lista med person objekt</returns>
        public List<Person> ListByBirthplace(string input)
        {
            string city = $"%{input}%";
            var data = db.GetDataTable("SELECT * FROM dbo.Family WHERE Birthplace LIKE @City", ("@City", city));
            return List(data);
        }
        /// <summary>
        /// Metoden listar alla personer som är födda samma år. Strängen som metoden tar emot som parameter är året som sökningen utgår ifrån.
        /// Informationen/personerna som passar in på sökningen hämtas från databasen och sparas i en tabell som skickas till List-metoden som lagrar personerna i en lista.
        /// </summary>
        /// <returns>En lista med person objekt</returns>
        public List<Person> ListByDateOfBirth(string input)
        {
            string year = $"%{input}";
            var data = db.GetDataTable("SELECT * FROM dbo.Family WHERE DateOfBirth LIKE @Year", ("@Year", year));
            return List(data);
        }
        /// <summary>
        /// Metoden listar alla personer som har samma fader. Strängen som metoden tar emot som parameter är namnet på fadern som sökningen utgår ifrån.
        /// Informationen/personerna som passar in på sökningen hämtas från databasen och sparas i en tabell som skickas till List-metoden som lagrar personerna i en lista.
        /// </summary>
        /// <returns>En lista med person objekt</returns>
        public List<Person> ListByFather(string name)
        {
            var data = db.GetDataTable("SELECT * FROM dbo.Family WHERE FatherId=@Father", ("@Father", GetParentId(name)));
            return List(data);
        }
        /// <summary>
        /// Metoden listar alla personer som börjar på samma bokstav. Strängen som metoden tar emot som parameter är bokstaven som sökningen utgår ifrån.
        /// Informationen/personerna som passar in på sökningen hämtas från databasen och sparas i en tabell som skickas till List-metoden som lagrar personerna i en lista.
        /// </summary>
        /// <returns>En lista med person objekt</returns>
        public List<Person> ListByFirstLetter(string letter)
        {
            string parameter = $"{letter.ToUpper()}%";
            var data = db.GetDataTable("SELECT * FROM dbo.Family WHERE FirstName LIKE @Letter", ("@Letter", parameter));
            return List(data);
        }
        /// <summary>
        /// Metoden listar alla personer som har samma moder. Strängen som metoden tar emot som parameter är namnet på modern som sökningen utgår ifrån.
        /// Informationen/personerna som passar in på sökningen hämtas från databasen och sparas i en tabell som skickas till List-metoden som lagrar personerna i en lista.
        /// </summary>
        /// <returns>En lista med person objekt</returns>
        public List<Person> ListByMother(string name)
        {
            var data = db.GetDataTable("SELECT * FROM dbo.Family WHERE MotherId=@Mother", ("@Mother", GetParentId(name)));
            return List(data);
        }
        /// <summary>
        /// Metoden listar alla personer som har samma namn. Strängen som metoden tar emot som parameter är namnet på personen som sökningen utgår ifrån.
        /// Informationen/personerna som passar in på sökningen hämtas från databasen och sparas i en tabell som skickas till List-metoden som lagrar personerna i en lista.
        /// </summary>
        /// <returns>En lista med person objekt</returns>
        public List<Person> ListByName(string name)
        {
            DataTable data;
            if (name.Contains(" "))
            {
                var names = name.Split(' ');
                data = db.GetDataTable("SELECT * FROM dbo.Family WHERE FirstName LIKE @FirstName AND LastName LIKE @LastName",
                    ("@FirstName", names[0]),
                    ("@LastName", names[^1])
                    );
            }
            else
            {
                data = db.GetDataTable("SELECT * FROM dbo.Family WHERE firstName LIKE @name OR LastName LIKE @name ", ("@name", name));
            }
            return List(data);
        }
        /// <summary>
        /// Metoden listar alla personer som har samma moder och fader. Metoden tar emot två strängar som motsvarar moderns namn och faderns namn, sökningen utgår från moderns och faderns id som hämtas med hjälp av namnen genom en annan metod.
        /// Informationen/personerna som passar in på sökningen hämtas från databasen och sparas i en tabell som skickas till List-metoden som lagrar personerna i en lista.
        /// </summary>
        /// <returns>En lista med person objekt</returns>
        public List<Person> ListByParents(string mother, string father)
        {
            var data = db.GetDataTable("SELECT * FROM dbo.Family WHERE FatherId=@Father AND MotherId=@Mother ORDER BY DateOfBirth ASC", ("@Father", GetParentId(father)), ("@Mother", GetParentId(mother)));
            return List(data);
        }
        /// <summary>
        /// Metoden listar alla personer/syskon som har samma moder eller fader. Metoden tar emot ett person objekt och söker på alla personer som has samma moderId eller faderId. 
        /// Informationen/personerna som passar in på sökningen hämtas från databasen och sparas i en tabell som skickas till List-metoden som lagrar personerna i en lista.
        /// </summary>
        /// <returns>En lista med person objekt</returns>
        public List<Person> ListSiblings(Person person)
        {
            var data = db.GetDataTable("SELECT * FROM dbo.Family WHERE MotherId = @Mother OR FatherId = @Father", ("@Mother", person.MotherId.ToString()), ("@Father", person.FatherId.ToString()));
            return List(data);
        }
        /// <summary>
        /// Metoden listar alla personer som saknar födelsedatum. 
        /// Informationen/personerna som passar in på sökningen hämtas från databasen och sparas i en tabell som skickas till List-metoden som lagrar personerna i en lista.
        /// </summary>
        /// <returns>En lista med person objekt</returns>
        public List<Person> PeopleWithoutDateOfBirth()
        {
            var data = db.GetDataTable("SELECT * FROM dbo.Family WHERE IsNull(DateOfBirth, '') = ''");
            return List(data);
        }
        /// <summary>
        /// Metoden listar alla personer som saknar moder eller fader. 
        /// Informationen/personerna som passar in på sökningen hämtas från databasen och sparas i en tabell som skickas till List-metoden som lagrar personerna i en lista.
        /// </summary>
        /// <returns>En lista med person objekt</returns>
        public List<Person> PeopleWithoutParents()
        {
            var data = db.GetDataTable("SELECT * FROM dbo.Family WHERE IsNull(MotherId, '') = '' OR IsNull(FatherId, '') = ''");
            return List(data);
        }
        /// <summary>
        /// Metoden skriver ut de värden som finns lagrade för det person objektet som tas emot som parameter. Metoden skriver ut personens för- och efternamn samt födelsedatum.       
        /// </summary>
        public void Print(Person person)
        {
            if (person != null)
            {
                Console.WriteLine($"\n> Förnamn: {person.FirstName}\n> Efternamn: {person.LastName}\n> Födelsedatum: {person.DateOfBirth}");
            }
            else
            {
                string choice = Logic.PersonDontExist();
                if (choice.ToLower().Replace(" ", "") == "n")
                {
                    Logic.Menu();
                }
                else
                {
                    Logic.AddPerson();
                }
            }
        }
        /// <summary>
        /// Metoden skriver ut de värden som finns lagrade för det person objektet som tas emot som parameter. Metoden skriver ut personens för- och efternamn, födelsedatum, födelsestad, dödsstad, far och mor.       
        /// </summary>
        public void PrintAllInfo(Person person)
        {
            if (person != null)
            {
                Console.WriteLine($"\n> Förnamn: {person.FirstName}\n> Efternamn: {person.LastName}\n> Födelsedatum: {person.DateOfBirth}\n> Födelsestad: {person.Birthplace}\n> Dödsdatum: {person.DateOfDeath}\n> Dödsstad: {person.Deathplace} \n> Far: {GetParentNameById(person.FatherId)}\n> Mor: {GetParentNameById(person.MotherId)}");
            }
            else
            {
                Logic.PersonDontExist();
            }
        }
        /// <summary>
        /// Metoden skriver ut de värden som finns lagrade för det person objektet som tas emot som parameter. Metoden skriver ut personens id, för- och efternamn, födelsedatum, födelsestad, dödsstad, far och mor.       
        /// </summary>
        public void PrintWithId(Person person)
        {
            if (person != null)
            {
                Console.WriteLine($"\n> ID: {person.Id}\n> Förnamn: {person.FirstName}\n> Efternamn: {person.LastName}\n> Födelsedatum: {person.DateOfBirth}\n> Födelsestad: {person.Birthplace}\n> Dödsdatum: {person.DateOfDeath}\n> Dödsstad: {person.Deathplace} \n> Far: {GetParentNameById(person.FatherId)}\n> Mor: {GetParentNameById(person.MotherId)}");
            }
            else
            {
                Logic.PersonDontExist();
            }
        }
        /// <summary>
        /// Metoden skriver ut de värden som finns lagrade för det person objektet som tas emot som parameter. Metoden skriver ut personens id, för- och efternamn, födelsedatum och namn på far och mor.       
        /// </summary>
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
        /// <summary>
        /// Metoden läser alla personer som finns i databasen och tabellen Family. Sökningen utgår från Id-numret som tas emot som parameter. 
        /// Informationen/personerna som passar in på sökningen hämtas från databasen och sparas i en tabell. Första raden i tabellen skickas till en metod som lagrar värdena i ett person objekt, det person objekt som returneras.
        /// </summary>
        /// <returns>Ett person objekt</returns>
        public Person Read(int id)
        {
            var row = db.GetDataTable("SELECT * FROM dbo.Family WHERE Id = @Id", ("@Id", id.ToString()));
            if (row.Rows.Count == 0)
            { return null; }
            return GetPersonObject(row.Rows[0]);
        }
        /// <summary>
        /// Metoden läser alla personer som finns i databasen och tabellen Family. Sökningen utgår från för-och efternamn som tas emot som parameter. 
        /// Informationen/personerna som passar in på sökningen hämtas från databasen och sparas i en tabell. Första raden i tabellen skickas till en metod som lagrar värdena i ett person objekt, det person objekt som returneras.
        /// </summary>
        /// <returns>Ett person objekt</returns>
        public Person Read(string firstName, string lastName)
        {
            var row = db.GetDataTable("SELECT * FROM dbo.Family WHERE firstName LIKE @firstName AND lastName like @lastName", ("@firstName", firstName.ToString()), ("@lastName", lastName.ToString()));
            if (row.Rows.Count == 0)
            { return null; }
            return GetPersonObject(row.Rows[0]);
        }
        /// <summary>
        /// Metoden läser alla personer som finns i databasen och tabellen Family. Sökningen utgår från namnet/strängen som tas emot som parameter. 
        /// Informationen/personerna som passar in på sökningen hämtas från databasen och sparas i en tabell. Första raden i tabellen skickas till en metod som lagrar värdena i ett person objekt, det person objekt som returneras.
        /// </summary>
        /// <returns>Ett person objekt</returns>
        public Person Read(string name)
        {
            DataTable dt;
            if (name == null)
            { return null; }
            if (name.Contains(" "))
            {
                var names = name.Split(' ');
                dt = db.GetDataTable("SELECT * FROM dbo.Family WHERE FirstName LIKE @FirstName AND LastName LIKE @LastName",
                    ("@FirstName", names[0]),
                    ("@LastName", names[^1])
                    );
            }
            else
            {
                dt = db.GetDataTable("SELECT * FROM dbo.Family WHERE firstName LIKE @name OR LastName LIKE @name ", ("@name", name));
            }

            if (dt.Rows.Count == 0)
            { return null; }

            return GetPersonObject(dt.Rows[0]);
        }
        /// <summary>
        /// Metoden updaterar en person i databasen med hjälp av Id, och "sätter" eller anger person objektets (som tas emot som parameter) värden i respektive kolumner för den personen.      
        /// </summary>
        /// <returns>Ett person objekt</returns>
        public void Update(Person person)
        {
            db.ExecuteSQL(@"UPDATE dbo.Family SET FirstName=@FirstName, LastName=@LastName, DateOfBirth=@DateOfBirth, Birthplace=@Birthplace, DateOfDeath=@DateOfDeath, Deathplace=@Deathplace, FatherId=@FatherId, MotherId=@MotherId
            WHERE Id=@Id",
            ("@FirstName", person.FirstName),
            ("@LastName", person.LastName),
            ("@DateOfBirth", person.DateOfBirth.ToString()),
            ("@Birthplace", person.Birthplace),
            ("@DateOfDeath", person.DateOfDeath.ToString()),
            ("@Deathplace", person.Deathplace),
            ("@FatherId", person.FatherId.ToString()),
            ("@MotherId", person.MotherId.ToString()),
            ("@Id", person.Id.ToString())
            );
        }
    }
}