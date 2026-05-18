using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QMS.Data;
using QMS.Models;
namespace QMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectorController : ControllerBase
    {
        private readonly QmsDbContext _context;
 
    public ProjectorController(QmsDbContext context)
    {
        _context = context;
    }
 
   [HttpPost]
public IActionResult Add(Projector projector)
{
    int count = _context.Projectors
        .Count(p => p.BatchNumber == projector.BatchNumber);

    if (count >= 10)
    {
        return BadRequest("This batch already has 10 projectors");
    }

    _context.Projectors.Add(projector);
    _context.SaveChanges();

    return Ok(projector);
}
 
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.Projectors.ToList());
    }
}
        
    }

