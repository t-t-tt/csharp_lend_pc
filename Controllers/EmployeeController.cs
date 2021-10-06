using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using csharp_lend_pc.Models;
using Microsoft.EntityFrameworkCore;
using csharp_lend_pc.Db;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace csharp_lend_pc.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly AppDbContext _context;
        public EmployeeController(AppDbContext context, ILogger<EmployeeController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet]
        [Route("employee")]
        public async Task<IActionResult> Index(int pageNo = 1, int displayNum = 5, string word = "", string types = "")
        {
            var employees = new List<EmployeeEntity>();
            int lastPage = 0, employeeCount = 0;
            var today = DateTime.Today;
            ViewData["currentWord"] = word;
            ViewData["currentTypes"] = types;

            try
            {
                var beforeFilter = _context.employees.Where(u => !u.IsDeleted)
                .Where(u => (word == null || word == "") || (u.Id.Contains(word) || u.FirstName.Contains(word) || u.FirstNameKana.Contains(word) || u.LastName.Contains(word) || u.LastNameKana.Contains(word)))
                .Where(u =>
                (types != "all" && types != "isRetired" && types != "notRetired") ||
                (types == "all") ||
                (types == "isRetired" && u.RetiredAt != null) ||
                (types == "notRetired" && u.RetiredAt == null));

                employeeCount = await beforeFilter.CountAsync();
                employees = await beforeFilter.Skip(displayNum * (pageNo - 1)).Take(displayNum).ToListAsync();
            }
            catch (System.Exception e)
            {
                throw new Exception("MySql Server Error", e);
            }

            if (employeeCount == 0)
            {
                return View(employees);
            }
            if (displayNum > 0) lastPage = (int)Math.Ceiling((double)employeeCount / displayNum);
            if (pageNo > lastPage || pageNo < 1) return Redirect($"/employee?displayNum={displayNum}");
            ViewData["currentPage"] = pageNo;
            ViewData["currentDisplayNum"] = displayNum;
            ViewData["lastPage"] = lastPage;

            return View(employees);
        }

        [HttpGet]
        [Route("employee/detail/{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            var employee = await _context.employees.FindAsync(id);

            if (employee != null)
            {
                return View(employee);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("employee/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id != null && id != "")
            {
                var employee = await _context.employees.FindAsync(id);
                if (employee != null)
                {
                    ViewData["pathId"] = "edit";
                    ViewData["Title"] = "社員情報編集";
                    return View(employee);
                }
                else
                {
                    NotFound();
                }
            }

            return Error();
        }

        [HttpGet]
        [Route("employee/add")]
        public IActionResult Edit()
        {
            var employee = new EmployeeEntity();
            ViewData["PathId"] = "add";
            ViewData["Title"] = "社員新規追加";
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("employee/create")]
        public async Task<IActionResult> Create(EmployeeEntity employee)
        {
            var nowDatetime = DateTime.Now;
            employee.CreatedAt = nowDatetime;
            employee.UpdatedAt = nowDatetime;
            employee.IsDeleted = false;

            _context.employees.Add(employee);
            await _context.SaveChangesAsync();
            return Redirect("/employee");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("employee/update/{Id}")]
        public async Task<IActionResult> Update(EmployeeEntity employee)
        {
            employee.UpdatedAt = DateTime.Now;
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Redirect($"/employee/detail/{employee.Id}");
        }

        [Route("employee/delete/{id}")]
        public async Task<IActionResult> Delete(String id)
        {
            var employee = await _context.employees.FindAsync(id);
            if (employee != null)
            {
                employee.UpdatedAt = DateTime.Now;
                employee.IsDeleted = true;
                _context.Entry(employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Redirect("/employee");
            }
            return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
