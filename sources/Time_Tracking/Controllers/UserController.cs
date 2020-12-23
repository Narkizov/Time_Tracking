using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Time_Tracking.Models;
using Time_Tracking.Logger;
using Time_Tracking.ModelsView;
using Time_Tracking.Services;

namespace Time_Tracking.Controllers
{
    public class UserController : Controller
    {
        private ILogger _logger;
        private UsersGRUD _usersGRUD;
        private ReportsGRUD _reportsGRUD;
        public UserController(ILogger<UserController> logger, UsersGRUD usersGRUD, ReportsGRUD reportsGRUD)
        {
            _logger = logger;
            _reportsGRUD = reportsGRUD;
            _usersGRUD = usersGRUD;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            List<User> users = null;
            List<Report> reports = null;

            try
            {
                users = await _usersGRUD.GetAllAsync();
                reports = await _reportsGRUD.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.ErrorMessage(ex.Message);
                return BadRequest();
            }           

            IndexModelUserPerson usersAndPersons = new IndexModelUserPerson()
            {
                Reports = reports,
                Users = users
            };            

            return View(usersAndPersons);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (String.IsNullOrEmpty(user.Email) && String.IsNullOrWhiteSpace(user.Email))
                ModelState.AddModelError("Email", "Поле не может быть пустым.");
            else if (String.IsNullOrEmpty(user.FName) && String.IsNullOrWhiteSpace(user.FName))
                ModelState.AddModelError("FName", "Поле фамилия не может быть пустым.");
            else if (String.IsNullOrEmpty(user.MName) && String.IsNullOrWhiteSpace(user.MName))
                ModelState.AddModelError("MName", "Поле имя не может быть пустым.");

            if (ModelState.IsValid)
            {
                User isUniqueEmail = null; 

                try
                {
                    isUniqueEmail = await _usersGRUD.FirstOrDefaultByEmailAsync(user);

                    if (isUniqueEmail == null)
                    {
                        await _usersGRUD.AddAsync(user);
                        await _usersGRUD.SaveChanges();

                        _logger.InfoGrud(DateTime.Now, "User", "Create", "Add", String.Format("{0} {1} {2} {3}", user.Email, user.FName, user.MName, user.LName));

                        return RedirectToAction("Index");
                    }
                    else
                        ModelState.AddModelError("Email", "Поле email уже содержиться в базе данных.");
                }
                catch (Exception ex)
                {
                    _logger.ErrorMessage(ex.Message);
                    return BadRequest();
                }
                
            }

            return View(user);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                NotFound();

            User user = null;
            try
            {
                user = await _usersGRUD.FirstOrDefaultByIdAsync(id);

                if (user != null)                
                    return View(user);
                                 
            } 
            catch (Exception ex)
            {
                _logger.ErrorMessage(ex.Message);
                return BadRequest();
            }

            return NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> Edit(User user, string currentEmail)
        {
            if (String.IsNullOrEmpty(user.Email) && String.IsNullOrWhiteSpace(user.Email))
                ModelState.AddModelError("Email", "Поле EMail не может быть пустым.");
            else if (String.IsNullOrEmpty(user.FName) && String.IsNullOrWhiteSpace(user.FName))
                ModelState.AddModelError("FName", "Поле фамилия не может быть пустым.");
            else if (String.IsNullOrEmpty(user.MName) && String.IsNullOrWhiteSpace(user.MName))
                ModelState.AddModelError("MName", "Поле имя не может быть пустым.");

            if (ModelState.IsValid)
            {          
                User isUniqueEmail = null;

                try
                {
                    isUniqueEmail = await _usersGRUD.CheckUniqueEmail(user, currentEmail);

                    if (isUniqueEmail == null)
                    {
                        _usersGRUD.Update(user);
                        await _usersGRUD.SaveChanges();
                        _logger.InfoGrud(DateTime.Now, "User", "Edit", "Edit", String.Format("{0} {1} {2} {3}", user.Email, user.FName, user.MName, user.LName));
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorMessage(ex.Message);
                    return BadRequest();
                }

                ModelState.AddModelError("EMail", "Данный email уже используется в системе");

            }

            return View(user);
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                User user = null;

                try
                {
                    user = await _usersGRUD.FirstOrDefaultByIdAsync(id);

                    if (user != null)
                        return View(user);
                }
                catch (Exception ex)
                {
                    _logger.ErrorMessage(ex.Message);
                    return BadRequest();
                }

            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                User user = null;
                
                try
                {
                    user = await _usersGRUD.FirstOrDefaultByIdAsync(id);

                    if (user != null)
                    {
                        _usersGRUD.Remove(user);
                        await _usersGRUD.SaveChanges();
                        _logger.InfoGrud(DateTime.Now, "User", "Delete", "Delete", String.Format("{0} {1} {2} {3}", user.Email, user.FName, user.MName, user.LName));
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorMessage(ex.Message);
                    return BadRequest();
                }
            }

            return NotFound();
        }

    }
}
