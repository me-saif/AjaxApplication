using AjaxApplication.Models;
using AjaxApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AjaxApplication.Controllers
{
    public class PicUploadController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly TransactionDbContext _context;

        public PicUploadController(IWebHostEnvironment hostingEnvironment, TransactionDbContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.PicUploads.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);

                PicUploadModel picUploadModel = new PicUploadModel
                {
                    Name = model.Name,
                    Email = model.Email,
                    PhotoPath = uniqueFileName
                };

                _context.PicUploads.Add(picUploadModel);
                _context.SaveChanges();
                return RedirectToAction("details", new { id = picUploadModel.Id });
            }

            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            PicUploadModel picUpload = _context.PicUploads.Find(id);
            EditViewModel editViewModel = new EditViewModel
            {
                Id = picUpload.Id,
                Name = picUpload.Name,
                Email = picUpload.Email,
                ExistingPhotoPath = picUpload.PhotoPath
            };
            return View(editViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                PicUploadModel picUpload = _context.PicUploads.Find(model.Id);
                picUpload.Name = model.Name;
                picUpload.Email = model.Email;
                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                            "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    picUpload.PhotoPath = ProcessUploadedFile(model);
                }

                _context.PicUploads.Update(picUpload);
                _context.SaveChanges();
                return RedirectToAction("index");
            }

            return View();
        }

        [AllowAnonymous]
        public ViewResult Details(int id)
        {
            int picId = id;

            PicUploadModel picUploads = _context.PicUploads.Find(picId);

            if (picUploads == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", picId);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                PicUploadModel = picUploads,
                PageTitle = "Employee Details"
            };

            return View(homeDetailsViewModel);
        }

        private string ProcessUploadedFile(CreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}