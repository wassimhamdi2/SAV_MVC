using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniProjet.Models;
using MiniProjet.Models.Repository;
using MiniProjet.ViewModels;

[Authorize(Roles = "Admin")]
public class ArticleController : Controller
{
    private readonly IRepository<Article> _articleRepository;
    private readonly IWebHostEnvironment hostingEnvironment;


    public ArticleController(IRepository<Article> articleRepository, IWebHostEnvironment hostingEnvironment)
    {
        _articleRepository = articleRepository;
        this.hostingEnvironment = hostingEnvironment;

    }

    // GET: Articles
    public IActionResult Index()
    {
        // Retrieve all articles
        var articles = _articleRepository.GetAll();
        return View(articles);
    }

    // GET: Create Article
    public IActionResult Create()
    {
        ViewBag.Articles = _articleRepository.GetAll().ToList(); // Get all Articles


        return View();
    }

    // POST: Create Article
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            string uniqueFileName = null;
            // If the Photo property on the incoming model object is not null, then the user has selected an image to upload.
            if (model.ImagePath != null)
            {
                // The image must be uploaded to the images folder in wwwroot
                // To get the path of the wwwroot folder we are using the inject
                // HostingEnvironment service provided by ASP.NET Core
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                // To make sure the file name is unique we are appending a new
                // GUID value and an underscore to the file name
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                // Use CopyTo() method provided by IFormFile interface to
                // copy the file to wwwroot/images folder
                model.ImagePath.CopyTo(new FileStream(filePath, FileMode.Create));
            }
            Article article = new Article
            {
                Désignation = model.Désignation,
                Prix = model.Prix,
                // Store the file name in PhotoPath property of the employee object
                // which gets saved to the Employees database table
                Image = uniqueFileName
            };
            _articleRepository.Add(article); // Add article to the repository

            return RedirectToAction(nameof(Index));
        }
        return View(); // Return to the Create view if validation fails
    }

    // GET: Edit Article
    public IActionResult Edit(int id)
    {
        // Retrieve the article to edit
        var article = _articleRepository.Get(id);
        EditViewModel articleEditViewModel = new EditViewModel
        {
            Id = article.Id,
            Désignation = article.Désignation,
            Prix = article.Prix,
            ExistingImagePath = article.Image
        }; return View(articleEditViewModel);
    }

    // POST: Edit Article
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(EditViewModel model)
    {
        // Check if the provided data is valid, if not rerender the edit view
        // so the user can correct and resubmit the edit form
        if (ModelState.IsValid)
        {
            // Retrieve the product being edited from the database
            Article product = _articleRepository.Get(model.Id);
            // Update the product object with the data in the model object
            product.Désignation = model.Désignation;
            product.Prix = model.Prix;
            // If the user wants to change the photo, a new photo will be
            // uploaded and the Photo property on the model object receives
            // the uploaded photo. If the Photo property is null, user did
            // not upload a new photo and keeps his existing photo
            if (model.ImagePath != null)
            {
                // If a new photo is uploaded, the existing photo must be
                // deleted. So check if there is an existing photo and delete
                if (model.ExistingImagePath != null)
                {
                    string filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingImagePath);
                    System.IO.File.Delete(filePath);
                }
                // Save the new photo in wwwroot/images folder and update
                // PhotoPath property of the product object which will be
                // eventually saved in the database
                product.Image = ProcessUploadedFile(model);
            }
            // Call update method on the repository service passing it the
            // product object to update the data in the database table
            Article updatedProduct = _articleRepository.Update(product);
            if (updatedProduct != null)
                return RedirectToAction("Index");
            else
                return NotFound();
        }
        return View(model);
    }
    public ActionResult Details(int id)
    {
        var product = _articleRepository.Get(id);

        return View(product);
    }

    [NonAction]
    private string ProcessUploadedFile(EditViewModel model)
    {
        string uniqueFileName = null;
        if (model.ImagePath != null)
        {
            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.ImagePath.CopyTo(fileStream);
            }
        }
        return uniqueFileName;
    }
    // GET: Delete Article
    public IActionResult Delete(int id)
    {
        // Retrieve the article to delete
        var article = _articleRepository.Get(id);
        if (article == null) return NotFound();
        return View(article);
    }

    // POST: Delete Article
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _articleRepository.Delete(id); // Delete the article from the repository
        return RedirectToAction(nameof(Index));
             }
            catch
            {
                return View();
            }
    }
}
