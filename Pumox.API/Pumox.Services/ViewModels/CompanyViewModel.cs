using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pumox.Services.ViewModels
{
    public class CompanyViewModel
    {
        public long CompanyId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int EstablishmentYear { get; set; }
        public ICollection<EmployeeViewModel> Employees { get; set; }
    }
}
