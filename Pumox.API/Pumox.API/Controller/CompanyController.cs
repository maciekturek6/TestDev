using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pumox.API.Extensions;
using Pumox.Domain.Entities;
using Pumox.Services;
using Pumox.Services.Exceptions;
using Pumox.Services.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pumox.API.Controller
{
    [Route("[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody]CompanyViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Log.Debug("Entering create new company action...");

            try
            {
                var companyId = _companyService.CreateCompany(viewModel);
                Log.Debug($"New company created with id: {companyId}");
                return Created("Create", companyId);
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message);
                throw;
            }
        }
        [HttpPost("Search")]
        [AllowAnonymous]
        public IActionResult SearchCompany([FromBody] CompanyFiltersViewModel filters)
        {
            if (!ModelState.IsValid)
                return BadRequest(filters);

            Log.Debug("Entering search company action...");

            var where = GetWhereExpression(filters);
            var result = new CompanyFiltersResultViewModel { Results = _companyService.SearchCompany(where) };

            return Ok(result);
        }
        [HttpPut("{Id}")]
        public IActionResult UpdateCompany([FromRoute]long id, [FromBody] CompanyViewModel viewmodel)
        {            
            if (!ModelState.IsValid)
                return BadRequest(viewmodel);

            Log.Debug("Entering update company action...");

            try
            {
                CompanyViewModel result = _companyService.UpdateCompany(viewmodel, id);

                if (result == null)
                    return NotFound(id);

                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                Log.Debug(ex.Message);
                return NotFound();
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message);
                throw;
            }
        }
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteCompany([FromRoute] long id)
        {
            Log.Debug("Entering delete company action...");
            try
            {
                _companyService.DeleteCompany(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                Log.Debug(ex.Message);
                return NotFound();
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message);
                throw;
            }
        }

        #region WhereExpression
        private Expression<Func<Company, bool>> GetWhereExpression(CompanyFiltersViewModel filters)
        {
            Expression<Func<Company, bool>> where = x => true;

            if (filters == null)
                return where;

            if (!string.IsNullOrWhiteSpace(filters.Keyword))
            {
                var term = filters.Keyword.ToUpper();
                where = where.And(x => (x.Name.ToUpper().Contains(term) || x.Employee.Any(y => y.FirstName.ToUpper().Contains(term)) || x.Employee.Any(y => y.LastName.ToUpper().Contains(term))));
            }

            if (filters.EmployeeDateOfBirthFrom.HasValue && filters.EmployeeDateOfBirthTo.HasValue)               
                where = where.And(x => x.Employee.Any(y => filters.EmployeeDateOfBirthFrom.Value < y.DateOfBirth && filters.EmployeeDateOfBirthTo.Value > y.DateOfBirth));

            if (filters.EmployeeJobTitles != null)
                where = where.And(x => x.Employee.Any(y => filters.EmployeeJobTitles.Contains(y.JobTitle.ToString())));

            return where;
        }
        #endregion
    }
}
