using Pumox.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pumox.Domain.Entities
{
    public class Company 
    {
        public long CompanyId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int EstablishmentYear { get; protected set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
