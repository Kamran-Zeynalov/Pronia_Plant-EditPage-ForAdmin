using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using P230_Pronia.DAL;
using P230_Pronia.Entities;

namespace P230_Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class CategoryController : Controller
    {
        private readonly ProniaDbContext _context;

        public CategoryController(ProniaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _context.Categories.AsEnumerable();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Creates(Category newCategory)
        {
            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }

                return View();
            }
            bool isDuplicated = _context.Categories.Any(c => c.Name == newCategory.Name);
            if (isDuplicated)
            {
                ModelState.AddModelError("", "You cannot duplicate value");
                return View();
            }
            _context.Categories.Add(newCategory);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category is null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int id, Category edited)
        {
            if (id != edited.Id) return BadRequest();
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category is null) return NotFound();
            bool duplicate = _context.Categories.Any(c => c.Name == edited.Name && edited.Name != category.Name);//test == albert 
            if (duplicate)
            {
                ModelState.AddModelError("", "You cannot duplicate category name");
                return View(category);
            }
            category.Name = edited.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
