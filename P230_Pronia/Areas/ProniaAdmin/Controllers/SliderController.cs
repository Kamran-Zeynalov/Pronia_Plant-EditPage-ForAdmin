using Microsoft.AspNetCore.Mvc;
using NuGet.ContentModel;
using P230_Pronia.DAL;
using P230_Pronia.Entities;
using P230_Pronia.Utilities.Extensions;

namespace P230_Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SliderController : Controller
    {
        private readonly ProniaDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(ProniaDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<Slider> sliders = _context.Sliders.AsEnumerable();
            return View(sliders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //[AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Slider newSlider)
        {
            if (newSlider.Image == null)
            {
                ModelState.AddModelError("Image", "Please choose image");
                return View();
            }
            if (!newSlider.Image.IsValidFile("image/"))
            {
                ModelState.AddModelError("Image", "Please choose image type file");
                return View();
            }
            if (!newSlider.Image.IsValidLength(1))
            {
                ModelState.AddModelError("Image", "Image size has to be maximum 1MB");
                return View();
            }

            string imagesFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
            newSlider.ImagePath = await newSlider.Image.CreateImage(imagesFolderPath, "website-images");
            _context.Sliders.Add(newSlider);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();

            Slider slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider is null) return BadRequest();
            return View(slider);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Slider edited)
        {
            if (id != edited.Id) return BadRequest();
            Slider slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (!ModelState.IsValid) return View(slider);

            _context.Entry(slider).CurrentValues.SetValues(edited);

            if (edited.Image is not null)
            {
                string imagesFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
                string filePath = Path.Combine(imagesFolderPath, "website-images", slider.ImagePath);
                FileUpload.DeleteImage(filePath);
                slider.ImagePath = await edited.Image.CreateImage(imagesFolderPath, "website-images");
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
