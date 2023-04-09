using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P230_Pronia.DAL;
using P230_Pronia.Entities;
using P230_Pronia.ViewModels.Cookie;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace P230_Pronia.Controllers
{
    public class PlantComparer : IEqualityComparer<Plant>
    {
        public bool Equals(Plant? x, Plant? y)
        {
            if (Equals(x?.Id, y?.Id)) return true;
            return false;
        }

        public int GetHashCode([DisallowNull] Plant obj)
        {
            throw new NotImplementedException();
        }
    }

    public class PlantCategoryComparer : IEqualityComparer<PlantCategory>
    {
        public bool Equals(PlantCategory? x, PlantCategory? y)
        {
            if (Equals(x?.Category.Id, y?.Category.Id)) return true;
            return false;
        }

        public int GetHashCode([DisallowNull] PlantCategory obj)
        {
            throw new NotImplementedException();
        }
    }

    public class PlantController : Controller
    {
        private readonly ProniaDbContext _context;
        public PlantController(ProniaDbContext context)
        {
            _context = context;
        }
        public IActionResult Detail(int id)
        {
            if (id == 0) return NotFound();
            IQueryable<Plant> plants = _context.Plants.AsNoTracking().AsQueryable();
            Plant? plant = plants
                                .Include(p => p.PlantImages)
                                    .Include(p => p.PlantDeliveryInformation)
                                        .Include(p => p.PlantTags)
                                            .ThenInclude(pt => pt.Tag)
                                                .Include(p => p.PlantCategories)
                                                    .ThenInclude(pc => pc.Category).AsSingleQuery().FirstOrDefault(p => p.Id == id);

            if (plant is null) return NotFound();

            ViewBag.Relateds = RelatedPlants(plants, plant, id);
            return View(plant);
        }

        static List<Plant> RelatedPlants(IQueryable<Plant> queryable, Plant plant, int id)
        {
            List<Plant> relateds = new();

            plant.PlantCategories.ForEach(pc =>
            {
                List<Plant> related = queryable
                    .Include(p => p.PlantImages)
                        .Include(p => p.PlantCategories)
                            .ThenInclude(pc => pc.Category)
                                    //.AsSingleQuery()
                                    .AsEnumerable()
                                        .Where(
                                        p => p.PlantCategories.Contains(pc, new PlantCategoryComparer())
                                        && p.Id != id
                                        && !relateds.Contains(p, new PlantComparer())
                                        )
                                        .ToList();
                relateds.AddRange(related);
            });
            return relateds;
        }

        public IActionResult AddBasket(int id)
        {
            //return Json(HttpContext.Request.Cookies["basket"] == null);
            if (id <= 0) return NotFound();
            Plant plant = _context.Plants.FirstOrDefault(p => p.Id == id);
            if (plant is null) return NotFound();
            var cookies = HttpContext.Request.Cookies["basket"];
            CookieBasketVM basket = new();
            if (cookies is null)
            {
                CookieBasketItemVM item = new CookieBasketItemVM
                {
                    Id = plant.Id,
                    Quantity = 1,
                    Price = (double)plant.Price
                };
                basket.CookieBasketItemVMs.Add(item);
                basket.TotalPrice = (double)plant.Price;
            }
            else
            {
                basket = JsonConvert.DeserializeObject<CookieBasketVM>(cookies);
                CookieBasketItemVM existed = basket.CookieBasketItemVMs.Find(c => c.Id == id);
                if (existed is null)
                {
                    CookieBasketItemVM newItem = new()
                    {
                        Id = plant.Id,
                        Quantity = 1,
                        Price = (double)plant.Price
                    };
                    basket.CookieBasketItemVMs.Add(newItem);
                    basket.TotalPrice += newItem.Price;
                }
                else
                {
                    existed.Quantity++;
                    basket.TotalPrice += existed.Price;
                }
                //basket.TotalPrice = basket.CookieBasketItemVMs.Sum(c=>c.Quantity*c.Price);
            }
            var basketStr = JsonConvert.SerializeObject(basket);

            HttpContext.Response.Cookies.Append("basket", basketStr);

            return RedirectToAction("Index", "Home");

        }
        public IActionResult ShowBasket()
        {
            var cookies = HttpContext.Request.Cookies["basket"];
            //return View(nameof(Index));
            return Json(JsonConvert.DeserializeObject<CookieBasketVM>(cookies));
        }
    }
}
