using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pumox.Services.ViewModels
{
    public class EmployeeViewModel
    {
        public long EmployeeId { get; set; }

        [Required]
        public long CompanyId { get; set; }
        public virtual CompanyViewModel CompanyModel { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string JobTitle { get; set; }
    }
}
