using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Gardentools.Data;
using Gardentools.Models;
using Gardentools.Helpers;
using static System.Formats.Asn1.AsnWriter;

namespace Gardentools.Pages.Articles
{
    public class DetailsModel : PageModel
    {
        private readonly Gardentools.Data.GardentoolsContext _context;

        public DetailsModel(Gardentools.Data.GardentoolsContext context)
        {
            _context = context;
        }

        public Article Article { get; set; }
        public int? PreviousId { get; set; }
        public int? NextId { get; set; }
        private IList<Article> Articles { get; set; }
        public Availability Availability { get; private set; }

        public IActionResult OnGet(int? id)
        {
            Availability = new Availability(_context, HttpContext);
            if (id == null || _context.Article == null)
            {
                return NotFound();
            }

            var article = _context.Article
                .Include(b => b.Brand)
                .Include(f => f.Category).ToList()
                .FirstOrDefault(m => m.Id == id);

            Articles = _context.Article
                .Include(b => b.Brand)
                .Include(f => f.Category).ToList();
            Articles = Articles.OrderBy(s => s.Brand.BrandName)
                .ThenBy(s => s.Category.CategoryName).ToList();
            PreviousId = null;
            NextId = null;
            for (int i = 0; i < Articles.Count; i++)
            {
                if (((Article)Articles[i]).Id == id)
                {
                    if (i > 0)
                        PreviousId = ((Article)Articles[i - 1]).Id;
                    if (i < Articles.Count - 1)
                        NextId = ((Article)Articles[i + 1]).Id;
                    break;
                }
            }
            if (PreviousId == null) PreviousId = id;
            if (NextId == null) NextId = id;


            if (article == null)
            {
                return NotFound();
            }
            else
            {
                Article = article;
            }
            return Page();
        }

    }
}
