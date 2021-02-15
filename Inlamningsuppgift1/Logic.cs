using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace Inlamningsuppgift1
{
    public static class Logic
    {
        internal static Database db = new Database();
        public static FamilyMember crud = new FamilyMember();

        public static void Menu()
        {
            try
            {
                bool runProjekt = true;
                /*if (!db.CheckIfDatabaseExist("Genealogy"))
                {
                    db.CreateDB("Genealogy", true);
                    AddDatabaseData();
                } */
                while(runProjekt)
                    {
                    Console.Clear();
                    Console.WriteLine("Du är nu kopplad till databasen: " + db.DatabaseName);
                    Console.WriteLine("1. Skapa en familjemedlem");
                    Console.WriteLine("2. Redigera familjemedlem");// Meny
                    Console.WriteLine("3. Lista familjemedlemmar efter specifika kriterier");// Meny kolla denna
                    Console.WriteLine("4. Exit");

                    Console.Write("> ");
                    int input = Convert.ToInt32(Console.ReadLine());

                    switch (input)
                    {
                        case 1:
                            AddPerson();
                            break;
                        case 2:
                            EditPerson();
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                        case 5:
                            Console.WriteLine("In case I don't see you, good afternoon, good evening and good night! - Truman Burbank");
                            Console.ReadLine();
                            runProjekt = false;
                            //Environment.Exit(0);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch //(Exception ex)
            {
                //Console.WriteLine(ex);
                Menu();
            }           
        }
        public static void AddPerson()
        {
            Console.WriteLine("Du är nu kopplad till databasen: " + db.DatabaseName);
            Console.WriteLine("Ange uppgifter om personen du vill lägga till som familjemedlem");
            Console.WriteLine("Förnamn:");
            Console.Write("> ");
            string firstName = Console.ReadLine();
            Console.WriteLine("Efternamn:");
            Console.Write("> ");
            string lastName = Console.ReadLine();
            Console.WriteLine("Födelsedatum (YYMMDD):");
            Console.Write("> ");
            string dateOfBirth = Console.ReadLine();
            Console.WriteLine("Ange Fader (För- och efternamn):");
            Console.Write("> ");
            string father = Console.ReadLine();
            int fatherId = Convert.ToInt32(crud.GetParentId(father));
            Console.WriteLine("Ange Moder (För- och efternamn):");
            Console.Write("> ");
            string mother = Console.ReadLine();
            int motherId = Convert.ToInt32(crud.GetParentId(mother));

            if (!crud.CheckIfPersonExist(firstName, lastName, fatherId, motherId))
            {
                var person = new Person
                {
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dateOfBirth,
                    FatherId = fatherId,
                    MotherId = motherId,
                };
                crud.Create(person);
                Console.WriteLine("\nEn ny person är tillagd i familjeträdet!");
                ContinueOrQuit();
            }
            else
            {
                Console.WriteLine("\nPersonen existerar redan!");
                var person = crud.Read(firstName, lastName);
                crud.Print(person);
                Console.WriteLine("\nVill du redigera personen? J/N");
                string choice = Console.ReadLine();
                if (choice.ToLower().Replace(" ", "") == "n")
                {
                    Menu();
                }
                else
                {
                    EditPerson(person);
                }
            }
        }
        public static void EditPerson(Person person)
        {
            bool run = true;
            
            Console.Clear();
            if (person == null)
            { Console.WriteLine("Vänligen skriv in namnet på personen du vill redigera:");
                Console.Write("> ");
                string name = Console.ReadLine();
                person = crud.Read(name);
                crud.Print(person);
            }
            while (run)
            {
            Console.WriteLine("1. Ändra uppgifter om familjemedlem");
            Console.WriteLine("2. Ta bort familjemedlem");
            Console.WriteLine("3. Exit");
            int input = Convert.ToInt32(Console.ReadLine());

            switch (input)
            {
                
                case 1:
                Console.WriteLine("Vad är det du vill ändra? (Skriv ex. Efternamn)");
                string userInput = Console.ReadLine();
                switch (userInput.ToLower().Replace(" ", ""))
                {
                    case "förnamn":
                        Console.WriteLine("Skriv in det nya förnamnet:");
                        Console.Write("> ");
                        string newFirstName = Console.ReadLine();
                        person.FirstName = newFirstName;
                        crud.Update(person);
                        break;
                    case "efternamn":
                        Console.WriteLine("Skriv in det nya efternamnet:");
                        Console.Write("> ");
                        string newLastName = Console.ReadLine();
                        person.LastName = newLastName;
                        crud.Update(person);
                        break;
                    case "födelsedatum":
                        Console.WriteLine("Skriv in det nya födelsedatumet:");
                        Console.Write("> ");
                        string newDateOfBirth = Console.ReadLine();
                        person.FirstName = newDateOfBirth;
                        crud.Update(person);
                        break;
                    case "far":
                        Console.WriteLine("Skriv in det nya namnet på din far:");
                        Console.Write("> ");
                        string newFather = Console.ReadLine();
                        int newFatherId = Convert.ToInt32(crud.GetParentId(newFather));
                        person.FatherId = newFatherId;
                        crud.Update(person);
                        break;
                    case "mor":
                        Console.WriteLine("Skriv in det nya namnet på din mor:");
                        Console.Write("> ");
                        string newMother = Console.ReadLine();
                        int newMotherId = Convert.ToInt32(crud.GetParentId(newMother));
                        person.MotherId = newMotherId;
                        crud.Update(person);
                        break;
                }
                break;
                case 2:
                    crud.Delete(person);
                Console.WriteLine("Borttagning av person lyckad!");
                break;
                case 3:
                        run = false;
                    //Menu();
                break;
            }
        }
        }
        public static void EditPerson()
        {
            bool run = true;
            while (run)
            {
                Console.Clear();
                Console.WriteLine("Vänligen skriv in namnet på personen du vill redigera:");
                Console.Write("> ");
                string name = Console.ReadLine();
                var person = crud.Read(name);
                crud.Print(person);

                Console.WriteLine("1. Ändra uppgifter om familjemedlem");
                Console.WriteLine("2. Ta bort familjemedlem");
                Console.WriteLine("3. Exit");
                int input = Convert.ToInt32(Console.ReadLine());

                switch (input)
                {

                    case 1:
                        Console.WriteLine("Vad är det du vill ändra? (Skriv ex. Efternamn)");
                        string userInput = Console.ReadLine();
                        switch (userInput.ToLower().Replace(" ", ""))
                        {
                            case "förnamn":
                                Console.WriteLine("Skriv in det nya förnamnet:");
                                Console.Write("> ");
                                string newFirstName = Console.ReadLine();
                                person.FirstName = newFirstName;
                                crud.Update(person);
                                break;
                            case "efternamn":
                                Console.WriteLine("Skriv in det nya efternamnet:");
                                Console.Write("> ");
                                string newLastName = Console.ReadLine();
                                person.FirstName = newLastName;
                                crud.Update(person);
                                break;
                            case "födelsedatum":
                                Console.WriteLine("Skriv in det nya födelsedatumet:");
                                Console.Write("> ");
                                string newDateOfBirth = Console.ReadLine();
                                person.FirstName = newDateOfBirth;
                                crud.Update(person);
                                break;
                            case "far":
                                Console.WriteLine("Skriv in det nya namnet på din far:");
                                Console.Write("> ");
                                string newFather = Console.ReadLine();
                                int newFatherId = Convert.ToInt32(crud.GetParentId(newFather));
                                person.FatherId = newFatherId;
                                crud.Update(person);
                                break;
                            case "mor":
                                Console.WriteLine("Skriv in det nya namnet på din mor:");
                                Console.Write("> ");
                                string newMother = Console.ReadLine();
                                int newMotherId = Convert.ToInt32(crud.GetParentId(newMother));
                                person.MotherId = newMotherId;
                                crud.Update(person);
                                break;
                        }
                        break;
                    case 2:
                        crud.Delete(person);
                        Console.WriteLine("Borttagning av person lyckad!");
                        break;
                    case 3:
                        run = false;
                        //Menu();
                        break;
                }
            }
        }
        public static void ContinueOrQuit()
        {
            Console.WriteLine("Vill du fortsätta? J/N");
            string input = Console.ReadLine();
            if (input.ToLower().Replace(" ", "") == "n")
            {
                Console.WriteLine("In case I don't see you, good afternoon, good evening and good night! - Truman Burbank");
                Console.ReadLine();
                Environment.Exit(0);
            }
            else
            {
                Menu();
            }          
        }
        public static void AddDatabaseData()
        {
            db.CreateTable("[Family]", @"(
                            [Id] [int] IDENTITY (1,1) NOT NULL,
                            [FirstName] [nvarchar](255) NULL,
                            [LastName] [nvarchar](255) NULL,
                            [DateOfBirth] [nvarchar](50) NULL,
                            [FatherId] [int] NULL,
                            [MotherId] [int] NULL
                            ) ON [PRIMARY]");
            
            db.ExecuteSQL(@"
            INSERT INTO Family(firstName, LastName, DateOfBirth, FatherId, MotherId) VALUES('Ivan', 'Winroth', '200103', null, null);
            INSERT INTO Family(firstName, LastName, DateOfBirth, FatherId, MotherId) VALUES('Stina', 'Winroth', '251019', null, null);
            INSERT INTO Family(firstName, LastName, DateOfBirth, FatherId, MotherId) VALUES('Bo', 'Linde', '320716', null, null);
            INSERT INTO Family(firstName, LastName, DateOfBirth, FatherId, MotherId) VALUES('Inger', 'Linde', '360215', null, null);
            ");
            /*var morfar = new Person
            {
                FirstName = "Bo",
                LastName = "Linde",
                DateOfBirth = "320716",
                FatherId = default,
                MotherId = default,
            };
            var mormor = new Person
            {
                FirstName = "Inger",
                LastName = "Linde",
                DateOfBirth = "360215",
                FatherId = default,
                MotherId = default,
            };
            var farfar = new Person
            {
                FirstName = "Ivan",
                LastName = "Winroth",
                DateOfBirth = "200103",
                FatherId = default,
                MotherId = default,
            };
            var farmor = new Person
            {
                FirstName = "Stina",
                LastName = "Winroth",
                DateOfBirth = "251019",
                FatherId = default,
                MotherId = default,
            };
            var pappa = new Person
            {
                FirstName = "Sture",
                LastName = "Winroth",
                DateOfBirth = "571211",
                FatherId = default,
                MotherId = default,
            };
            var mamma = new Person
            {
                FirstName = "Ingela",
                LastName = "Winroth",
                DateOfBirth = "610608",
                FatherId = default,
                MotherId = default,
            };
            var bror = new Person
            {
                FirstName = "Fredrik",
                LastName = "Winroth",
                DateOfBirth = "840401",
                FatherId = default,
                MotherId = default,
            };
            var storaSyster = new Person
            {
                FirstName = "Charlotte",
                LastName = "Fransson",
                DateOfBirth = "860331",
                FatherId = default,
                MotherId = default,
            };
            var jag = new Person
            {
                FirstName = "Sarah",
                LastName = "Winroth",
                DateOfBirth = "890928",
                FatherId = default,
                MotherId = default,
            };
            var lillaSyster = new Person
            {
                FirstName = "Rebecca",
                LastName = "Winroth",
                DateOfBirth = "970818",
                FatherId = default,
                MotherId = default,
            };
            crud.Create(morfar);
            crud.Create(mormor);
            crud.Create(farfar);
            crud.Create(farmor);
            crud.Create(pappa);
            crud.Create(mamma);
            crud.Create(bror);
            crud.Create(storaSyster);
            crud.Create(jag);
            crud.Create(lillaSyster);*/
        }
    }
}
