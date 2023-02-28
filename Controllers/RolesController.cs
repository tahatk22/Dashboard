using Demo.BL.Models;
using Demo.DAL.Extend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public RolesController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            var Roles = roleManager.Roles;
            return View(Roles);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole model)
        {
            var role = new IdentityRole()
            {
                Name = model.Name,
                NormalizedName = model.Name.ToUpper()
            };
            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
               return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Edit(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IdentityRole model)
        {

            var role = await roleManager.FindByIdAsync(model.Id);

            role.Name = model.Name;
            role.NormalizedName = model.Name.ToUpper();

            var result = await roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> AddOrRemoveUsers (string RoleId)
        {
            ViewBag.RoleId = RoleId;
            var Role = await roleManager.FindByIdAsync(RoleId);
            var Model = new List<UserInRole>();
            foreach (var User in userManager.Users)
            {
                var UserInRole = new UserInRole()
                {
                    UserId = User.Id,
                    UserName = User.UserName
                };
                if ( await userManager.IsInRoleAsync(User,Role.Name))
                {
                    UserInRole.IsSelected = true;
                }
                else
                {
                    UserInRole.IsSelected = false;
                }
                Model.Add(UserInRole);
            }
            return View(Model);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(List<UserInRole> model,string RoleId)
        {
            var Role = await roleManager.FindByIdAsync(RoleId);
            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if (model[i].IsSelected && !( await userManager.IsInRoleAsync(user,Role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, Role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, Role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, Role.Name);
                }
                if (i < model.Count)
                    continue;
            }
            return RedirectToAction("Edit", new { id = RoleId });
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            return View(role);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(IdentityRole model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            var result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }

        }
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            return View(role);
        }
    }
}
