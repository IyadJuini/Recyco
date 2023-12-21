using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectRecycle.Models;
using ProjectRecycle.Utility;

namespace ProjectRecycle.Controllers
{
    public class AppUsersController : Controller
    {
        private readonly MyContext _context;

        public AppUsersController(MyContext context)
        {
            _context = context;
        }

        // GET: AppUsers
        public async Task<IActionResult> ErrorPage()
        {
            return View(await _context.AppUsers.ToListAsync());
                        
        }


        #region Admin
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("adminId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            
            
            int wastes = _context.Wastes.Count();
            int companies = _context.Companies.Count();
            int offers = _context.Offers.Count();
            int bids = _context.Bids.Count();

            ViewBag.Bids = bids;
            ViewBag.Wastes = wastes;
            ViewBag.Companies = companies;
            ViewBag.Offers = offers;

            var dashboard = _context.Companies;
            ViewBag.ShowFooter = false;
            return View(dashboard);

        }
        public async Task<IActionResult> AllOfferAsync()
        {
            if (HttpContext.Session.GetInt32("adminId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            var myContext = _context.Offers.Include(o => o.Waste).ThenInclude(w=>w.Owner);
            ViewBag.ShowFooter = false;
            return View(await myContext.ToListAsync());
        }

        public async Task<IActionResult> AllWasteAsync()
        {
            if (HttpContext.Session.GetInt32("adminId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            var myContext = _context.Wastes.Include(c => c.Owner).Include(o => o.Validation).ThenInclude(v=>v.User);
            ViewBag.ShowFooter = false;
            return View(await myContext.ToListAsync());
        }

        public async Task<IActionResult> AllCompanies()
        {
            if (HttpContext.Session.GetInt32("adminId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            ViewBag.ShowFooter = false;
            return View(await _context.Companies.ToListAsync());
        }

        public async Task<IActionResult> AllUsers()
        {
            if (HttpContext.Session.GetInt32("adminId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            ViewBag.ShowFooter = false;
            return _context.AppUsers != null ?
                        View(await _context.AppUsers.ToListAsync()) :
                        Problem("Entity set 'MyContext.AppUsers'  is null.");
        }

        // GET: AppUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetInt32("adminId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            if (id == null || _context.AppUsers == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (appUser == null)
            {
                return NotFound();
            }
            ViewBag.ShowFooter = false;
            return View(appUser);
        }
        // GET: AppUsers/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("adminId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            ViewBag.ShowFooter = false;
            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult Register(AppUser newUser)
        {
            if (HttpContext.Session.GetInt32("adminId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            if (ModelState.IsValid)
            {
                // Email Exist ?
                if (_context.AppUsers.Any(u => u.Email == newUser.Email))
                {
                    // True
                    ModelState.AddModelError("Email", "Email already in use .");
                    return View("LoginPage");
                }
                else
                {
                    // False
                    // 1 - Hash Password
                    PasswordHasher<AppUser> Hasher = new PasswordHasher<AppUser>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    // 2 - Add 
                    _context.Add(newUser);
                    // 3 - Save
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("userId", newUser.UserId);
                    HttpContext.Session.SetString("username", newUser.FirstName);
                    // HttpContext.Session.
                    return RedirectToAction("AllUsers", new { id = newUser.UserId });
                }
            }

            return View("LoginPage");
        }


        // GET: AppUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetInt32("adminId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            if (id == null || _context.AppUsers == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (appUser == null)
            {
                return NotFound();
            }
            ViewBag.ShowFooter = false;
            return View(appUser);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetInt32("adminId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            if (_context.AppUsers == null)
            {
                return Problem("Entity set 'MyContext.AppUsers'  is null.");
            }
            var appUser = await _context.AppUsers.FindAsync(id);
            if (appUser != null)
            {
                _context.AppUsers.Remove(appUser);
            }
            ViewBag.ShowFooter = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppUserExists(int id)
        {
            
            return (_context.AppUsers?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
        #endregion

        #region Consultant
        //----------------------------------------------------------------Start Consultant Pages--------------------------------------------->
        //--------------------------Update the mission status---->
        [HttpPost]
        public IActionResult UpdateMissionStatus(int missionId, string newStatus)
        {
            var mission = _context.Missions.Find(missionId);

            if (mission != null)
            {

                mission.Status = Enum.Parse<StaticData.MissionStatus>(newStatus);

                _context.SaveChanges();
            }
            ViewBag.ShowFooter = false;
            return RedirectToAction("ConsultWaste", new { id = mission.UserId });
        }
        // GET: AppUsers/Consultant/5
        [HttpGet("AppUsers/Consultant/{id}")]
        public async Task<IActionResult> Consultant(int? id)

        {
            if (HttpContext.Session.GetInt32("consultantId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            if (id == null || _context.AppUsers == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (appUser == null)
            {
                return NotFound();
            }
            ViewBag.ShowFooter = false;
            return View(appUser);
        }


        // GET: AppUsers/ConsultWastes/5
        [HttpGet("AppUsers/ConsultWastes/{id}")]
        public async Task<IActionResult> ConsultWaste(int? id)
        {
            if (HttpContext.Session.GetInt32("consultantId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            if (id == null || _context.AppUsers == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers
                .Include(u => u.Missions).ThenInclude(m => m.Waste).ThenInclude(m => m.Owner)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (appUser == null)
            {
                return NotFound();
            }
            ViewBag.ShowFooter = false;
            return View(appUser);
        }

        public FileResult DownloadFile(string fileName)
        {
            //Build the File Path.
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            string filepath = Path.Combine(uploadsFolder, fileName);

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(filepath);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }

        // GET: AppUsers/ConsultOffre/5
        [HttpGet("AppUsers/ConsultOffre/{id}")]
        public async Task<IActionResult> ConsultOffre(int? id)
        {
            if (HttpContext.Session.GetInt32("consultantId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            if (id == null || _context.AppUsers == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers
                .Include(u => u.Missions)
                    .ThenInclude(m => m.Waste).ThenInclude(w => w.Offers)
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (appUser == null)
            {
                return NotFound();
            }

            // Get the wastes with status "Approved" and include associated company information
            var approvedWastes = _context.Wastes
                .Where(w => w.Validation != null && w.Validation.Status == StaticData.MissionStatus.Approved)
                .Include(w => w.Owner)
                .ToList();

            ViewData["ApprovedWastes"] = approvedWastes;
            ViewBag.ShowFooter = false;
            return View(appUser);
        }
        
        //----------------------------------------------------------------Ending Consultant Pages--------------------------------------------->

        #endregion

        #region Shared
        //-------------------- Logout ---------------------

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: AppUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetInt32("adminId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            if (id == null || _context.AppUsers == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }
            return View(appUser);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FirstName,LastName,Email,Password,Role,Diploma,Expertise,Description,CreatedAt,UpdatedAt")] AppUser appUser)
        {
            if (HttpContext.Session.GetInt32("adminId") == null)
            {
                return RedirectToAction("ErrorPage");
            }
            if (id != appUser.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(appUser);
        }


        #endregion
        
    }
}
