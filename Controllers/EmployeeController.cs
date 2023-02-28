using AutoMapper;
using Demo.BL.Interface;
using Demo.BL.Models;
using Demo.BL.Repository;
using Demo.DAL.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Demo.BL.Helper;
using Microsoft.AspNetCore.Authorization;

namespace Demo.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        #region Fiels

        // Loosly Coupled
        private readonly IEmployeeRep Employee;
        private readonly IDepartmentRep department;
        private readonly IMapper mapper;

        // Tightly Coupled
        //EmployeeRep Employee ;

        #endregion


        #region Ctor

        public EmployeeController(IEmployeeRep Employee,IDepartmentRep department, IMapper mapper)
        {
            this.Employee = Employee;
            this.department = department;
            this.mapper = mapper;
        }

        #endregion


        #region Actions


        public IActionResult Index()
        {
            #region SearchByName

            //if (searchvalue == "")
            //{
            //    var data = Employee.Get();
            //    var model = mapper.Map<IEnumerable<EmployeeVM>>(data);
            //    return View(model);
            //}
            //else
            //{
            //    var data = Employee.Search(searchvalue);
            //    var model = mapper.Map<IEnumerable<EmployeeVM>>(data);
            //    return View(model);
            //}
            #endregion
            var data = Employee.Get();
            var model = mapper.Map<IEnumerable<EmployeeVM>>(data);
            if (User.IsInRole("Admin"))
            {
                return View(model);
            }
            else
            {
                return View("Index2", model);
            }
           
        }
        public IActionResult Index2()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var data = Employee.GetById(id);
            var model = mapper.Map<EmployeeVM>(data);
            ViewBag.DL = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
            if (User.IsInRole("Admin"))
            {
                return View(model);
            }
            else
            {
                return View("Details2",model);
            } 
        }
        public IActionResult Details2(int id)
        {
            return View();
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.DL = new SelectList(department.Get(), "Id", "Name");
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(EmployeeVM model)
        {

            try
            {

                if (ModelState.IsValid)
                {
                    model.PhotoName = FileUploader.Upload("/wwwroot/Files/Imgs", model.Img);
                    model.CvName = FileUploader.Upload("/wwwroot/Files/Docs", model.CV);
                    var data = mapper.Map<Employee>(model);
                    Employee.Create(data);
                    return RedirectToAction("Index");
                }
                ViewBag.DL = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.DL = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = Employee.GetById(id);
            var model = mapper.Map<EmployeeVM>(data);
            ViewBag.DL = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(EmployeeVM model)
        {

            try
            {

                if (ModelState.IsValid)
                {                   
                    model.PhotoName = FileUploader.Upload("/wwwroot/Files/Imgs", model.Img);
                    model.CvName = FileUploader.Upload("/wwwroot/Files/Docs", model.CV);
                    var data = mapper.Map<Employee>(model);
                    Employee.Edit(data);
                    ViewBag.DL = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
                    return RedirectToAction("Index");
                }
                ViewBag.DL = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.DL = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
                return View(model);
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var data = Employee.GetById(id);
            var model = mapper.Map<EmployeeVM>(data);
            ViewBag.DL = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(EmployeeVM model)
        {

            try
            {
                var data = Employee.GetById(model.Id);
                FileUploader.FileRemover("/wwwroot/Files/Imgs", data.PhotoName);
                FileUploader.FileRemover("/wwwroot/Files/Docs", data.CvName);
                Employee.Delete(data);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.DL = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
                return View(model);
            }
        }

        #endregion
        #region CITY/District
        //[HttpPost]
        //public JsonResult GetCityByCountryId(int ctrId)
        //{
        //    var data = city.Get(a => a.CountryId == ctrId);
        //    var model = mapper.Map<IEnumerable<CityVM>>(data);
        //    return Json(model);

        //}
        //[HttpPost]
        //public JsonResult GetDistrictByCityId(int CitId)
        //{
        //    var data = district.Get(a => a.CityId == CitId);
        //    var model = mapper.Map<IEnumerable<DistrictVM>>(data);
        //    return Json(model);

        //}
        #endregion

    }
}
