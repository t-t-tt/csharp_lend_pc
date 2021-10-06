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
    public class PcController : Controller
    {
        private readonly ILogger<PcController> _logger;
        private readonly AppDbContext _context;
        public PcController(AppDbContext context, ILogger<PcController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet]
        [Route("pc")]
        public async Task<IActionResult> Index(int pageNo = 1, int displayNum = 5, string word = "", string types = "")
        {
            var pcs = new List<PcEntity>();
            int lastPage = 0, pcCount = 0;
            var today = DateTime.Today;
            ViewData["currentWord"] = word;
            ViewData["currentTypes"] = types;

            try
            {
                var beforeFilter = _context.pcs.Where(p => !p.IsDeleted)
                .Where(p => (word == null || word == "") || p.Id.Contains(word))
                .Where(p =>
                (types != "all" && types != "isBroken" && types != "notBroken") ||
                (types == "all") ||
                (types == "isBroken" && p.IsBroken) ||
                (types == "notBroken" && !p.IsBroken));

                pcCount = await beforeFilter.CountAsync();
                pcs = await beforeFilter.Skip(displayNum * (pageNo - 1)).Take(displayNum).ToListAsync();
            }
            catch (System.Exception e)
            {
                throw new Exception("MySql Server Error", e);
            }

            if (pcCount == 0)
            {
                return View(pcs);
            }
            if (displayNum > 0) lastPage = (int)Math.Ceiling((double)pcCount / displayNum);
            if (pageNo > lastPage || pageNo < 1) return Redirect($"/pc?displayNum={displayNum}");
            ViewData["currentPage"] = pageNo;
            ViewData["currentDisplayNum"] = displayNum;            
            ViewData["lastPage"] = lastPage;

            return View(pcs);
        }

        [HttpGet]
        [Route("pc/detail/{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            var pc = await _context.pcs.FindAsync(id);

            if (pc != null)
            {
                return View(pc);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("pc/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id != null && id != "")
            {
                var pc = await _context.pcs.FindAsync(id);
                if (pc != null)
                {
                    ViewData["pathId"] = "edit";
                    ViewData["Title"] = "機器情報編集";
                    return View(pc);
                }
                else
                {
                    NotFound();
                }
            }

            return Error();
        }

        [HttpGet]
        [Route("pc/add")]
        public IActionResult Edit()
        {
            var pc = new PcEntity();
            pc.LeaseStartAt = DateTime.Now;
            pc.LeaseEndAt = DateTime.Now;
            ViewData["PathId"] = "add";
            ViewData["Title"] = "機器新規追加";
            return View(pc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("pc/create")]
        public async Task<IActionResult> Create(PcEntity pc)
        {
            var nowDatetime = DateTime.Now;
            pc.CreatedAt = nowDatetime;
            pc.UpdatedAt = nowDatetime;
            pc.IsDeleted = false;

            _context.pcs.Add(pc);
            await _context.SaveChangesAsync();
            return Redirect("/pc");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("pc/update/{Id}")]
        public async Task<IActionResult> Update(PcEntity pc)
        {
            pc.UpdatedAt = DateTime.Now;
            _context.Entry(pc).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Redirect($"/pc/detail/{pc.Id}");
        }

        [Route("pc/delete/{id}")]
        public async Task<IActionResult> Delete(String id)
        {
            var pc = await _context.pcs.FindAsync(id);
            if (pc != null)
            {
                pc.UpdatedAt = DateTime.Now;
                pc.IsDeleted = true;
                _context.Entry(pc).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Redirect("/pc");
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
