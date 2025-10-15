using EasyGamesWeb.Models;                 // Product, Category
using EasyGamesWeb.Models.DTOs;            // ProductsDTO
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EasyGamesWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly FileService _fileService;           
        private readonly ICategoryRepository _categoryRepo;

        public ProductController(
            IProductRepository productRepo,
            FileService fileService,
            ICategoryRepository categoryRepo)
        {
            _productRepo = productRepo;
            _fileService = fileService;
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepo.getProducts();
            return View(products);
        }

        public async Task<IActionResult> addProduct()
        {
            var categorySelect = (await _categoryRepo.getCategories()).Select(category => new SelectListItem
            {
                Text = category.CategoryName,
                Value = category.Id.ToString(),
            });

            var vm = new ProductsDTO
            {
                CategoryList = categorySelect
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> addProduct(ProductsDTO productToAdd)
        {
            
            productToAdd.CategoryList = (await _categoryRepo.getCategories()).Select(category => new SelectListItem
            {
                Text = category.CategoryName,
                Value = category.Id.ToString(),
            });

            if (!ModelState.IsValid)
                return View(productToAdd);

            try
            {
                if (productToAdd.ImageFile != null && productToAdd.ImageFile.Length > 0)
                {
                    if (productToAdd.ImageFile.Length > 1 * 1024 * 1024)
                        throw new InvalidOperationException("Image file cannot be more than 1 MB");

                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(productToAdd.ImageFile, allowedExtensions);
                    productToAdd.Image = imageName;
                }

                
                if (productToAdd.SellPrice > 0 && productToAdd.Price == 0)
                    productToAdd.Price = productToAdd.SellPrice;

                var product = new Product
                {
                    Id = productToAdd.Id,
                    ProductName = productToAdd.ProductName,
                    CategoryId = productToAdd.CategoryId,
                    ShortDesc = productToAdd.ProductDescription,
                    Image = productToAdd.Image,
                    Price = productToAdd.Price, 
                    
                    Source = productToAdd.Source,
                    BuyPrice = productToAdd.BuyPrice,
                    SellPrice = productToAdd.SellPrice > 0 ? productToAdd.SellPrice : productToAdd.Price,
                    Sku = productToAdd.Sku,
                    IsActive = productToAdd.IsActive
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
            catch
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

            var vm = new ProductsDTO
            {
                Id = product.Id,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                CategoryList = categorySelectList,
                ProductDescription = product.ShortDesc,
                Image = product.Image,
                
                Price = product.Price,
                BuyPrice = product.BuyPrice,
                SellPrice = product.SellPrice,
                Source = product.Source,
                Sku = product.Sku,
                IsActive = product.IsActive
            };

            if (vm.SellPrice == 0 && vm.Price > 0)
                vm.SellPrice = vm.Price;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> updateProduct(ProductsDTO productToUpdate)
        {
            
            productToUpdate.CategoryList = (await _categoryRepo.getCategories()).Select(category => new SelectListItem
            {
                Text = category.CategoryName,
                Value = category.Id.ToString(),
                Selected = category.Id == productToUpdate.CategoryId
            });

            if (!ModelState.IsValid)
                return View(productToUpdate);

            try
            {
                string? oldImage = null;

                if (productToUpdate.ImageFile != null && productToUpdate.ImageFile.Length > 0)
                {
                    if (productToUpdate.ImageFile.Length > 1 * 1024 * 1024)
                        throw new InvalidOperationException("Image file can not exceed 1 MB");

                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(productToUpdate.ImageFile, allowedExtensions);

                    oldImage = productToUpdate.Image; 
                    productToUpdate.Image = imageName;
                }

                if (productToUpdate.SellPrice > 0 && productToUpdate.Price == 0)
                    productToUpdate.Price = productToUpdate.SellPrice;

                var product = new Product
                {
                    Id = productToUpdate.Id,
                    ProductName = productToUpdate.ProductName,
                    CategoryId = productToUpdate.CategoryId,
                    ShortDesc = productToUpdate.ProductDescription,
                    Image = productToUpdate.Image,
                    Price = productToUpdate.Price,
                    
                    Source = productToUpdate.Source,
                    BuyPrice = productToUpdate.BuyPrice,
                    SellPrice = productToUpdate.SellPrice > 0 ? productToUpdate.SellPrice : productToUpdate.Price,
                    Sku = productToUpdate.Sku,
                    IsActive = productToUpdate.IsActive
                };

                await _productRepo.updateProduct(product);

                if (!string.IsNullOrWhiteSpace(oldImage))
                    _fileService.DeleteFile(oldImage);

                TempData["SuccessMsg"] = "Product is updated successfully";
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
            catch
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
                        _fileService.DeleteFile(product.Image);
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
            catch
            {
                TempData["ErrorMsg"] = "error in deleting the data";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
