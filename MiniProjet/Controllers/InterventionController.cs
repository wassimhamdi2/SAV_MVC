using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniProjet.Models;
using MiniProjet.Models.Repository;
using System.Linq;
using System.Collections.Generic;

public class InterventionController : Controller
{
    private readonly IRepository<Intervention> _interventionRepository;
    private readonly IRepository<Complaint> _complaintRepository;
    private readonly IRepository<Technicien> _technicientRepository;
    private readonly UserManager<IdentityUser> _userManager;

    public InterventionController(
        IRepository<Intervention> interventionRepository,
        IRepository<Complaint> complaintRepository,
        IRepository<Technicien> technicientRepository,
        UserManager<IdentityUser> userManager)
    {
        _interventionRepository = interventionRepository;
        _complaintRepository = complaintRepository;
        _technicientRepository = technicientRepository;
        _userManager = userManager;
    }

    [Authorize(Roles = "Client")]
    public IActionResult MyInterventions()
    {
        var userId = _userManager.GetUserId(User); // Get logged-in user's ID

        var interventions = _interventionRepository.GetAllWithIncludes()
            .Where(i => i.Complaint.CustomerId == userId)
            .ToList();

        return View(interventions);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        var interventions = _interventionRepository.GetAllWithIncludes().ToList();
        return View(interventions);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        // Get all complaints and technicians
        var complaints = _complaintRepository.GetAll()?.Select(c => new { c.Id, c.Description }).ToList();
        var techniciens = _technicientRepository.GetAll()?.Select(t => new { t.Id, t.Name }).ToList();

        // Check if complaints or techniciens are null or empty
        if (complaints == null || !complaints.Any())
        {
            ModelState.AddModelError("", "No complaints found in the system.");
        }

        if (techniciens == null || !techniciens.Any())
        {
            ModelState.AddModelError("", "No technicians found in the system.");
        }

        // Check if ModelState has errors before proceeding
        if (!ModelState.IsValid)
        {
            // If there are issues with the ModelState, return to the view with the model errors
            return View();
        }

        // Populate ViewBag with the SelectLists if no errors
        ViewBag.TechnicienId = new SelectList(techniciens, "Id", "Name");
        ViewBag.ComplaintId = new SelectList(complaints, "Id", "Description");

        return View();
    }
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Intervention? intervention)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _interventionRepository.Add(intervention);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error occurred while creating intervention: " + ex.Message);
            }
        }

        var complaints = _complaintRepository.GetAll()
            .Select(c => new { c.Id, c.Description })
            .ToList();
        ViewBag.ComplaintId = new SelectList(complaints, "Id", "Description", intervention.ComplaintId);

        var techniciens = _technicientRepository.GetAll()
            .Select(t => new { t.Id, t.Name })
            .ToList();
        ViewBag.TechnicienId = new SelectList(techniciens, "Id", "Name", intervention.TechnicienId);

        return View(intervention);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id)
    {
        var intervention = _interventionRepository.Get(id);
        if (intervention == null) return NotFound();

        var complaints = _complaintRepository.GetAll()
            .Select(c => new { c.Id, c.Description })
            .ToList();
        ViewBag.ComplaintId = new SelectList(complaints, "Id", "Description", intervention.ComplaintId);

        var techniciens = _technicientRepository.GetAll()
            .Select(t => new { t.Id, t.Name })
            .ToList();
        ViewBag.TechnicienId = new SelectList(techniciens, "Id", "Name", intervention.TechnicienId);

        return View(intervention);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(Intervention intervention)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _interventionRepository.Update(intervention);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error occurred while editing intervention: " + ex.Message);
            }
        }

        var complaints = _complaintRepository.GetAll()
            .Select(c => new { c.Id, c.Description })
            .ToList();
        ViewBag.ComplaintId = new SelectList(complaints, "Id", "Description", intervention.ComplaintId);

        var techniciens = _technicientRepository.GetAll()
            .Select(t => new { t.Id, t.Name })
            .ToList();
        ViewBag.TechnicienId = new SelectList(techniciens, "Id", "Name", intervention.TechnicienId);

        return View(intervention);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var intervention = _interventionRepository.Get(id);
        if (intervention == null) return NotFound();

        return View(intervention);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _interventionRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error occurred while deleting intervention: " + ex.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}
