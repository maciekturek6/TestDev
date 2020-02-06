using AutoMapper;
using Pumox.Domain.Entities;
using Pumox.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pumox.Services.Mapping
{
    public class CompanyMapping : Profile
    {
        public CompanyMapping()
        {
            CreateMap<CompanyViewModel, Company>()
               .ForMember(x => x.Name, x => x.MapFrom(c => c.Name))
               .ForMember(x => x.EstablishmentYear, x => x.MapFrom(c => c.EstablishmentYear));

            CreateMap<Company, CreatedCompanyViewModel>()
                .ForMember(x => x.CompanyId, x => x.MapFrom(c => c.CompanyId));

            CreateMap<Company, CompanyViewModel>()
                .ForMember(x => x.Employees, x => x.MapFrom(c => c.Employee.Select(s => new EmployeeViewModel() {EmployeeId = s.EmployeeId, CompanyId = s.CompanyId, FirstName = s.FirstName, LastName = s.LastName, DateOfBirth = s.DateOfBirth, JobTitle = s.JobTitle.ToString()}).ToList()))
                .ReverseMap();
        }
    }
}
