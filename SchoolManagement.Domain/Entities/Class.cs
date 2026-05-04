using SchoolManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace SchoolManagement.Domain.Entities
{
    public class Class : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public Guid SchoolId { get; set; }
        public School School { get; set; } = null!;

        public ICollection<Section> Sections { get; set; } = new List<Section>();
        public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    }
}
