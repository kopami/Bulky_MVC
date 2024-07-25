using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;        
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() //for DataTable
        {
            List<ApplicationUser> userList = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(ur => ur.UserId == user.Id)!.RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId)!.Name;
                if (user.Company is null)
                {
                    user.Company = new() { Name = "" };
                }
            }
            return Json(new { data = userList });
        }

        [HttpPost]                
        public IActionResult LockUnlock([FromBody]string id)
        {
            var obj = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            string message;
            if (obj is null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (obj is not null && obj.LockoutEnd > DateTime.Now)
            {
                //currently locked => need to unlock
                obj.LockoutEnd = DateTime.Now;
                message = "Unlock Successful";
            }
            else
            {
                obj!.LockoutEnd = DateTime.Now.AddYears(1000);
                message = "Lock Successful";
            }
            _db.SaveChanges();

            return Json(new { success = true, message });
        }
        #endregion
    }
}
