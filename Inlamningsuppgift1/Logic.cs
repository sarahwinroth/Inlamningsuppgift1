using System;
using System.Collections.Generic;

namespace Inlamningsuppgift1
{
    public static class Logic
    {
        public static Crud crud = new Crud();
        public static List<Person> listOfPeople;
        internal static Database db = new Database();

        public static Crud Crud
        {
            get => default;
            set
            {
            }
        }

        public static Person Person
        {
            get => default;
            set
            {
            }
        }

        /// <summary>
        /// Lägger till en person i databasen, men kollar först om personen existerar genom att söka på personens för- och efternamn samt mor och far.
        /// Om personen redan existerar så meddelas det samt ger möjligheten för användaren att redigera personen.
        /// </summary>
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
            Console.WriteLine("Ange far (För- och efternamn):");
            Console.Write("> ");
            string father = Console.ReadLine();
            int fatherId = Convert.ToInt32(crud.GetParentId(father));
            Console.WriteLine("Ange mor (För- och efternamn):");
            Console.Write("> ");
            string mother = Console.ReadLine();
            int motherId = Convert.ToInt32(crud.GetParentId(mother));

            if (!crud.CheckIfPersonExist(firstName, lastName, fatherId, motherId))
            {
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
                string birthplace = Console.ReadLine();
                Console.WriteLine("Dödsdatum (YYYY):");
                Console.Write("> ");
                string yearOfDeath = Console.ReadLine().Trim();
                int dateOfDeath;
                if (yearOfDeath == "")
                { dateOfDeath = 0; }
                else
                { dateOfDeath = Convert.ToInt32(yearOfDeath); }              
                Console.WriteLine("Dödsstad:");
                Console.Write("> ");
                string deathplace = Console.ReadLine();

                var person = new Person
                {
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dateOfBirth,
                    Birthplace = birthplace,
                    DateOfDeath = dateOfDeath,
                    Deathplace = deathplace,
                    FatherId = fatherId,
                    MotherId = motherId,
                };
                crud.Create(person);
                Console.WriteLine("\nEn ny person är tillagd i familjeträdet!");
                crud.PrintAllInfo(person);
                ContinueOrQuitAddPerson();
            }
            else
            {
                Console.WriteLine("\nPersonen existerar redan!");
                var person = crud.Read(firstName, lastName);
                crud.Print(person);
                Console.WriteLine("\nVill du redigera personen? [J] / [N]");
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

        /// <summary>
        /// Ger användaren möjligheten att återigen skapa en person eller gå tillbaka till Menyn.
        /// </summary>
        public static void ContinueOrQuitAddPerson()
        {
            Console.WriteLine("\nVill du lägga till en ny person? [J] / [N]");
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

        /// <summary>
        /// Ger användaren möjligheten att fortsätta redigera pågående person eller redigera ny person eller gå tillbaka till Menyn.
        /// </summary>
        public static void ContinueOrQuitEditPerson(Person person)
        {
            Console.WriteLine($"\nVill du fortsätta redigera uppgifter om {person.FirstName} {person.LastName}? [J] / [N]");
            Console.WriteLine($"Om du vill redigera uppgifter om en annan person. [R]");
            Console.Write("> ");
            string input = Console.ReadLine();
            if (input.ToLower().Replace(" ", "") == "n")
            {
                Menu();
            }
            if (input.ToLower().Replace(" ", "") == "r")
            {
                GetPersonToEdit();
            }
            else
            {
                EditPerson(person);
            }
        }

        /// <summary>
        /// Ger användaren möjligheten att fortsätta redigera en ny person eller gå tillbaka till Menyn.
        /// </summary>
        public static void ContinueOrQuitEditPerson()
        {
            Console.WriteLine("\nVill du fortsätta redigera? [J] / [N]");
            Console.Write("> ");
            string input = Console.ReadLine();
            if (input.ToLower().Replace(" ", "") == "n")
            {
                Menu();
            }
            else
            {
                GetPersonToEdit();
            }
        }

        /// <summary>
        /// Ger användaren möjligheten att fortsätta söka i databasen eller gå tillbaka till Menyn.
        /// </summary>
        public static void ContinueOrQuitSearchInFamily()
        {
            Console.WriteLine("\nVill du fortsätta att söka? [J] / [N]");
            Console.Write("> ");
            string input = Console.ReadLine();
            if (input.ToLower().Replace(" ", "") == "n")
            {
                Menu();
            }
            else
            {
                SearchInFamily();
            }
        }

        /// <summary>
        /// En redigerings meny med alternativ så som att ändra uppgifter om personen eller ta bort personen.
        /// Väljer man ändra så får användaren möjlighet att ändra uppgifter om personen genom att skriva in vilken uppgift som skall ändras.
        /// När användaren matat in det nya värdet så updateras personen i databasen.
        /// Väljer användaren att ta bort person så tas personen bort från databasen.
        /// </summary>
        /// <param name="person">Person objektet som kommer att redigeras</param>
        public static void EditPerson(Person person)
        {
            bool run = true;

            while (run)
            {
                Console.Clear();
                Console.WriteLine($"\n1. Ändra uppgifter om {person.FirstName} {person.LastName}");
                Console.WriteLine($"2. Ta bort familjemedlem {person.FirstName} {person.LastName}");
                Console.WriteLine("3. Exit");
                Console.Write("> ");
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
                                ContinueOrQuitEditPerson(person);
                                break;

                            case "efternamn":
                                Console.WriteLine("Skriv in det nya efternamnet:");
                                Console.Write("> ");
                                string newLastName = Console.ReadLine();
                                person.LastName = newLastName;
                                crud.Update(person);
                                ContinueOrQuitEditPerson(person);
                                break;

                            case "födelsedatum":
                                Console.WriteLine("Skriv in det nya födelsedatumet:");
                                Console.Write("> ");
                                int newDateOfBirth = Convert.ToInt32(Console.ReadLine());
                                person.DateOfBirth = newDateOfBirth;
                                crud.Update(person);
                                ContinueOrQuitEditPerson(person);
                                break;

                            case "födelsestad":
                                Console.WriteLine("Skriv in den nya födelsestaden:");
                                Console.Write("> ");
                                string newBirthplace = Console.ReadLine();
                                person.Birthplace = newBirthplace;
                                crud.Update(person);
                                ContinueOrQuitEditPerson(person);
                                break;
                            case "dödsdatum":
                                Console.WriteLine("Skriv in det nya dödsdatumet:");
                                Console.Write("> ");
                                int newDateOfDeath = Convert.ToInt32(Console.ReadLine());
                                person.DateOfDeath = newDateOfDeath;
                                crud.Update(person);
                                ContinueOrQuitEditPerson(person);
                                break;
                            case "dödsstad":
                                Console.WriteLine("Skriv in den nya dödsstaden:");
                                Console.Write("> ");
                                string newDeathplace = Console.ReadLine();
                                person.Deathplace = newDeathplace;
                                crud.Update(person);
                                ContinueOrQuitEditPerson(person);
                                break;

                            case "far":
                                Console.WriteLine("Skriv in det nya namnet på fadern:");
                                Console.Write("> ");
                                string newFather = Console.ReadLine();
                                int newFatherId = Convert.ToInt32(crud.GetParentId(newFather));
                                person.FatherId = newFatherId;
                                crud.Update(person);
                                ContinueOrQuitEditPerson(person);
                                break;

                            case "mor":
                                Console.WriteLine("Skriv in det nya namnet på mordern");
                                Console.Write("> ");
                                string newMother = Console.ReadLine();
                                int newMotherId = Convert.ToInt32(crud.GetParentId(newMother));
                                person.MotherId = newMotherId;
                                crud.Update(person);
                                ContinueOrQuitEditPerson(person);
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

        /// <summary>
        /// Hämtar personen som användaren vill redigera. Låt användaren mata in namnet på den person som skall redigeras.
        /// Metoden kollar om personen existerar om ja så fortsätter metoden att kolla om det är fler än en person med samma namn.
        /// Om det är fler än en person med samma namn så visas Id-numret för varje person och användaren väljer person genom att skriva in Id-numret för den personen användaren vill redigera.
        /// När metoden har en person som matchar inmatningen så anropas metoden EditPerson().
        /// </summary>
        public static void GetPersonToEdit()
        {
            bool run = true;
            while (run)
            {
                Person person;
                Console.Clear();
                Console.WriteLine("Vänligen skriv in namnet på personen du vill redigera:");
                Console.Write("> ");
                string name = Console.ReadLine();
                person = crud.Read(name);
                if (crud.CheckIfPersonExist(person))
                {
                    if (crud.IfReadingManyPeople(name))
                    {
                        Console.WriteLine("\nDet finns fler personer med samma namn!");
                        listOfPeople = crud.ListByName(name);
                        crud.GetPeopleFromListWithId(listOfPeople);
                        Console.WriteLine("\nVänligen skriv in Id-numret för den personen du vill redigera:");
                        Console.Write("> ");
                        int id = Convert.ToInt32(Console.ReadLine());
                        person = crud.Read(id);
                        crud.PrintAllInfo(person);
                        Console.WriteLine("\n[TRYCK ENTER FÖR ATT REDIGERA]");
                    }
                    else
                    {
                        crud.PrintAllInfo(person);
                        Console.WriteLine("\n[TRYCK ENTER FÖR ATT REDIGERA]");
                    }
                    EditPerson(person);
                }
                else
                {
                    string choice = PersonDontExist();
                    if (choice.ToLower().Replace(" ", "") == "n")
                    {
                        GetPersonToEdit();
                    }
                    else
                    {
                        AddPerson();
                    }
                }
            }
        }

        /// <summary>
        /// Huvudmenyn där programmet utgår ifrån. Användaren har valmöjligheter att Skapa person, Redigera person, Söka i familjeträdet och avsluta programmet.
        /// </summary>
        public static void Menu()
        {
            try
            {
                bool runProjekt = true;
                while (runProjekt)
                {
                    Console.Clear();
                    Console.WriteLine("[VÄLKOMMEN TILL FAMLJETRÄDET]");
                    Console.WriteLine("\n1. Skapa en familjemedlem");
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
                            GetPersonToEdit();
                            break;

                        case 3:
                            SearchInFamily();
                            break;

                        case 4:
                            Console.WriteLine("In case I don't see you, good afternoon, good evening and good night! - Truman Burbank");
                            Console.ReadLine();
                            runProjekt = false;
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

        /// <summary>
        /// Meddelar användaren att personen inte existerar och ger möjligheten att lägga till en person eller avsluta.
        /// Genom att mata in bokstaven J eller N.
        /// </summary>
        /// /// <returns>String - Användarens val</returns>
        public static string PersonDontExist()
        {
            Console.WriteLine("Personen existerar inte!");
            Console.WriteLine("\nVill du lägga till en familjemedlem? J/N");
            Console.Write("> ");
            string choice = Console.ReadLine();
            return choice;
        }

        /// <summary>
        /// Meny med olika val att söka på i Familj-tabellen där varje val hämtar data från databasen och visar för användaren.
        /// </summary>
        public static void SearchInFamily()
        {
            bool run = true;
            while (run)
            {
                Person person;
                string userInput;
                Console.Clear();
                Console.WriteLine("[SÖK EFTER..]");
                Console.WriteLine("1. En specifik person i familjeträdet");
                Console.WriteLine("2. Personer som börjar på en specifik bokstav");
                Console.WriteLine("3. Personer som är födda ett visst år");
                Console.WriteLine("4. Personer med samma föräldrar");
                Console.WriteLine("5. Syskon med samma mor");
                Console.WriteLine("6. Syskon med samma far");
                Console.WriteLine("7. Syskon till en person");
                Console.WriteLine("8. Föräldrar till en person");
                Console.WriteLine("9. Personer som är födda i samma stad");
                Console.WriteLine("10. Mor- och farföräldrar till en person");
                Console.WriteLine("11. Personer som saknar födelsedatum");
                Console.WriteLine("12. Personer som saknar föräldrar");
                Console.WriteLine("13. Exit");
                Console.Write("> ");
                int input = Convert.ToInt32(Console.ReadLine());

                switch (input)
                {
                    case 1:
                        Console.WriteLine("Skriv in namnet på personen:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        person = crud.Read(userInput);
                        if (crud.CheckIfPersonExist(person))
                        {
                            if (crud.IfReadingManyPeople(userInput))
                            {
                                listOfPeople = crud.ListByName(userInput);
                                crud.GetPeopleFromList(listOfPeople);
                            }
                            else
                            {
                                crud.PrintAllInfo(person);
                            }
                            ContinueOrQuitSearchInFamily();
                        }
                        else
                        {
                            string choice = PersonDontExist();
                            if (choice.ToLower().Replace(" ", "") == "n")
                            {
                                SearchInFamily();
                            }
                            else
                            {
                                AddPerson();
                            }
                        }
                        break;

                    case 2:
                        Console.WriteLine("Skriv in en bokstav som du vill utgå sökningen ifrån:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        listOfPeople = crud.ListByFirstLetter(userInput);
                        crud.GetPeopleFromList(listOfPeople);
                        ContinueOrQuitSearchInFamily();
                        break;

                    case 3:
                        Console.WriteLine("Skriv in ett årtal och se vilka personer som är födda då:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        listOfPeople = crud.ListByDateOfBirth(userInput);
                        crud.GetPeopleFromList(listOfPeople);
                        ContinueOrQuitSearchInFamily();
                        break;

                    case 4:
                        Console.WriteLine("Skriv in namnet på fadern:");
                        Console.Write("> ");
                        string father = Console.ReadLine();
                        Console.WriteLine("Skriv in namnet på modern:");
                        Console.Write("> ");
                        string mother = Console.ReadLine();
                        listOfPeople = crud.ListByParents(mother, father);
                        crud.GetPeopleFromList(listOfPeople);
                        ContinueOrQuitSearchInFamily();
                        break;

                    case 5:
                        Console.WriteLine("Skriv in namnet på modern:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        listOfPeople = crud.ListByMother(userInput);
                        crud.GetPeopleFromList(listOfPeople);
                        ContinueOrQuitSearchInFamily();
                        break;

                    case 6:
                        Console.WriteLine("Skriv in namnet på fadern du vill lista barnen för:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        listOfPeople = crud.ListByFather(userInput);
                        crud.GetPeopleFromList(listOfPeople);
                        ContinueOrQuitSearchInFamily();
                        break;

                    case 7:
                        Console.WriteLine("Skriv in namnet på en person och se syskonen:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        person = crud.Read(userInput);
                        listOfPeople = crud.ListSiblings(person);
                        crud.GetPeopleFromList(listOfPeople);
                        ContinueOrQuitSearchInFamily();
                        break;

                    case 8:
                        Console.WriteLine("Skriv in namnet på personen du vill se dennes mor-och farföräldrar:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        person = crud.Read(userInput);
                        if (crud.CheckIfPersonExist(person))
                        {
                            crud.PrintWithParents(person);
                            ContinueOrQuitSearchInFamily();
                        }
                        else
                        {
                            string choice = PersonDontExist();
                            if (choice.ToLower().Replace(" ", "") == "n")
                            {
                                SearchInFamily();
                            }
                            else
                            {
                                AddPerson();
                            }
                        }                     
                        break;

                    case 9:
                        Console.WriteLine("Skriv in den stad som du vill utgå sökningen ifrån:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        listOfPeople = crud.ListByBirthplace(userInput);
                        crud.GetPeopleFromList(listOfPeople);
                        ContinueOrQuitSearchInFamily();
                        break;

                    case 10:
                        Console.WriteLine("Skriv in namnet på personen:");
                        Console.Write("> ");
                        userInput = Console.ReadLine();
                        person = crud.Read(userInput);
                        if (crud.CheckIfPersonExist(person))
                        { 
                            crud.GetThreeGenerations(person); 
                            ContinueOrQuitSearchInFamily(); 
                        }
                        else 
                        { 
                            string choice = PersonDontExist();
                            if (choice.ToLower().Replace(" ", "") == "n")
                            {
                                SearchInFamily();
                            }
                            else
                            {
                                AddPerson();
                            }
                        }
                        break;

                    case 11:
                        listOfPeople = crud.PeopleWithoutDateOfBirth();
                        crud.GetPeopleFromList(listOfPeople);
                        ContinueOrQuitSearchInFamily();
                        break;

                    case 12:
                        listOfPeople = crud.PeopleWithoutParents();
                        crud.GetPeopleFromList(listOfPeople);
                        ContinueOrQuitSearchInFamily();
                        break;

                    case 13:
                        run = false;
                        Menu();
                        break;
                }
            }
        }

        /// <summary>
        /// Metoden som anropas när programmet startar. Kollar om databas existerar, om Ja anger den databasnamn till variabeln databaseName.
        /// Om databasen inte existerar så skapas en utifrån Master, tabell skapas och data läggs in i tabellen.
        /// Sedan anropas Menu-metoden.
        /// </summary>
        public static void Start()
        {
            if (!db.CheckIfDatabaseExist("Genealogy"))
            {
                db.DatabaseName = "Master";
                db.CreateDB("Genealogy");
                db.DatabaseName = "Genealogy";
                db.AddDatabaseData();
            }
            else { db.DatabaseName = "Genealogy"; }
            Menu();
        }
    }
}