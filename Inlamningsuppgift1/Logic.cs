using System;
using System.Collections.Generic;

namespace Inlamningsuppgift1
{
    public static class Logic
    {
        public static Crud crud = new Crud();
        internal static Database db = new Database();
        public static void AddDatabaseData()
        {
            db.CreateTable("[Family]", @"(
                            [Id] [int] IDENTITY (1,1) NOT NULL,
                            [FirstName] [nvarchar](255) NULL,
                            [LastName] [nvarchar](255) NULL,
                            [DateOfBirth] [int] DEFAULT 0,
                            [CityOfBirth] [nvarchar](255) NULL,
                            [CityOfDeath] [nvarchar](255) NULL,
                            [FatherId] [int] DEFAULT 0,
                            [MotherId] [int] DEFAULT 0
                            ) ON [PRIMARY]");

            db.ExecuteSQL(@"
            INSERT INTO Family(FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES('Ivan', 'Winroth', 1920, 'Öttum, Sverige', 'Kvänum, Sverige', null, null);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES('Stina', 'Winroth', 1925, 'Öttum, Sverige', 'Kvänum, Sverige', null, null);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES('Bo', 'Linde', 1932, 'Vemdalen, Sverige', 'Skara, Sverige', null, null);
            INSERT INTO Family(firstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES('Inger', 'Linde', 1936, 'Karlstad, Sverige', 'Skara, Sverige', null, null);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES('Sture', 'Winroth', 1957, 'Öttum, Sverige', null, 1, 2);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES('Ingela', 'Winroth', 1961, 'Karlstad, Sverige', null, 3, 4);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES('Fredrik', 'Winroth', 1984, 'Öttum, Sverige', null, 5, 6);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES('Charlotte', 'Fransson', 1986, 'Öttum, Sverige', null, 5, 6);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES('Sarah', 'Winroth', 1989, 'Öttum, Sverige', null, 5, 6);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES('Rebecca', 'Winroth', 1997, 'Öttum, Sverige', null, 5, 6);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES('Magnus', 'Fransson', 1985, 'Tråvad, Sverige', null, null, null);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES('Wille', 'Fransson', 2013, 'Tråvad, Sverige', null, 11, 8);
            INSERT INTO Family(FirstName, LastName, DateOfBirth, CityOfBirth, CityOfDeath, FatherId, MotherId) VALUES('Saga', 'Fransson', 2015, 'Tråvad, Sverige', null, 11, 8);
            ");
        }

        public static void AddPerson()
        {
            Console.Clear();
            Console.WriteLine("[LÄGG TILL PERSON]");
            Console.WriteLine("Ange uppgifter om personen du vill lägga till som familjemedlem");
            Console.WriteLine("Förnamn:");
            Console.Write("> ");
            string firstName = Console.ReadLine();
            Console.WriteLine("Efternamn:");
            Console.Write("> ");
            string lastName = Console.ReadLine();
            Console.WriteLine("Födelsedatum (YYYY):");
            Console.Write("> ");
            string yearOfBirth = Console.ReadLine().Trim();
            int dateOfBirth;
            if (yearOfBirth == "")
            { dateOfBirth = 0; }
            else
            { dateOfBirth = Convert.ToInt32(yearOfBirth); }
            Console.WriteLine("Födelsestad:");
            Console.Write("> ");
            string cityOfBirth = Console.ReadLine();
            Console.WriteLine("Dödsstad:");
            Console.Write("> ");
            string cityOfDeath = Console.ReadLine();
            Console.WriteLine("Ange Far (För- och efternamn):");
            Console.Write("> ");
            string father = Console.ReadLine();
            int fatherId = Convert.ToInt32(crud.GetParentId(father));
            Console.WriteLine("Ange Mor (För- och efternamn):");
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
                    CityOfBirth = cityOfBirth,
                    CityOfDeath = cityOfDeath,
                    FatherId = fatherId,
                    MotherId = motherId,
                };
                crud.Create(person);
                Console.WriteLine("\nEn ny person är tillagd i familjeträdet!");
                ContinueOrQuitAddPerson();
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

        public static void ContinueOrQuitAddPerson()
        {
            Console.WriteLine("Vill du fortsätta? J/N");
            string input = Console.ReadLine();
            if (input.ToLower().Replace(" ", "") == "n")
            {
                Menu();
            }
            else
            {
                AddPerson();
            }
        }

        public static void ContinueOrQuitEditPerson()
        {
            Console.WriteLine("Vill du fortsätta? J/N");
            string input = Console.ReadLine();
            if (input.ToLower().Replace(" ", "") == "n")
            {
                Menu();
            }
            else
            {
                EditPerson();
            }
        }

        public static void EditPerson(Person person)
        {
            bool run = true;

            while (run)
            {
                Console.WriteLine($"\n1. Ändra uppgifter om {person.FirstName} {person.LastName}");
                Console.WriteLine($"2. Ta bort familjemedlem {person.FirstName} {person.LastName}");
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
                                ContinueOrQuitEditPerson();
                                break;

                            case "efternamn":
                                Console.WriteLine("Skriv in det nya efternamnet:");
                                Console.Write("> ");
                                string newLastName = Console.ReadLine();
                                person.LastName = newLastName;
                                crud.Update(person);
                                ContinueOrQuitEditPerson();
                                break;

                            case "födelsedatum":
                                Console.WriteLine("Skriv in det nya födelsedatumet:");
                                Console.Write("> ");
                                int newDateOfBirth = Convert.ToInt32(Console.ReadLine());
                                person.DateOfBirth = newDateOfBirth;
                                crud.Update(person);
                                ContinueOrQuitEditPerson();
                                break;

                            case "födelsstad":
                                Console.WriteLine("Skriv in det nya födelsedatumet:");
                                Console.Write("> ");
                                string newCityOfBirth = Console.ReadLine();
                                person.CityOfBirth = newCityOfBirth;
                                crud.Update(person);
                                ContinueOrQuitEditPerson();
                                break;

                            case "dödsstad":
                                Console.WriteLine("Skriv in det nya födelsedatumet:");
                                Console.Write("> ");
                                string newCityOfDeath = Console.ReadLine();
                                person.CityOfDeath = newCityOfDeath;
                                crud.Update(person);
                                ContinueOrQuitEditPerson();
                                break;

                            case "far":
                                Console.WriteLine("Skriv in det nya namnet på din far:");
                                Console.Write("> ");
                                string newFather = Console.ReadLine();
                                int newFatherId = Convert.ToInt32(crud.GetParentId(newFather));
                                person.FatherId = newFatherId;
                                crud.Update(person);
                                ContinueOrQuitEditPerson();
                                break;

                            case "mor":
                                Console.WriteLine("Skriv in det nya namnet på din mor:");
                                Console.Write("> ");
                                string newMother = Console.ReadLine();
                                int newMotherId = Convert.ToInt32(crud.GetParentId(newMother));
                                person.MotherId = newMotherId;
                                crud.Update(person);
                                ContinueOrQuitEditPerson();
                                break;
                        }
                        break;

                    case 2:
                        crud.Delete(person);
                        Console.WriteLine("Borttagning av person lyckad!");
                        Console.WriteLine("[TRYCK ENTER]");
                        Console.ReadLine();
                        break;

                    case 3:
                        run = false;
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
                crud.PrintAllInfo(person);

                Console.WriteLine("\n1. Ändra uppgifter om familjemedlem");
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
                                ContinueOrQuitEditPerson();
                                break;

                            case "efternamn":
                                Console.WriteLine("Skriv in det nya efternamnet:");
                                Console.Write("> ");
                                string newLastName = Console.ReadLine();
                                person.FirstName = newLastName;
                                crud.Update(person);
                                ContinueOrQuitEditPerson();
                                break;

                            case "födelsedatum":
                                Console.WriteLine("Skriv in det nya födelsedatumet:");
                                Console.Write("> ");
                                int newDateOfBirth = Convert.ToInt32(Console.ReadLine());
                                person.DateOfBirth = newDateOfBirth;
                                crud.Update(person);
                                ContinueOrQuitEditPerson();
                                break;

                            case "födelsestad":
                                Console.WriteLine("Skriv in den nya födelsestaden:");
                                Console.Write("> ");
                                string newCityOfBirth = Console.ReadLine();
                                person.CityOfBirth = newCityOfBirth;
                                crud.Update(person);
                                ContinueOrQuitEditPerson();
                                break;

                            case "dödsstad":
                                Console.WriteLine("Skriv in den nya dödstaden:");
                                Console.Write("> ");
                                string newCityOfDeath = Console.ReadLine();
                                person.CityOfDeath = newCityOfDeath;
                                crud.Update(person);
                                ContinueOrQuitEditPerson();
                                break;

                            case "far":
                                Console.WriteLine("Skriv in det nya namnet på din far:");
                                Console.Write("> ");
                                string newFather = Console.ReadLine();
                                int newFatherId = Convert.ToInt32(crud.GetParentId(newFather));
                                person.FatherId = newFatherId;
                                crud.Update(person);
                                ContinueOrQuitEditPerson();
                                break;

                            case "mor":
                                Console.WriteLine("Skriv in det nya namnet på din mor:");
                                Console.Write("> ");
                                string newMother = Console.ReadLine();
                                int newMotherId = Convert.ToInt32(crud.GetParentId(newMother));
                                person.MotherId = newMotherId;
                                crud.Update(person);
                                ContinueOrQuitEditPerson();
                                break;
                        }
                        break;

                    case 2:
                        crud.Delete(person);
                        Console.WriteLine("Borttagning av person lyckad!");
                        ContinueOrQuitEditPerson();
                        break;

                    case 3:
                        run = false;
                        break;
                }
            }
        }

