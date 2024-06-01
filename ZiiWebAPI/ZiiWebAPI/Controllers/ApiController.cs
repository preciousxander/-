using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassDataBaseLibrary;

namespace ZiiWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController<T> : ControllerBase where T : BaseModel
    {
        private DataBase db = new DataBase("dataBase");

        //[Authorize]
        [HttpGet]
        public virtual async Task<IActionResult> List()
        {
            var entities = await db.Set<T>().ToListAsync();

            return Ok(entities);
        }

        //[Authorize]
        [HttpPost]
        public virtual async Task<IActionResult> Create(T entity)
        {
            await db.Set<T>().AddAsync(entity);
            await db.SaveChangesAsync();

            return Ok("Save succesful");
        }

        //[Authorize]
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get(int id)
        {
            if (!await EntityExists(id))
                return NotFound();

            var entity = await db.Set<T>().FindAsync(id);

            return Ok(entity);
        }


        //[Authorize]
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(int id, T entity)
        {
            if (entity.Id != id)
            {
                return BadRequest();
            }

            try
            {
                db.Update(entity);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await db.Set<T>().FindAsync(id);

            if (entity == null)
                return NotFound();

            db.Set<T>().Remove(entity);
            await db.SaveChangesAsync();

            return NoContent();
        }

        private Task<bool> EntityExists(int id)
        {
            return db.Set<T>().AnyAsync(e => e.Id == id);
        }
    }
}
