﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReceteX.Models;
using ReceteX.Repository.Shared.Abstract;
using System.Security.Claims;

namespace ReceteX.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }




        //Admin olarak girildiğinde tüm user'ları listeleyebileceğimiz action.

        [Authorize(Roles ="Admin")]

        public IActionResult GetAll()
        {
            return Json(new { data = unitOfWork.Users.GetAll() });

            //DataTables oluşabilmek için verilerin hepsinin DATA isimli bir obje içinde gelme kuralı koyuyor. O yuzden aşağıdaki gibi yeni bir anonim nesne oluşturup içerisine data diye bir field açıyoruz ve bütün datayı onun içine koyuyoruz. Bunu sırf DataTables için yapıyoruz.
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]

        public IActionResult GetById(Guid guid)
        {
            AppUser usr = unitOfWork.Users.GetFirstOrDefault(u=>u.Id == guid);
            return Json(usr);

        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public IActionResult Create(AppUser appUser)
        {
            unitOfWork.Users.Add(appUser);  
            unitOfWork.Save();
            return Ok();
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]

        public IActionResult Update(AppUser appUser)
        {
            unitOfWork.Users.Update(appUser);
            unitOfWork.Save();
            return Ok();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        //login'i ajax ile yapmayacağız
        public async Task<IActionResult> Login(AppUser user)
        {

            //user null değilse çalışsın
            if (user == null)
            {//IUnitOfWork ekledik
                AppUser usr = unitOfWork.Users.GetFirstOrDefault(u=>u.Email==user.Email&&u.Password==user.Password && u.isActive && !u.isDeleted);
                
                if (usr != null) 
                {

                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, usr.Name));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, usr.Id.ToString()));
                    if (usr.isAdmin)
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    else
                        claims.Add(new Claim(ClaimTypes.Role, "User"));
                    var claimsIdentiy = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignOutAsync
                        (CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync
                        (CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentiy));

                    return RedirectToAction("Index", "Home");   
                }

                else

                {
                    return View();
                }
            
            }
            else

            {
                return View();
            }
        }
    }
}
