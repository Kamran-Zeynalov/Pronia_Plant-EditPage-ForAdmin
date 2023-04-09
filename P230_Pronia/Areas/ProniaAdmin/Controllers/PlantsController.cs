using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using P230_Pronia.DAL;
using P230_Pronia.Entities;
using P230_Pronia.Utilities.Extensions;
using P230_Pronia.ViewModels;

namespace P230_Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class PlantsController : Controller
    {
        private readonly ProniaDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PlantsController(ProniaDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<Plant> model = _context.Plants.Include(p => p.PlantImages)
                                                        .Include(p => p.PlantSizeColors).ThenInclude(p => p.Size)
                                                        .Include(p => p.PlantSizeColors).ThenInclude(p => p.Color)
                                                         .AsNoTracking().AsEnumerable();
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Informations = _context.PlantDeliveryInformation.AsEnumerable();
            ViewBag.Categories = _context.Categories.AsEnumerable();
            ViewBag.Tags = _context.Tags.AsEnumerable();
            ViewBag.Sizes = _context.Sizes.AsEnumerable();
            ViewBag.Colors = _context.Colors.AsEnumerable();
            ViewBag.ColorSizeQuantity = _context.PlantSizeColors.AsEnumerable();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlantVM newPlant)
        {
            ViewBag.Informations = _context.PlantDeliveryInformation.AsEnumerable();
            ViewBag.Categories = _context.Categories.AsEnumerable();
            ViewBag.Tags = _context.Tags.AsEnumerable();

            ViewBag.Sizes = _context.Sizes.AsEnumerable();
            ViewBag.Colors = _context.Colors.AsEnumerable();
            ViewBag.ColorSizeQuantity = _context.PlantSizeColors.AsEnumerable();
            TempData["InvalidImages"] = string.Empty;
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!newPlant.HoverPhoto.IsValidFile("image/") || !newPlant.MainPhoto.IsValidFile("image/"))
            {
                ModelState.AddModelError(string.Empty, "Please choose image file");
                return View();
            }
            if (!newPlant.HoverPhoto.IsValidLength(1) || !newPlant.MainPhoto.IsValidLength(1))
            {
                ModelState.AddModelError(string.Empty, "Please choose image which size is maximum 1MB");
                return View();
            }

            Plant plant = new()
            {
                Name = newPlant.Name,
                Desc = newPlant.Desc,
                Price = newPlant.Price,
                SKU = newPlant.SKU,
                PlantDeliveryInformationId = newPlant.PlantDeliveryInformationId
            };
            string imageFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
            foreach (var image in newPlant.Images)
            {
                if (!image.IsValidFile("image/") || !image.IsValidLength(1))
                {
                    TempData["InvalidImages"] += image.FileName;
                    continue;
                }
                PlantImage plantImage = new()
                {
                    IsMain = false,
                    Path = await image.CreateImage(imageFolderPath, "website-images")
                };
                plant.PlantImages.Add(plantImage);
            }

            PlantImage main = new()
            {
                IsMain = true,
                Path = await newPlant.MainPhoto.CreateImage(imageFolderPath, "website-images")
            };
            plant.PlantImages.Add(main);
            PlantImage hover = new()
            {
                IsMain = null,
                Path = await newPlant.HoverPhoto.CreateImage(imageFolderPath, "website-images")
            };
            plant.PlantImages.Add(hover);

            foreach (int id in newPlant.CategoryIds)
            {
                PlantCategory category = new()
                {
                    CategoryId = id
                };
                plant.PlantCategories.Add(category);
            }
            foreach (int id in newPlant.TagIds)
            {
                PlantTag tag = new()
                {
                    TagId = id
                };
                plant.PlantTags.Add(tag);
            }
            string[] colorSizeQuantities = newPlant.ColorSizeQuantity.Split(',');
            foreach (string colorSizeQuantity in colorSizeQuantities)
            {
                string[] datas = colorSizeQuantity.Split('-');
                PlantSizeColor plantSizeColor = new()
                {
                    SizeId = int.Parse(datas[0]),
                    ColorId = int.Parse(datas[1]),
                    Quantity = int.Parse(datas[2])
                };
                plant.PlantSizeColors.Add(plantSizeColor);
            }
            _context.Plants.Add(plant);
            _context.SaveChanges();
            return RedirectToAction("Index", "Plants");
        }


        public IActionResult Edit(int id)
        {
            if (id == 0) return BadRequest();
            PlantVM? model = EditedPlant(id);

            ViewBag.Informations = _context.PlantDeliveryInformation.AsEnumerable();
            ViewBag.Categories = _context.Categories.AsEnumerable();
            ViewBag.Tags = _context.Tags.AsEnumerable();
            ViewBag.Sizes = _context.Sizes.AsEnumerable();
            ViewBag.Colors = _context.Colors.AsEnumerable();
            if (model is null) return BadRequest();
            _context.SaveChanges();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, PlantVM edited)
        {
            //Plant plantt = _context.Plants.Include(p => p.PlantCategories).FirstOrDefault(x => x.Id == id);
            ViewBag.Informations = _context.PlantDeliveryInformation.AsEnumerable();
            ViewBag.Categories = _context.Categories.AsEnumerable();
            ViewBag.Tags = _context.Tags.AsEnumerable();
            PlantVM? model = EditedPlant(id);

            Plant? plant = await _context.Plants.Include(p => p.PlantImages).FirstOrDefaultAsync(p => p.Id == id);
            if (plant is null) return BadRequest();


            IEnumerable<string> removables = plant.PlantImages.Where(p => !edited.ImageIds.Contains(p.Id)).Select(i => i.Path).AsEnumerable();
            string imageFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
            foreach (string removable in removables)
            {
                string path = Path.Combine(imageFolderPath, "website-images", removable);
                await Console.Out.WriteLineAsync(path);
                Console.WriteLine(FileUpload.DeleteImage(path));
            }

            if (edited.MainPhoto is not null)
            {
                await AdjustPlantPhotos(edited.MainPhoto, plant, true);
            }
            if (edited.HoverPhoto is not null)
            {
                await AdjustPlantPhotos(edited.HoverPhoto, plant, null);
            }

            plant.PlantImages.RemoveAll(p => !edited.ImageIds.Contains(p.Id));
            if (edited.Images is not null)
            {
                foreach (var item in edited.Images)
                {
                    if (!item.IsValidFile("image/") || !item.IsValidLength(1))
                    {
                        TempData["InvalidImages"] += item.FileName;
                        continue;
                    }
                    PlantImage plantImage = new()
                    {
                        IsMain = false,
                        Path = await item.CreateImage(imageFolderPath, "website-images")
                    };
                    plant.PlantImages.Add(plantImage);
                }
            }




            plant.Name = edited.Name;
            plant.Price = edited.Price;
            plant.Desc = edited.Desc;
            plant.SKU = edited.SKU;


            if (edited.CategoryIds is not null)
            {
                var newCategories = new List<PlantCategory>();
                foreach (var categoryId in edited.CategoryIds)
                {
                    if (!plant.PlantCategories.Any(c => c.CategoryId == categoryId))
                    {
                        newCategories.Add(new PlantCategory { Plant = plant, CategoryId = categoryId });
                    }
                }
                //Remove hec cure eletdiremmedim(


                //plant.PlantCategories.RemoveAll(c => !edited.CategoryIds.Contains(c.CategoryId));
                plant.PlantCategories.AddRange(newCategories);
            }

            if (edited.TagIds is not null)
            {
                var newTags = new List<PlantTag>();
                foreach (var tagId in edited.TagIds)
                {
                    if (!plant.PlantTags.Any(t => t.TagId == tagId))
                    {
                        newTags.Add(new PlantTag { Plant = plant, TagId = tagId });
                    }
                }
                plant.PlantTags.AddRange(newTags);

                plant.PlantSizeColors.Clear();
            }


            string colorSizeQuantity = edited.ColorSizeQuantity;
            string[] colorSizeQuantities = colorSizeQuantity.Split(','); plant.PlantSizeColors.Clear();

            foreach (string colorSizeQuantityLoop in colorSizeQuantities)
            {
                string[] datas = colorSizeQuantityLoop.Split('-');
                PlantSizeColor plantSizeColor = new()
                {
                    SizeId = int.Parse(datas[0]),
                    ColorId = int.Parse(datas[1]),
                    Quantity = int.Parse(datas[2])
                };
                plant.PlantSizeColors.Add(plantSizeColor);
            }
            _context.SaveChanges();
            //return Json(edited.CategoryIds);
            return RedirectToAction(nameof(Index));
        }

        private PlantVM? EditedPlant(int id)
        {
            PlantVM? model = _context.Plants.Include(p => p.PlantCategories)
                                            .Include(p => p.PlantSizeColors).ThenInclude(psc => psc.Color)
                                            .Include(p => p.PlantTags)
                                            .Include(p => p.PlantImages)
                                            .Select(p =>
                                                new PlantVM
                                                {
                                                    Id = p.Id,
                                                    Name = p.Name,
                                                    SKU = p.SKU,
                                                    Desc = p.Desc,
                                                    Price = p.Price,
                                                    DiscountPrice = p.Price,
                                                    plantSizeColors = p.PlantSizeColors,
                                                    PlantDeliveryInformationId = p.PlantDeliveryInformationId,
                                                    CategoryIds = p.PlantCategories.Select(pc => pc.CategoryId).ToList(),
                                                    TagIds = p.PlantTags.Select(pc => pc.TagId).ToList(),
                                                    SpecificImages = p.PlantImages.Select(p => new PlantImage
                                                    {
                                                        Id = p.Id,
                                                        Path = p.Path,
                                                        IsMain = p.IsMain
                                                    }).ToList()
                                                })
                                                .FirstOrDefault(p => p.Id == id);
            return model;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="plant"></param>
        /// <param name="isMain">If IsMain attribute is true that is mean you want to change Main photo, if IsMain attribute is null that is mean you want to change HoverPhoto</param>
        /// <returns></returns>
        private async Task AdjustPlantPhotos(IFormFile? image, Plant? plant, bool? isMain)
        {
            string photoPath = plant.PlantImages.FirstOrDefault(p => p.IsMain == isMain).Path;
            string imagesFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
            string filePath = Path.Combine(imagesFolderPath, "website-images", photoPath);
            FileUpload.DeleteImage(filePath);
            plant.PlantImages.FirstOrDefault(p => p.IsMain == isMain).Path = await image.CreateImage(imagesFolderPath, "website-images");
        }


        public IActionResult Search(string data)
        {
            List<Plant> plant = _context.Plants.Where(p => p.Name.Contains(data)).ToList();
            return View(nameof(Index));
            //return Json(plant);
        }
    }
}