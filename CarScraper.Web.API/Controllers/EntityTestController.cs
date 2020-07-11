//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using CarScraper.Web.API.Data;
//using CarScraper.Web.API.Models;

//namespace CarScraper.Web.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class EntityTestController : ControllerBase
//    {
//        private readonly CarScraperWebAPIContext _context;

//        public EntityTestController(CarScraperWebAPIContext context)
//        {
//            _context = context;
//        }

//        // GET: api/EntityTest
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<SearchData>>> GetSearchData()
//        {
//            return await _context.SearchData.ToListAsync();
//        }

//        // GET: api/EntityTest/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<SearchData>> GetSearchData(int id)
//        {
//            var searchData = await _context.SearchData.FindAsync(id);

//            if (searchData == null)
//            {
//                return NotFound();
//            }

//            return searchData;
//        }

//        // PUT: api/EntityTest/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for
//        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutSearchData(int id, SearchData searchData)
//        {
//            if (id != searchData.Id)
//            {
//                return BadRequest();
//            }

//            _context.Entry(searchData).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!SearchDataExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // POST: api/EntityTest
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for
//        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
//        [HttpPost]
//        public async Task<ActionResult<SearchData>> PostSearchData(SearchData searchData)
//        {
//            _context.SearchData.Add(searchData);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction("GetSearchData", new { id = searchData.Id }, searchData);
//        }

//        // DELETE: api/EntityTest/5
//        [HttpDelete("{id}")]
//        public async Task<ActionResult<SearchData>> DeleteSearchData(int id)
//        {
//            var searchData = await _context.SearchData.FindAsync(id);
//            if (searchData == null)
//            {
//                return NotFound();
//            }

//            _context.SearchData.Remove(searchData);
//            await _context.SaveChangesAsync();

//            return searchData;
//        }

//        private bool SearchDataExists(int id)
//        {
//            return _context.SearchData.Any(e => e.Id == id);
//        }
//    }
//}
