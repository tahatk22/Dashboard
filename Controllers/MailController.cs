using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.BL.Models;
using System.Net.Mail;
using System.Net;
using Demo.BL.Helper;
using Microsoft.AspNetCore.Authorization;

namespace Demo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MailController : Controller
    {
        //مش شغالة
       [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(MailVM model)
        {
            try
            {
                 MailSender.SendMail(model);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {

                return View(model);
            }
        }
    }
}
