using FlowerStore.DataAccess.Repository.IRepository;
using FlowerStore.Models;
using FlowerStore.Models.ViewModels;
using FlowerStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlowerStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == userID,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = cart.Product.Price;
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == userID,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u=>u.Id== userID);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAdress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = cart.Product.Price;
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == userID,
                includeProperties: "Product");
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userID;

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userID);


			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = cart.Product.Price;
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

			}

            if (applicationUser.CompanyID.GetValueOrDefault() == 0)
            {
                //it is a regular customer account
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
				//it is a company user
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
			}

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductID,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.OrderHeaderId,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }


			if (applicationUser.CompanyID.GetValueOrDefault() == 0)
			{
				//it is a regular customer account and we need to capture payment
                //stripe logic
				//ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
				//ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
			}

			return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.OrderHeaderId });
		}

        public IActionResult OrderConfirmation (int id)
        {
            return View(id);
        }
		public IActionResult Plus(int cartId)
        {
            var cartFromDB = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId);
            cartFromDB.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDB);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDB = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId);
            if (cartFromDB.Count == 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDB);
            }
            else
            {
                cartFromDB.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDB);
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDB = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId);
            _unitOfWork.ShoppingCart.Remove(cartFromDB);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
