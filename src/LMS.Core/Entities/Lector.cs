using System;

using LMS.Core.Interfaces;

namespace LMS.Core.Entities
{
    public class Lector : Person, IEntity
    {
        public int Id { get; set; }
    }
}