        public static void GetPersonsFromList(List<Person> list)
        {
            if (list.Count != 0)
            {
                foreach (var p in list)
                {
                    crud.Print(p);
                }
            }
            if (list.Count == 0)
            {
                Console.WriteLine("Det finns ingen person som matchar din sökning, vänligen försök igen!");
            }
        }

        public static void Menu()
        {
            try
            {
                bool runProjekt = true;
                if (!db.CheckIfDatabaseExist("Genealogy"))
                {
                    db.DatabaseName = "Master";
                    db.CreateDB("Genealogy", true);
                    AddDatabaseData();
                }
                else
                { db.DatabaseName = "Genealogy"; }
                while (runProjekt)
                {
                    Console.Clear();
                    Console.WriteLine("Du är nu kopplad till databasen: " + db.DatabaseName);
                    Console.WriteLine("1. Skapa en familjemedlem");
                    Console.WriteLine("2. Redigera familjemedlem");
                    Console.WriteLine("3. Sök i familjeträdet");
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
                            SearchInFamily();
                            break;

                        case 4:
                            Console.WriteLine("In case I don't see you, good afternoon, good evening and good night! - Truman Burbank");
                            Console.ReadLine();
                            runProjekt = false;
                            break;

                        default:
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Fel inmatning, vänligen försök igen!");
                Console.WriteLine("[TRYCK ENTER]");
                Console.ReadLine();
                Menu();
            }
        }
        public static void PersonDontExist()
        {
            Console.WriteLine("Personen existerar inte!");
            Console.WriteLine("\nVill du lägga till en familjemedlem? J/N");
            string choice = Console.ReadLine();
            if (choice.ToLower().Replace(" ", "") == "n")
            {
                SearchInFamily();
            }
            else
            {
                AddPerson();
            }
        }

        public static void SearchInFamily()
        {
            bool run = true;
            while (run)
            {
                Person person;
                string userInput;
                List<Person> ListOfPersons;
                Console.Clear();
                Console.WriteLine("[SÖK EFTER..]");
                Console.WriteLine("1. En specifik person i familjeträdet");
                Console.WriteLine("2. Personer som börjar på en specifik bokstav");
                Console.WriteLine("3. Personer som är födda ett visst år");
                Console.WriteLine("4. Personer med samma föräldrar");
                Console.WriteLine("5. Syskon med samma mor");
                Console.WriteLine("6. Syskon med samma far");
                Console.WriteLine("7. Syskon till en person");
                Console.WriteLine("8. Person och deras mor/far-föräldrar");
                Console.WriteLine("9. Personer som är födda i samma stad");
                Console.WriteLine("10. Personer som saknar födelsedatum");
                Console.WriteLine("11. Personer som saknar föräldrar");
                Console.WriteLine("12. Exit");
                Console.Write("> ");
                int input = Convert.ToInt32(Console.ReadLine());

                switch (input)
                {
                    case 1:
                        Console.WriteLine("Skriv in namnet på personen:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        person = crud.Read(userInput);
                        crud.PrintAllInfo(person);
                        Console.ReadLine();
                        break;

                    case 2:
                        Console.WriteLine("Skriv in en bokstav som du vill utgå sökningen ifrån:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        ListOfPersons = crud.ListByFirstLetter(userInput);
                        GetPersonsFromList(ListOfPersons);
                        Console.ReadLine();
                        break;

                    case 3:
                        Console.WriteLine("Skriv in ett årtal och se vilka personer som är födda då:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        ListOfPersons = crud.ListByDateOfBirth(userInput);
                        GetPersonsFromList(ListOfPersons);
                        Console.ReadLine();
                        break;

                    case 4:
                        Console.WriteLine("Skriv in namnet på fadern:");
                        Console.Write("> ");
                        string father = Console.ReadLine();
                        Console.WriteLine("Skriv in namnet på modern:");
                        Console.Write("> ");
                        string mother = Console.ReadLine();
                        ListOfPersons = crud.ListByParents(mother, father);
                        GetPersonsFromList(ListOfPersons);
                        Console.ReadLine();
                        break;

                    case 5:
                        Console.WriteLine("Skriv in namnet på moder:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        ListOfPersons = crud.ListByMother(userInput);
                        GetPersonsFromList(ListOfPersons);
                        Console.ReadLine();
                        break;

                    case 6:
                        Console.WriteLine("Skriv in namnet på fadern du vill lista barnen för:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        ListOfPersons = crud.ListByFather(userInput);
                        GetPersonsFromList(ListOfPersons);
                        Console.ReadLine();
                        break;

                    case 7:
                        Console.WriteLine("Skriv in namnet på en person och se vems dennes syskon är:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        person = crud.Read(userInput);
                        ListOfPersons = crud.ListSiblings(person);
                        GetPersonsFromList(ListOfPersons);
                        Console.ReadLine();
                        break;

                    case 8:
                        Console.WriteLine("Skriv in namnet på personen du vill se dennes mor-och farföräldrar:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        person = crud.Read(userInput);
                        crud.PrintWithParents(person);
                        Console.ReadLine();
                        break;

                    case 9:
                        Console.WriteLine("Skriv in den stad som du vill utgå sökningen ifrån:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        ListOfPersons = crud.ListByCityOfBirth(userInput);
                        GetPersonsFromList(ListOfPersons);
                        Console.ReadLine();
                        break;

                    case 10:
                        ListOfPersons = crud.PeopleWithoutDateOfBirth();
                        GetPersonsFromList(ListOfPersons);
                        Console.ReadLine();
                        break;

                    case 11:
                        ListOfPersons = crud.PeopleWithoutParents();
                        GetPersonsFromList(ListOfPersons);
                        Console.ReadLine();
                        break;

                    case 12:
                        run = false;
                        break;
                }
            }
        }
    }
}