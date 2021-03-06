﻿using CocktailMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace CocktailMVC.Controllers
{
   
    public class CocktailController : Controller
    {
        
        private ApplicationDbContext _context;

        public CocktailController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Cocktail
        public ActionResult Index()
        {
            var cocktails = _context.Cocktails.Include(c => c.Theme).ToList();

            if (User.IsInRole(RoleName.BarManager))
                return View("Index", cocktails);

            return View("IndexReadOnly", cocktails);
        }

        [Authorize(Roles = RoleName.BarManager)]
        public ActionResult New()
        {
            var cocktailViewModel = new CocktailViewModel()
            {
                Themes = _context.Themes.ToList()
            };
            return View(cocktailViewModel);
        }

        [Authorize(Roles = RoleName.BarManager)]
        public ActionResult Edit(int id)
        {
            var cocktail = _context.Cocktails.Single(c => c.Id == id);
            var cocktailViewModel = new CocktailViewModel(cocktail)
            {
                Themes = _context.Themes.ToList()
            };
            return View("New", cocktailViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.BarManager)]
        public ActionResult Save(Cocktail cocktail)
        {
            if (!ModelState.IsValid)
            {
                var cocktailViewModel = new CocktailViewModel(cocktail)
                {
                    Themes = _context.Themes.ToList()
                };
            return View("New", cocktailViewModel);
            }

            if (cocktail.Id == 0)
            {
                _context.Cocktails.Add(cocktail);
            }
            else
            {
                var cocktailInDb = _context.Cocktails.SingleOrDefault(c => c.Id == cocktail.Id);
                cocktailInDb.Name = cocktail.Name;
                cocktailInDb.Price = cocktail.Price;
                cocktailInDb.Description = cocktail.Description;
                cocktailInDb.ThemeId = cocktail.ThemeId;
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Cocktail");
        }

        [Authorize(Roles = RoleName.BarManager)]
        public ActionResult Delete(int id)
        {
            var cocktail = _context.Cocktails.SingleOrDefault(c => c.Id == id);

            _context.Cocktails.Remove(cocktail);
            _context.SaveChanges();

            return RedirectToAction("Index", "Cocktail");
        }

        public ActionResult Detail(int id)
        {
            var cocktail = _context.Cocktails.Include(c => c.Theme).SingleOrDefault(c => c.Id == id);

            if (cocktail == null)
                return HttpNotFound();

            return View(cocktail);
        }
    }
}