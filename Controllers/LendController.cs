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
    public class LendController : Controller
    {
        private readonly ILogger<LendController> _logger;
        private readonly AppDbContext _context;

        public LendController(AppDbContext context, ILogger<LendController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public LendManagementViewModel getLendManagementEntity(PcEntity p, LendEntity l, EmployeeEntity u)
        {
            var lendManagement = new LendManagementViewModel();
            lendManagement.PcId = p.Id;
            lendManagement.Maker = p.Maker;
            lendManagement.Os = p.Os;
            lendManagement.StorageLocation = p.StorageLocation;
            lendManagement.LendId = l?.Id;
            lendManagement.EmployeeId = l?.EmployeeId;
            lendManagement.LendEndAt = l?.LendEndAt;
            lendManagement.LendStartAt = l?.LendStartAt;
            lendManagement.CreatedAt = l?.CreatedAt;
            lendManagement.UpdatedAt = l?.UpdatedAt;
            lendManagement.Remarks = l?.Remarks;
            lendManagement.FirstName = u?.FirstName;
            lendManagement.LastName = u?.LastName;
            return lendManagement;
        }


        [HttpGet]
        [Route("lend")]
        [Route("")]
        public async Task<IActionResult> Index(int pageNo = 1, int displayNum = 5, string word = "", string types = "")
        {
            var lendManagements = new List<LendManagementViewModel>();
            int lastPage = 0, lendManagementCount = 0;
            var today = DateTime.Today;
            ViewData["currentWord"] = word;
            ViewData["currentTypes"] = types;
            try
            {
                var beforeJoinEmployeeTable = _context.pcs.Where(p => !p.IsBroken && !p.IsDeleted)
                .Where(p => (word == null || word == "") || p.Id.Contains(word))
                .GroupJoin(_context.lends, p => p.Id, l => (!l.IsReturned && !l.IsDeleted) ? l.PcId : "", (p, ls) => new { p, ls })
                .SelectMany(x => x.ls.DefaultIfEmpty(), (x, l) => new { p = x.p, l })
                .Where(x =>
                (types != "all" && types != "empty" && types != "isRent" && types != "isExpired") ||
                (types == "all") ||
                (types == "empty" && x.l == null) ||
                (types == "isRent" && x.l != null && !x.l.IsReturned) ||
                (types == "isExpired" && x.l != null && x.l.LendEndAt < today));

                lendManagementCount = await beforeJoinEmployeeTable.CountAsync();

                var results = await beforeJoinEmployeeTable
                .GroupJoin(_context.employees, x => x.l.EmployeeId, u => u.Id, (x, us) => new { p = x.p, l = x.l, us })
                .SelectMany(x => x.us.DefaultIfEmpty(), (x, u) => new { p = x.p, l = x.l, u })
                .Skip(displayNum * (pageNo - 1)).Take(displayNum).ToListAsync();

                if (results != null && results.Count() > 0)
                {
                    for (var i = 0; i < results.Count(); i++)
                    {
                        // if (isSelectedTypes(types, results[i].l, today))
                        // {
                        var lendManagement = getLendManagementEntity(results[i].p, results[i].l, results[i].u);
                        lendManagements.Add(lendManagement);
                        // }
                    }
                }
            }
            catch (System.Exception e)
            {
                throw new Exception("MySql Server Error", e);
            }

            if (lendManagementCount == 0)
            {
                return View(lendManagements);
            }
            if (displayNum > 0) lastPage = (int)Math.Ceiling((double)lendManagementCount / displayNum);
            if (pageNo > lastPage || pageNo < 1) return Redirect($"/lend?displayNum={displayNum}");
            ViewData["currentPage"] = pageNo;
            ViewData["currentDisplayNum"] = displayNum;
            ViewData["lastPage"] = lastPage;

            return View(lendManagements);
        }

        [HttpGet]
        [Route("lend/detail/{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            var currentLend = new LendManagementViewModel();
            var lendHistory = new List<LendManagementViewModel>();

            var results = await _context.pcs
            .Where(p => p.Id == id)
            .GroupJoin(_context.lends, p => p.Id, l => l.PcId, (p, ls) => new { p, ls })
            .SelectMany(x => x.ls.DefaultIfEmpty(), (x, l) =>
            new { p = x.p, l })
            .GroupJoin(_context.employees, x => x.l.EmployeeId, u => u.Id, (x, us) => new { p = x.p, l = x.l, us })
            .SelectMany(x => x.us.DefaultIfEmpty(), (x, u) => new { p = x.p, l = x.l, u })
            .ToListAsync();
            if (results != null && results.Count() > 0)
            {
                for (var i = 0; i < results.Count(); i++)
                {
                    if (results[i].l != null && results[i].l.IsDeleted && currentLend.PcId == id) continue;

                    var lendManagement = getLendManagementEntity(results[i].p, results[i].l, results[i].u);

                    //機器情報が入っていない場合
                    if (currentLend.PcId != id)
                    {
                        currentLend = getLendManagementEntity(results[i].p, null, null);
                    }

                    //貸出情報が存在＆返却済み ＝＞ 履歴に追加
                    if (results[i].l != null && results[i].l.IsReturned && !results[i].l.IsDeleted) lendHistory.Add(lendManagement);

                    //貸出情報が存在しない|| 未返却 ＝＞ 現在の情報に追加
                    else if (results[i].l == null || (!results[i].l.IsReturned && !results[i].l.IsDeleted)) currentLend = lendManagement;
                }

                ViewData["history"] = lendHistory;
                return View(currentLend);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("lend/lend-return")]
        public async Task<IActionResult> Edit()
        {
            try
            {
                var availablePcs = new List<LendManagementViewModel>();
                var currentUsedPcs = new Dictionary<string, LendManagementViewModel>();
                var availableEmployees = await _context.employees.Where(u => !u.IsDeleted && u.RetiredAt == null).ToListAsync();

                var results = await _context.pcs
                .Where(p => !p.IsBroken && !p.IsDeleted)
                .GroupJoin(_context.lends, p => p.Id, l => (!l.IsDeleted && !l.IsReturned) ? l.PcId : "", (p, ls) => new { p, ls })
                .SelectMany(x => x.ls.DefaultIfEmpty(), (x, l) => new { p = x.p, l })
                .GroupJoin(_context.employees, x => x.l.EmployeeId, u => u.Id, (x, us) => new { p = x.p, l = x.l, us })
                .SelectMany(x => x.us.DefaultIfEmpty(), (x, u) => new { p = x.p, l = x.l, u })
                //lend情報ない||未削除＆未返却
                .Where(x => x.l == null || (!x.l.IsDeleted && !x.l.IsReturned)).ToListAsync();
                if (results != null && results.Count() > 0)
                {
                    for (var i = 0; i < results.Count(); i++)
                    {
                        var lendManagement = getLendManagementEntity(results[i].p, results[i].l, results[i].u);
                        if (results[i].l != null) currentUsedPcs.Add(lendManagement.PcId, lendManagement);
                        else availablePcs.Add(lendManagement);
                    }
                }

                ViewData["availablePcs"] = availablePcs;
                ViewData["currentUsedPcs"] = currentUsedPcs;
                ViewData["availableEmployees"] = availableEmployees;
                return View();
            }
            catch (System.Exception)
            {
                throw new Exception("データを取得できませんでした");
            }
        }


        [HttpGet]
        [Route("lend/lend-return/{id}")]
        public IActionResult Edit(string id)
        {
            var lend = new LendEntity();
            ViewData["PathId"] = "add";
            ViewData["Title"] = "社員新規追加";
            return View(lend);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("lend/create")]
        public async Task<IActionResult> Create(LendEntity lend)
        {
            var nowDatetime = DateTime.Now;
            lend.CreatedAt = nowDatetime;
            lend.UpdatedAt = nowDatetime;
            lend.IsDeleted = false;
            lend.IsReturned = false;

            _context.lends.Add(lend);
            await _context.SaveChangesAsync();
            return Redirect("/lend");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("lend/update")]
        public async Task<IActionResult> Update(LendEntity lend)
        {
            lend.UpdatedAt = DateTime.Now;
            lend.IsDeleted = false;
            lend.IsReturned = false;
            _context.Entry(lend).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Redirect($"/lend/detail/{lend.PcId}");
        }

        [Route("lend/return")]
        public async Task<IActionResult> Return(LendEntity lend)
        {
            if (lend != null)
            {
                lend.UpdatedAt = DateTime.Now;
                lend.IsDeleted = false;
                lend.IsReturned = true;
                _context.Entry(lend).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Redirect("/lend");
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
