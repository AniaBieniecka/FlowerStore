﻿using FlowerStore.DataAccess.Repository;
using FlowerStore.DataAccess.Repository.IRepository;
using FlowerStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace FlowerStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");

            return View(productList);
        }

        public IActionResult Details(int productID)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.Product.Get(u=>u.ProductID== productID, includeProperties: "Category"),
                Count =1,
                ProductID = productID
            };
                
            //Product product = _unitOfWork.Product.Get(u=>u.ProductID== productID, includeProperties: "Category");

            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserID=userID;

            ShoppingCart cartFromDB = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserID == userID &&
            u.ProductID == shoppingCart.ProductID);

            if (cartFromDB != null)
            {
                cartFromDB.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDB);
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);

            }
            TempData["success"] = "Cart updated successfully";
                _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}