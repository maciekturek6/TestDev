using AutoMapper;
using Pumox.Domain.Entities;
using Pumox.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pumox.Services.Mapping
{
    public class EmployeeMapping : Profile
    {
        public EmployeeMapping()
        {
            CreateMap<List<EmployeeViewModel>, List<Employee>>().ReverseMap();
        }        
    }
}
