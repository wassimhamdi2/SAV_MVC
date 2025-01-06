using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjet.Models;
using MiniProjet.Models.Repository;

[Authorize(Roles = "Admin")]
public class TechnicienController : Controller
{
    private readonly IRepository<Technicien> _technicienRepository;

    public TechnicienController(IRepository<Technicien> technicienRepository)
    {
        _technicienRepository = technicienRepository;
    }

    // GET: Techniciens
    public IActionResult Index()
    {
        // Retrieve all techniciens
        var techniciens = _technicienRepository.GetAll();
        return View(techniciens);
    }

    // GET: Create Technicien
    public IActionResult Create()
    {
        return View();
    }

    // POST: Create Technicien
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Technicien technicien)
    {
        if (ModelState.IsValid)
        {
            _technicienRepository.Add(technicien); // Add technicien to the repository
            return RedirectToAction(nameof(Index));
        }

        // Log validation errors
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine(error.ErrorMessage);
        }

        return View(technicien); // Return to Create view if validation fails
    }


    // GET: Edit Technicien
    public IActionResult Edit(int id)
    {
        var technicien = _technicienRepository.Get(id); // Retrieve the technicien to edit
        if (technicien == null)
        {
            return NotFound();
        }
        return View(technicien);
    }

    // POST: Edit Technicien
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Technicien technicien)
    {
        if (ModelState.IsValid)
        {
            var updatedTechnicien = _technicienRepository.Update(technicien); // Update technicien in the repository
            if (updatedTechnicien != null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
        return View(technicien); // Return to Edit view if validation fails
    }

    // GET: Details Technicien
    public IActionResult Details(int id)
    {
        var technicien = _technicienRepository.Get(id); // Retrieve the technicien details
        if (technicien == null)
        {
            return NotFound();
        }
        return View(technicien);
    }

    // GET: Delete Technicien
    public IActionResult Delete(int id)
    {
        var technicien = _technicienRepository.Get(id); // Retrieve the technicien to delete
        if (technicien == null)
        {
            return NotFound();
        }
        return View(technicien);
    }

    // POST: Delete Technicien
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _technicienRepository.Delete(id); // Delete the technicien from the repository
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}
