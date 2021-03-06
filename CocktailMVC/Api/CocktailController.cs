﻿using CocktailMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using AutoMapper;
using CocktailMVC.Dtos;

namespace CocktailMVC.Api
{
    public class CocktailController : ApiController
    {
        private ApplicationDbContext _context;
        public CocktailController()
        {
            _context = new ApplicationDbContext();
        }

        // /api/Cocktail
        public IHttpActionResult GetCocktails()
        {
            var cocktails = _context.Cocktails.Include(c => c.Theme).ToList();
            var cocktailsDto = Mapper.Map<IEnumerable<Cocktail>, IEnumerable<CocktailDto>>(cocktails);

            return Ok(cocktailsDto);
        }
        
        // /api/Cocktail/1
        public IHttpActionResult GetCocktail(int id)
        {
            var cocktail = _context.Cocktails.Include(c => c.Theme).SingleOrDefault(c => c.Id == id);

            if (cocktail == null)
                return NotFound();

            var cocktailDto = Mapper.Map<Cocktail, CocktailDto>(cocktail);

            return Ok(cocktailDto);
        }

        [HttpPost]
        public IHttpActionResult PostCocktail(CocktailForPostDto cocktailDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var cocktail = Mapper.Map<CocktailForPostDto, Cocktail>(cocktailDto);
            _context.Cocktails.Add(cocktail);
            _context.SaveChanges();

            var createdCocktailDto = Mapper.Map<Cocktail, CocktailDto>(cocktail);

            return Created(new Uri(Request.RequestUri + "/" + createdCocktailDto.Id), createdCocktailDto);
        }

        [HttpPut]
        public IHttpActionResult PutCocktail(int id, CocktailDto cocktailDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var cocktail = _context.Cocktails.SingleOrDefault(c => c.Id == id);

            if (cocktail == null)
                return NotFound();

            Mapper.Map<CocktailDto, Cocktail>(cocktailDto, cocktail);
            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete]
        public IHttpActionResult DeteleCocktail(int id)
        {
            var cocktail = _context.Cocktails.SingleOrDefault(c => c.Id == id);

            if (cocktail == null)
                return NotFound();

            _context.Cocktails.Remove(cocktail);
            _context.SaveChanges();

            return Ok();
        }
    }
}
