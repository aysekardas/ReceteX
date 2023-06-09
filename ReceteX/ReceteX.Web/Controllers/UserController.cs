using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        public IActionResult Index()
        {
            return View();
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
                AppUser usr = unitOfWork.Users.GetFirstOrDefault(u=>u.Email==user.Email&&u.Password==user.Password && u.isActive);
                
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
            return View();
        }
    }
}
