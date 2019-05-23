using System;

namespace LMS.Core.Entities
{
    public abstract class Person
    {
        public int PersonId { get; set; }
        
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
