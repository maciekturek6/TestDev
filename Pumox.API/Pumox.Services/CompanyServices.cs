using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pumox.Domain;
using Pumox.Domain.Entities;
using Pumox.Domain.Enums;
using Pumox.Services.Exceptions;
using Pumox.Services.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pumox.Services
{
    public class CompanyServices : ICompanyService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<PumoxDbContext> _uow;

        public CompanyServices(IUnitOfWork<PumoxDbContext> uow, IMapper mapper)
        {
            _mapper = mapper;
            _uow = uow;
        }

        public CreatedCompanyViewModel CreateCompany(CompanyViewModel viewModel)
        {
            using (var context = _uow.DbContext)
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var company = _mapper.Map<Company>(viewModel);
                        context.Companies.Add(company);
                        var employee = new List<Employee>(viewModel.Employees
                            .Select(
                                x => new Employee()
                                {
                                    FirstName = x.FirstName,
                                    LastName = x.LastName,
                                    DateOfBirth = x.DateOfBirth,
                                    JobTitle = (JobTitle)Enum.Parse(typeof(JobTitle), x.JobTitle, true),
                                    CompanyId = company.CompanyId
                                }));
                        context.Employees.AddRange(employee);
                        context.SaveChanges();
                        transaction.Commit();
                        return _mapper.Map<CreatedCompanyViewModel>(company);
                    }
                    catch (Exception e)
                    {
                        Log.Debug("Adding new company failed");
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public CompanyViewModel UpdateCompany(CompanyViewModel viewModel, long id)
        {
            if (!_uow.DbContext.Companies.Any(p => p.CompanyId == id))
                throw new EntityNotFoundException("Company");

            var company = _mapper.Map<Company>(viewModel);
            var newEmployee = new List<Employee>(viewModel.Employees
                            .Select(
                                x => new Employee()
                                {
                                    FirstName = x.FirstName,
                                    LastName = x.LastName,
                                    DateOfBirth = x.DateOfBirth,
                                    JobTitle = (JobTitle)Enum.Parse(typeof(JobTitle), x.JobTitle, true),
                                    CompanyId = company.CompanyId
                                }));

            using (var context = _uow.DbContext)
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Companies.Update(company);

                        var currentEmployee = context.Employees.Where(x => x.CompanyId == id);
                        context.Employees.RemoveRange(currentEmployee);

                        context.Employees.AddRange(newEmployee);

                        context.SaveChanges();
                        transaction.Commit();

                        var result = context.Companies
                              .Where(c => c.CompanyId == id)
                              .Include(x => x.Employee).DefaultIfEmpty().First();           

                        return _mapper.Map<CompanyViewModel>(result);
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public IEnumerable<CompanyViewModel> SearchCompany(Expression<Func<Company, bool>> where)
        {
            var result = _uow.DbContext.Companies
                 .Include(x => x.Employee)
                 .Where(where).ToList();

            return _mapper.Map<List<CompanyViewModel>>(result);
        }

        public void DeleteCompany(long id)
        {
            if (!_uow.DbContext.Companies.Any(p => p.CompanyId == id))
                throw new EntityNotFoundException("Company");

            var company = _uow.DbContext.Companies
                .Include(x => x.Employee)
                .FirstOrDefault(c => c.CompanyId == id);

            using (var context = _uow.DbContext)
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var employee = context.Employees.Where(x => x.EmployeeId == id);
                        context.Employees.RemoveRange(employee);

                        context.Remove(company);
                        context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

        }

    }
    public interface ICompanyService
    {
        CreatedCompanyViewModel CreateCompany(CompanyViewModel viewModel);
        IEnumerable<CompanyViewModel> SearchCompany(Expression<Func<Company, bool>> where);
        CompanyViewModel UpdateCompany(CompanyViewModel viewModel, long id);
        void DeleteCompany(long id);
    }
}
