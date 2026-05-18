using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


using QMS.Data;
using QMS.Models;
using QMS.Services;

namespace QMS.Controllers;

[ApiController]
[Route("api/inspections")]
public class InspectionsController : ControllerBase
{
    private readonly QmsDbContext _context;
    private readonly QmsService _service;

    public InspectionsController(QmsDbContext context)
    {
        _context = context;
        _service = new QmsService();
    }

  [Authorize(Roles ="QAE")]
  [HttpPost]
public IActionResult AddInspection(Inspection inspection)
{
    //  Prevent duplicate inspection for same projector
    bool alreadyInspected = _context.Inspections
        .Any(i => i.ProjectorId == inspection.ProjectorId);

    if (alreadyInspected)
    {
        return BadRequest("Inspection already exists for this projector");
    }

    inspection.Result = _service.Evaluate(inspection);
    inspection.CheckedAt = DateTime.Now;

    _context.Inspections.Add(inspection);
    _context.SaveChanges();

    return Ok(inspection);
}
// [Authorize(Roles ="Analyst")]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.Inspections.ToList());
    }

    [HttpGet("pass-rate")]
    public IActionResult PassRate()
    {
        int total = _context.Inspections.Count();
        int pass = _context.Inspections.Count(x => x.Result == "Pass");

        return Ok(new
        {
            Total = total,
            Pass = pass,
            Fail = total - pass
        });
    }
    [Authorize(Roles ="ANALYST")]
    [HttpGet("batch-summary")]
public IActionResult BatchSummary()
{
    var result = _context.Inspections
        .Join(
            _context.Projectors,
            i => i.ProjectorId,
            p => p.ProjectorId,
            (i, p) => new
            {
                p.BatchNumber,
                i.ProjectorId,
                i.Result
            }
        )
        // One result per projector
        .GroupBy(x => new { x.BatchNumber, x.ProjectorId })
        .Select(g => new
        {
            g.Key.BatchNumber,
            g.Key.ProjectorId,
            FinalResult = g.Any(x => x.Result == "Fail") ? "Fail" : "Pass"
        })
        // Batch-wise summary
        .GroupBy(x => x.BatchNumber)
        .Select(bg => new
        {
            Batch = bg.Key,
            TotalProjectors = bg.Count(),
            Pass = bg.Count(x => x.FinalResult == "Pass"),
            Fail = bg.Count(x => x.FinalResult == "Fail")
        })
        .ToList();

    return Ok(result);
}

    [HttpPut("{inspectionId}")]
public IActionResult UpdateInspection(int inspectionId, Inspection updatedInspection)
{
    // Find existing inspection
    var inspection = _context.Inspections.Find(inspectionId);
    if (inspection == null)
    {
        return NotFound("Inspection not found");
    }

    //  Update only editable fields
    inspection.Lens = updatedInspection.Lens;
    inspection.Button = updatedInspection.Button;
    inspection.Power = updatedInspection.Power;
    inspection.Speaker = updatedInspection.Speaker;
    inspection.Temperature = updatedInspection.Temperature;

    // Recalculate result
    inspection.Result = _service.Evaluate(inspection);

    _context.SaveChanges();

    return Ok(inspection);
}
[HttpDelete("{inspectionId}")]
public IActionResult DeleteInspection(int inspectionId)
{
    var inspection = _context.Inspections.Find(inspectionId);

    if (inspection == null)
    {
        return NotFound("Inspection not found");
    }

    _context.Inspections.Remove(inspection);
    _context.SaveChanges();

    return Ok($"Inspection {inspectionId} deleted successfully");
}
}
