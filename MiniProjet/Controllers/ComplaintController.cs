using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniProjet.Models.Repository;
using MiniProjet.Models;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ComplaintController : Controller
{
    private readonly IRepository<Complaint> _complaintRepository;
    private readonly IRepository<Article> _articleRepository;  // Added repository for Article
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IRepository<Intervention> _interventionRepository;

    // Constructor
    public ComplaintController(
        IRepository<Complaint> complaintRepository,
        IRepository<Article> articleRepository,  // Inject Article repository
        UserManager<IdentityUser> userManager,
        IRepository<Intervention> interventionRepository)
    {
        _complaintRepository = complaintRepository;
        _articleRepository = articleRepository;  // Assigning Article repository
        _userManager = userManager;
        _interventionRepository = interventionRepository;
    }

    // Admin action: List all complaints
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var complaints = await _complaintRepository.GetAll()
            .AsQueryable()
            .Include(c => c.Customer)
            .Include(c => c.article)  // Including the related Article
            .ToListAsync();
        return View(complaints);
    }

    // Client action: Create a new complaint
    [Authorize(Roles = "Client")]
    public IActionResult Create()
    {
        // Fetch articles from the database
        var articles = _articleRepository.GetAll().ToList();  // Assuming you have a repository for Article

        // Pass the list of articles to the view using ViewBag
        ViewBag.Articles = articles;

        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Client")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Complaint complaint)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            complaint.CustomerId = userId;
            complaint.CreatedAt = DateTime.Now;

            // Ensure the selected article is linked
            if (complaint.ArticleId == 0)
            {
                ModelState.AddModelError("ArticleId", "Please select an article.");
                return View(complaint);
            }

            // Save the complaint to the database
            _complaintRepository.Add(complaint);

            return RedirectToAction(nameof(MyComplaints));
        }
        catch
        {
            return View(complaint);
        }
    }

    // Client action: View a specific complaint
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Details(int id)
    {
        var complaint = await _complaintRepository.GetAllWithIncludes()
            .Include(c => c.article)  // Include the related Article
            .Include(c => c.Customer) // If you need to include Customer as well
            .FirstOrDefaultAsync(c => c.Id == id);

        if (complaint == null)
        {
            return NotFound();
        }
        return View(complaint);
    }

    // Client action: View all complaints of the logged-in user
    [Authorize(Roles = "Client")]
    public IActionResult MyComplaints()
    {
        var userId = _userManager.GetUserId(User);
        var complaints = _complaintRepository.GetAllWithIncludes()
            .Where(c => c.CustomerId == userId)  // Filter by logged-in user
            .Include(c => c.article)             // Include the related Article
            .Include(c => c.Customer)            // Include the Customer for email or other info
            .ToList();

        return View(complaints);
    }




    // Client action: Edit a complaint
    [Authorize(Roles = "Client")]
    public IActionResult Edit(int id)
    {
        var complaint = _complaintRepository.Get(id);
        if (complaint == null || complaint.CustomerId != _userManager.GetUserId(User))
        {
            return Unauthorized();
        }
        return View(complaint);
    }

    [HttpPost]
    [Authorize(Roles = "Client")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Complaint complaint)
    {
        if (complaint.CustomerId != _userManager.GetUserId(User))
        {
            return Unauthorized();
        }

        if (ModelState.IsValid)
        {
            _complaintRepository.Update(complaint);
            return RedirectToAction(nameof(MyComplaints));
        }
        return View(complaint);
    }

    // Client action: Delete a complaint
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Delete(int id)
    {
        var complaint = await _complaintRepository.GetAllWithIncludes()
            .Include(c => c.article)  // Include the related Article
            .Include(c => c.Customer) // If you need to include Customer as well
            .FirstOrDefaultAsync(c => c.Id == id);

        if (complaint == null)
        {
            return NotFound();
        }
        return View(complaint);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Client")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var complaint = _complaintRepository.Get(id);
        if (complaint == null || complaint.CustomerId != _userManager.GetUserId(User))
        {
            return Unauthorized();
        }
        _complaintRepository.Delete(id);
        return RedirectToAction(nameof(MyComplaints));
    }

    // Admin action: Create an Intervention for a Complaint
    [Authorize(Roles = "Admin")]
    public IActionResult CreateIntervention(int complaintId)
    {
        var complaint = _complaintRepository.Get(complaintId);
        if (complaint == null) return NotFound();

        var intervention = new Intervention { ComplaintId = complaintId };
        return View(intervention);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public IActionResult CreateIntervention(Intervention intervention)
    {
        try
        {
            _interventionRepository.Add(intervention);
            return RedirectToAction("Index", "Intervention");
        }
        catch
        {
            return View(intervention);
        }
    }
}
