using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectRecycle.Models;
using ProjectRecycle.Utility;

namespace ProjectRecycle.Controllers
{
    public class OffersController : Controller
    {
        private readonly MyContext _context;

        public OffersController(MyContext context)
        {
            _context = context;
        }

        public class MyModel
        {
            public List<Offer> LastSixRecordsByEndDate { get; set; }
            public List<Offer> LastSixRecordsByCreatedAt { get; set; }
            public List<Offer> MostSixPopularRecords { get; set; }
            public List<Offer> AllOffers { get; set; }
        }
        // GET: Offers
        public async Task<IActionResult> Offers()
        {
            var myContext = _context.Offers.Include(o => o.Waste);
            ViewBag.ShowFooter = false;
            return View(await myContext.ToListAsync());
        }
        // GET: Offers
        public IActionResult Index()
        {
            var MyContext = _context.Offers.Include(o => o.Waste);

            var allOffers = _context.Offers.Include(o => o.Waste).ToList();


            var lastSixRecordsByEndDate = _context.Offers.Where(d => d.EndDate > DateTime.Now).Include(o => o.Waste)
            .OrderBy(x => x.EndDate) // Order by the property that determines the order
            .Take(6)
            .ToList();

            var lastSixRecordsByCreatedAt = _context.Offers.Include(o => o.Waste)
            .OrderByDescending(x => x.CreatedAt) // Order by the property that determines the order
            .Take(6)
            .ToList();

            var mostSixPopularRecords = _context.Offers.Include(o => o.Waste)
            .OrderByDescending(x => x.Bids.Count) // Order by the property that determines the order
            .Take(6)
            .ToList();
            //Console.WriteLine($"WASTE0-------- {myContext.ToList()[0].Waste.Title}");

            var myModel = new MyModel
            {
                LastSixRecordsByEndDate = lastSixRecordsByEndDate,
                LastSixRecordsByCreatedAt = lastSixRecordsByCreatedAt,
                MostSixPopularRecords = mostSixPopularRecords,
                AllOffers = allOffers

            };
            
            return View(myModel);
        }

        // GET: Offers/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null || _context.Offers == null)
            {
                return NotFound();
            }

            var offer = _context.Offers
            .Where(o => o.OfferId == id)
            .Include(o => o.Waste)
            .Include(o => o.Bids) // Include the related Bids for the Offer
            .ThenInclude(b => b.Bidder) // Include the related Bidder (Company)
            .FirstOrDefault();
            if (offer == null)
            {
                return NotFound();
            }
            
            return View(offer);
        }
        [HttpPost]
        public async Task<IActionResult> Bid(Bid bid)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bid);
                await _context.SaveChangesAsync();
                return Redirect($"Details/{bid.OfferId}");
            }
            
            return Redirect($"Details/{bid.OfferId}");
        }

        // GET: Offers/Create
        public IActionResult Create()
        {
            ViewData["WasteId"] = new SelectList(_context.Wastes, "WasteId", "Title");
            ViewBag.ShowFooter = false;
            return View();
        }

        // POST: Offers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(Offer offer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(offer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WasteId"] = new SelectList(_context.Wastes, "WasteId", "WasteId", offer.WasteId);
            ViewBag.ShowFooter = false;
            return View(offer);
        }

        // GET: Offers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Offers == null)
            {
                return NotFound();
            }

            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }
            ViewData["WasteId"] = new SelectList(_context.Wastes, "WasteId", "WasteId", offer.WasteId);
            ViewBag.ShowFooter = false;
            return View(offer);
        }

        // POST: Offers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Offer offer)
        {
            if (id != offer.OfferId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(offer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfferExists(offer.OfferId))
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
            ViewData["WasteId"] = new SelectList(_context.Wastes, "WasteId", "WasteId", offer.WasteId);
            ViewBag.ShowFooter = false;
            return View(offer);
        }

        // GET: Offers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Offers == null)
            {
                return NotFound();
            }

            var offer = await _context.Offers
                .Include(o => o.Waste)
                .FirstOrDefaultAsync(m => m.OfferId == id);
            if (offer == null)
            {
                return NotFound();
            }
            ViewBag.ShowFooter = false;
            return View(offer);
        }

        // POST: Offers/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Offers == null)
            {
                return Problem("Entity set 'MyContext.Offers'  is null.");
            }
            var offer = await _context.Offers.FindAsync(id);
            if (offer != null)
            {
                _context.Offers.Remove(offer);
            }

            await _context.SaveChangesAsync();
            ViewBag.ShowFooter = false;
            return RedirectToAction(nameof(Index));
        }

        #region APIs
        [HttpGet]
        public IActionResult OffersByCategory(string offerCategory)
        {
            var categoryOffer = StaticData.WasteType.Concrete;
            if (offerCategory is not null)
            {
                IEnumerable<Offer> offersList = _context.Offers.Include(offer => offer.Waste).ToList();
                if (offerCategory.ToLower() == "concrete")
                {
                    categoryOffer = StaticData.WasteType.Concrete;
                    offersList = _context.Offers.
                    Include(offer => offer.Waste).
                    Where(offer => offer.Waste.Type == categoryOffer).ToList();
                }
                else if (offerCategory.ToLower() == "plastic")
                {
                    categoryOffer = StaticData.WasteType.Plastic;
                    offersList = _context.Offers.
                    Include(offer => offer.Waste).
                    Where(offer => offer.Waste.Type == categoryOffer).ToList();
                }
                else if (offerCategory.ToLower() == "paper")
                {
                    categoryOffer = StaticData.WasteType.Paper;
                    offersList = _context.Offers.
                    Include(offer => offer.Waste).
                    Where(offer => offer.Waste.Type == categoryOffer).ToList();
                }
                else if (offerCategory.ToLower() == "metal")
                {
                    categoryOffer = StaticData.WasteType.Metal;
                    offersList = _context.Offers.
                    Include(offer => offer.Waste).
                    Where(offer => offer.Waste.Type == categoryOffer).ToList();
                }
                else if (offerCategory.ToLower() == "hazardous")
                {
                    categoryOffer = StaticData.WasteType.hazardous;
                    offersList = _context.Offers.
                    Include(offer => offer.Waste).
                    Where(offer => offer.Waste.Type == categoryOffer).ToList();
                }else if (offerCategory.ToLower() == "wood")
                {
                    categoryOffer = StaticData.WasteType.Wood;
                    offersList = _context.Offers.
                    Include(offer => offer.Waste).
                    Where(offer => offer.Waste.Type == categoryOffer).ToList();
                }
                //else if (offerCategory.ToLower() == "all")
                //{
                //    offersList = _context.Offers.Include(offer => offer.Waste).ToList();
                //}
                return Json(new { data = offersList });
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        private bool OfferExists(int id)
        {
            return (_context.Offers?.Any(e => e.OfferId == id)).GetValueOrDefault();
        }
    }
}
