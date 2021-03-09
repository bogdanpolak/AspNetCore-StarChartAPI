using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null) return NotFound();
            BuildSatelites(celestialObject);
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects
                .Where(co => co.Name == name)
                .ToList();
            if (!celestialObjects.Any()) return NotFound();
            celestialObjects.ForEach(co => BuildSatelites(co));
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects
                .ToList();
            celestialObjects.ForEach(co => BuildSatelites(co));
            return Ok(celestialObjects);
        }

        private void BuildSatelites(CelestialObject celestialObject)
        {
            celestialObject.Satellites = _context.CelestialObjects
                .Where(co => co.OrbitedObjectId == celestialObject.Id)
                .ToList();
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _ = _context.SaveChanges();
            return CreatedAtRoute(
                nameof(GetById),
                new { celestialObject.Id },
                celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(
            int id,
            CelestialObject updatedCelestialObject)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null) return NotFound();
            celestialObject.Name = updatedCelestialObject.Name;
            celestialObject.OrbitalPeriod = updatedCelestialObject.OrbitalPeriod;
            celestialObject.OrbitedObjectId = updatedCelestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(celestialObject);
            _ = _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null) return NotFound();
            celestialObject.Name = name;
            _context.CelestialObjects.Update(celestialObject);
            _ = _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objectWithSatelites = _context.CelestialObjects
                .Where(co => co.Id == id || co.OrbitedObjectId == id)
                .ToList();
            if (objectWithSatelites.Count == 0) return NotFound();
            _context.CelestialObjects.RemoveRange(objectWithSatelites);
            _ = _context.SaveChanges();
            return NoContent();
        }
    }
}
