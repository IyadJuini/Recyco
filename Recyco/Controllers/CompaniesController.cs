using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectRecycle.Models;
using System.ComponentModel.Design;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ProjectRecycle.Controllers
{
    //[Area("Company")]
    public class CompaniesController : Controller
    {
        private readonly MyContext _context;
        private int companyId;

        public CompaniesController(MyContext context)
        {
            _context = context;
        }
        


        // GET: Companies/Details/5
        [HttpGet("Companies/Details/{companyId}")]
        public async Task<IActionResult> Details(int? companyId)
        {

            if (HttpContext.Session.GetInt32("companyId") == null)
            {
                return RedirectToAction("ErrorPage", "AppUsers");
            }
            if (companyId == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.CompanyId == companyId);
            if (company == null)
            {
                return NotFound();
            }
            ViewBag.ShowFooter = false;
            return View(company);
        }
        // GET: Companies/Dashboard/5
        [HttpGet("Companies/Dashboard/{id}")]

        public async Task<IActionResult> Dashboard(int? id)
        {
            if (HttpContext.Session.GetInt32("companyId") == null)
            {
                return RedirectToAction("ErrorPage", "AppUsers");
            }
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }
            var modelWaste = _context.Wastes;
            var company = await _context.Companies
        .Include(c => c.CompanyWastes).ThenInclude(c=> c.Offers)
        .Include(c => c.Bids) 
        .FirstOrDefaultAsync(m => m.CompanyId == id);

            if (company == null)
            {
                return NotFound();
            }
            

            int numberOfWastes = company.CompanyWastes.Count;
            int numberOfBids = company.Bids.Count;
            int numberOfOffers = company.CompanyWastes.SelectMany(waste => waste.Offers).Count();
            

            ViewBag.NumberOfWastes = numberOfWastes;
            ViewBag.NumberOfBids = numberOfBids;
            ViewBag.NumberOfOffers = numberOfOffers;

            ViewBag.ShowFooter = false;
            return View(company);
        }
       
        // GET: Companies/Wastes/5
        [HttpGet("Companies/Wastes/{companyId}")]

        public async Task<IActionResult> Wastes(int? companyId)
        {
            if (HttpContext.Session.GetInt32("companyId") == null)
            {
                return RedirectToAction("ErrorPage", "AppUsers");
            }
            if (companyId == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
        .Include(c => c.CompanyWastes) 
        .Include(c => c.Bids) 
        .FirstOrDefaultAsync(m => m.CompanyId == companyId);

            if (company == null)
            {
                return NotFound();
            }
            ViewBag.ShowFooter = false;
            return View(company);
        }// GET: Companies/Bids/5
        [HttpGet("Companies/Bids/{companyId}")]

        public async Task<IActionResult> Bids(int? companyId)
        {
            if (HttpContext.Session.GetInt32("companyId") == null)
            {
                return RedirectToAction("ErrorPage", "AppUsers");
            }
            if (companyId == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(c => c.Bids)
                .FirstOrDefaultAsync(m => m.CompanyId == companyId);

            if (company == null)
            {
                return NotFound();
            }



            var orderedBids = company.Bids.OrderByDescending(b => b.CreatedAt).DistinctBy(bid => bid.OfferId).ToList();
            foreach (var item in orderedBids)
            {
                var offer = _context.Offers.Include(o=>o.Waste).FirstOrDefaultAsync(o => o.OfferId == item.OfferId).Result;
                if (offer!=null) item.Offer = offer ;
            }

            company.Bids = orderedBids;
            //company.Bids = company.Bids.DistinctBy(bid => bid.OfferId).ToList();
            ViewBag.ShowFooter = false;
            return View(company);
        }

        // GET: Companies/Create
        [HttpGet("Companies/Create")]
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult Register(Company newCompany, IFormFile Logo)
        {
            if (ModelState.IsValid)
            {
                // Email Exist ?
                if (_context.Companies.Any(u => u.Email == newCompany.Email))
                {
                    // True
                    ModelState.AddModelError("Email", "Email already in use .");
                    return View("Index");
                }
                else
                {
                    // False
                    // 1 - Hash Password
                    PasswordHasher<Company> Hasher = new PasswordHasher<Company>();
                    newCompany.Password = Hasher.HashPassword(newCompany, newCompany.Password);

                    //upload file
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Logo.FileName);
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    string filepath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        Logo.CopyTo(stream);
                    }

                    newCompany.Logo = fileName;

                    // 2 - Add 
                    _context.Add(newCompany);
                    // 3 - Save
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("companyId", newCompany.CompanyId);
                    HttpContext.Session.SetString("companyName", newCompany.Name);
                    // HttpContext.Session.
                    return RedirectToAction("Details", new { id = newCompany.CompanyId });
                }
            }
            return View("Create");
        }



        public IActionResult Privacy()
        {
            if (HttpContext.Session.GetInt32("companyId") == null)
            {
                return RedirectToAction("Index");
            }
            int? CompanyId = (int)HttpContext.Session.GetInt32("companyId");
            Company? company = _context.Companies.FirstOrDefault(u => u.CompanyId == companyId);
            return View(company);
        }
        

       

        

        // GET: Companies/Edit/5
        [HttpGet("Companies/Edit/{companyId}")]
        public async Task<IActionResult> Edit(int? companyId)
        {
            if (HttpContext.Session.GetInt32("companyId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            if (companyId == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(companyId);
            if (company == null)
            {
                return NotFound();
            }
            ViewBag.ShowFooter = false;
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(Company company)
        {
            if (ModelState.IsValid)
            {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                ViewBag.ShowFooter = false;
                return RedirectToAction("Details", new { id = company.CompanyId }); ;
            }
            ViewBag.ShowFooter = false;
            return RedirectToAction("Details", new { id = company.CompanyId }); ;
        }
        

        // GET: Companies/Delete/5
        [HttpGet("companies/{id}/delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetInt32("companyId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }
            ViewBag.ShowFooter = false;
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Companies == null)
            {
                return Problem("Entity set 'MyContext.Companies'  is null.");
            }
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
            }
            ViewBag.ShowFooter = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
          return (_context.Companies?.Any(e => e.CompanyId == id)).GetValueOrDefault();
        }

    }
}
