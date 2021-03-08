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
    }
}
