using EasyGamesWeb.Views.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using System.Diagnostics.Eventing.Reader;

namespace EasyGamesWeb.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class ProductController : Controller
{
    private readonly IProductRepository _productRepo;
    private readonly IFileService _fileService;
    private readonly ICategoryRepository _categoryRepo;

    public ProductController(IProductRepository productRepo, IFileService fileService, ICategoryRepository categoryRepo)
    {
        _productRepo = productRepo;
        _fileService = fileService;
        _categoryRepo = categoryRepo;
    }


    public async Task<IActionResult> Index()
    {
        var product = await _productRepo.getProducts();
        return View(product);
    }

    public async Task<IActionResult> addProduct ()
    {
        var categorySelect = (await _categoryRepo.getCategories()).Select(category => new SelectListItem
        {
            Text = category.CategoryName,
            Value = category.Id.ToString(),
        });
        ProductsDTO productToAdd = new () { CategoryList = categorySelect };
        return View(productToAdd);
    }

    [HttpPost]

    public async Task <IActionResult> addProduct(ProductsDTO productToAdd)
    {
        var categorySelectList = (await _categoryRepo.getCategories()).Select(category => new SelectListItem
        {
            Text = category.CategoryName,
            Value = category.Id.ToString(),
        });

        productToAdd.CategoryList = categorySelectList;

        if (!ModelState.IsValid)
             return View(productToAdd);

            try
            { 
           
                if (productToAdd.ImageFile != null)
                {
                    if(productToAdd.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("Image file cannot be more than 1 MB");
                    }
                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(productToAdd.ImageFile, allowedExtensions);
                    productToAdd.Image = imageName;
                }
                Product product = new()
                {
                    Id = productToAdd.Id,
                    ProductName = productToAdd.ProductName,
                    Image = productToAdd.Image,
                    ShortDesc = productToAdd.ProductDescription,
                    CategoryId = productToAdd.CategoryId,
                    Price = productToAdd.Price,
                };
                await _productRepo.addProduct(product);
                TempData["SuccessMsg"] = "Successfully added";
                return Redirect(nameof(addProduct));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMsg"] = ex.Message;
                return View(productToAdd);
            }

            catch (FileNotFoundException ex)
            {
                TempData["ErrorMsg"] = ex.Message;
                return View(productToAdd);

            }

            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Error in saving data";
                return View(productToAdd);
            }

        }

    public async Task<IActionResult> updateProduct(int id)
    {
        var product = await _productRepo.getProductById(id);
        if (product == null)
        {
            TempData["ErrorMsg"] = $"Product with the id: {id} cannot be found";
            return RedirectToAction(nameof(Index));
        }

        var categorySelectList = (await _categoryRepo.getCategories()).Select(category => new SelectListItem
        {
            Text = category.CategoryName,
            Value = category.Id.ToString(),
            Selected = category.Id == product.CategoryId
        });

        ProductsDTO productToUpdate = new()
        {
            CategoryList = categorySelectList,
            ProductName = product.ProductName,
            CategoryId = product.CategoryId,
            ProductDescription = product.ShortDesc,
            Price = product.Price,
            Image = product.Image,
        };

        return View(productToUpdate);

    }

    [HttpPost]

    public async Task<IActionResult> updateProduct(ProductsDTO productToUpdate)
    {
        var categorySelectList = (await _categoryRepo.getCategories()).Select(category => new SelectListItem
        {
            Text = category.CategoryName,
            Value = category.Id.ToString(),
            Selected = category.Id == productToUpdate.CategoryId
        });

        productToUpdate.CategoryList = categorySelectList;

        if (!ModelState.IsValid)
            return View(productToUpdate);

        try
        {
            string oldImage = "";
            if (productToUpdate.ImageFile != null)
            {
                if (productToUpdate.ImageFile.Length > 1 * 1024 * 1024)
                {
                    throw new InvalidOperationException("Image file can not exceed 1 MB");
                }
                string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                string imageName = await _fileService.SaveFile(productToUpdate.ImageFile, allowedExtensions);
                // hold the old image name. Because we will delete this image after updating the new
                oldImage = productToUpdate.Image;
                productToUpdate.Image = imageName;
            }
            // manual mapping of BookDTO -> Book
            Product product = new()
            {
                Id = productToUpdate.Id,
                ProductName = productToUpdate.ProductName,
                CategoryId = productToUpdate.CategoryId,
                ShortDesc = productToUpdate.ProductDescription,
                Price = productToUpdate.Price,
                Image = productToUpdate.Image
            };
            await _productRepo.updateProduct(product);
            // if image is updated, then delete it from the folder too
            if (!string.IsNullOrWhiteSpace(oldImage))
            {
                _fileService.DeleteFile(oldImage);
            }
            TempData["SuccessMsg"] = "Book is updated successfully";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMsg"] = ex.Message;
            return View(productToUpdate);
        }
        catch (FileNotFoundException ex)
        {
            TempData["ErrorMsg"] = ex.Message;
            return View(productToUpdate);
        }
        catch (Exception ex)
        {
            TempData["ErrorMsg"] = "Error on saving data";
            return View(productToUpdate);
        }
    }

    public async Task<IActionResult> deleteProduct(int id)
    {
        try
        {
            var product = await _productRepo.getProductById(id);
            if (product == null)
            {
                TempData["ErrorMsg"] = $"Product with the id: {id} cannot be found";
            }
            else
            {
                await _productRepo.deleteProduct(product);
                if (!string.IsNullOrWhiteSpace(product.Image))
                {
                    _fileService.DeleteFile(product.Image);
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMsg"] = ex.Message;
        }

        catch (FileNotFoundException ex)
        {
            TempData["ErrorMsg"] = ex.Message;
        }

        catch (Exception ex)
        {
            TempData["ErrorMsg"] = "error in deleting hte data";
        }
        return RedirectToAction(nameof(Index));
    }



}


