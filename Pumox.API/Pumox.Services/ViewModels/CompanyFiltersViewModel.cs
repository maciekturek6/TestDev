using System;
using System.Collections.Generic;
using System.Text;

namespace Pumox.Services.ViewModels
{
    public class CompanyFiltersViewModel
    {
        public string Keyword { get; set; }
        public DateTime? EmployeeDateOfBirthFrom { get; set; }
        public DateTime? EmployeeDateOfBirthTo { get; set; }
        public List<string> EmployeeJobTitles { get; set; }
    }
}
