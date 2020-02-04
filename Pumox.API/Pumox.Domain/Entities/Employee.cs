using Pumox.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pumox.Domain.Entities
{
    public class Employee 
    {
        public long EmployeeId { get; set; }
        public long CompanyId { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [EnumDataType(typeof(JobTitle))]
        public JobTitle JobTitle { get; set; }

        public virtual Company Company { get; set; }
    }
}
