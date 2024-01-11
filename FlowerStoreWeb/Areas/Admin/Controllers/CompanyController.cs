using FlowerStore.DataAccess.Data;
using FlowerStore.DataAccess.Repository.IRepository;
using FlowerStore.Models;
using FlowerStore.Models.ViewModels;
using FlowerStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlowerStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();

            return View(objCompanyList);
        }
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            //{
            //    Text = u.Name,
            //    Value = u.CategoryID.ToString(),
            //});
            //ViewBag.CategoryList = CategoryList;


            if (id==null || id==0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //update
                Company companyobj = _unitOfWork.Company.Get(u => u.CompanyID == id);
                return View(companyobj);
            }

        }
        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {

            if (ModelState.IsValid)
            {
               
                if(CompanyObj.CompanyID == 0)
                {
                    _unitOfWork.Company.Add(CompanyObj);
                }
                else
                {
                    _unitOfWork.Company.Update(CompanyObj);
                }
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }

            else
            {


                return View(CompanyObj);
            }
        }

        //public IActionResult Edit(int id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Company? CompanyFromDB = _unitOfWork.Company.Get(u => u.CompanyID == id);
        //    if (CompanyFromDB == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(CompanyFromDB);
        //}
        //[HttpPost]
        //public IActionResult Edit(Company obj)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Company.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Category updated successfully";
        //        return RedirectToAction("Index");
        //    }

        //    return View();
        //}



        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new {data=objCompanyList});
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted =_unitOfWork.Company.Get(u=>u.CompanyID== id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            
            _unitOfWork.Company.Remove(CompanyToBeDeleted); 
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion

    }
}
