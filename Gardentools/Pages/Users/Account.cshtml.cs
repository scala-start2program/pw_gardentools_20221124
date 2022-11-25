using Gardentools.Helpers;
using Gardentools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gardentools.Pages.Users
{
    public class AccountModel : PageModel
    {
        private readonly Gardentools.Data.GardentoolsContext _context;

        public AccountModel(Gardentools.Data.GardentoolsContext context)
        {
            _context = context;
        }
        [BindProperty]
        public User Me { get; set; }
        public Availability Availability { get; set; }

        public IActionResult OnGet()
        {
            Availability = new Availability(_context, HttpContext);
            if (string.IsNullOrEmpty(Availability.UserId))
            {
                return NotFound();
            }
            int id = int.Parse(Availability.UserId);

            Me = _context.User.FirstOrDefault(m => m.Id == id);
            if (Me == null)
            {
                return NotFound();
            }
            Me.Password = "";
            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            int searchId = Me.Id;
            User copyUser = _context.User.FirstOrDefault(m => m.Id == searchId);
            bool isAdmin = copyUser.IsAdmin;
            string oldPassword = copyUser.Password;


            if (Me.Password!=null &&  Me.Password.Length > 0)
                Me.Password = Gardentools.Helpers.Encoding.EncodePassword(Me.Password);
            else
                Me.Password = oldPassword;
            Me.IsAdmin = isAdmin;
            _context.Attach(copyUser).State = EntityState.Detached;

            _context.Attach(Me).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(Me.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("../articles/Index");
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
