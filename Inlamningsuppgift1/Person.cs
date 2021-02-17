using System;
using System.Collections.Generic;
using System.Text;

namespace Inlamningsuppgift1
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DateOfBirth { get; set; }
        public string CityOfBirth { get; set; }
        public string CityOfDeath { get; set; }
        public int FatherId { get; set; }
        public int MotherId { get; set; }
    }
}
