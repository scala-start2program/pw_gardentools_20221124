using Gardentools.Helpers;
using Gardentools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.EntityFrameworkCore;

namespace Gardentools.Pages.Articles
{
    public class BasketModel : PageModel
    {
        private readonly Gardentools.Data.GardentoolsContext _context;
        public Availability Availability { get; set; }
        public Article Article { get; set; }

        [BindProperty]
        public Basket Basket { get; set; }

        public BasketModel(Gardentools.Data.GardentoolsContext context)
        {
            _context = context;
        }
        public IActionResult OnGet(int? id)
        {
            Availability = new Availability(_context, HttpContext);
            if (id == null)
            {
                return NotFound();
            }
            Article = _context.Article
                .Include(s => s.Brand)
                .Include(s => s.Category)
                .FirstOrDefault(m => m.Id == id);
            if (Article == null)
            {
                return NotFound();
            }
            return Page();
        }
        public IActionResult OnPost(int? id)
        {
            Availability = new Availability(_context, HttpContext);
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (id == null)
            {
                return Page();
            }
            Basket.ArticleId = (int)id;
            Basket.UserId = int.Parse(Availability.UserId);


            _context.Basket.Add(Basket);
            _context.SaveChanges();

            return RedirectToPage("./Index");
        }
    }
}
