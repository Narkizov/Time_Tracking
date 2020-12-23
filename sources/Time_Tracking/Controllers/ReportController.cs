using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Time_Tracking.Models;
using Time_Tracking.Services;
using Time_Tracking.Logger;

namespace Time_Tracking.Controllers
{
    public class ReportController : Controller
    {
        private ILogger _logger;
        private UsersGRUD _usersGRUD;
        private ReportsGRUD _reportsGRUD;
        public ReportController(ILogger<ReportController> logger, UsersGRUD usersGRUD, ReportsGRUD reportsGRUD)
        {
            _logger = logger;
            _reportsGRUD = reportsGRUD;
            _usersGRUD = usersGRUD;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Users = new SelectList(await _usersGRUD.GetAllAsync(), "Id", "Email");
                ViewBag.DefaultUserEmail = null;
            }
            catch (Exception ex)
            {
                _logger.ErrorMessage(ex.Message);
                return BadRequest();
            }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Report report)
        {
            if (String.IsNullOrEmpty(report.Comment) && String.IsNullOrWhiteSpace(report.Comment))
                ModelState.AddModelError("Comment", "Поле примечание обязательно для заполнения.");
            else if (report.QuantityOfHours <= 0)
                ModelState.AddModelError("QuantityOfHours", "Поле количество часов не может быть меньше нуля или равняться ему.");
            else if ((report.Date != null) && (report.Date.Year < 2000))
                ModelState.AddModelError("Date", "Поле дата не должно быть меньше 2000 года");

            if (ModelState.IsValid)
            {
                try
                {
                    await _reportsGRUD.AddAsync(report);
                    await _reportsGRUD.SaveChanges();
                    _logger.InfoGrud(DateTime.Now, "Report", "Create", "Add", String.Format("{0} {1} {2}", report.Comment, report.Date, report.QuantityOfHours));
                    return RedirectToAction("Index", "User");
                }
                catch (Exception ex)
                {
                    _logger.ErrorMessage(ex.Message);
                    return BadRequest();
                }

            }

            List<User> users = null;
            try
            {
                users = await _usersGRUD.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.ErrorMessage(ex.Message);
                return BadRequest();
            }

            ViewBag.Users = new SelectList(users, "Id", "Email");
            ViewBag.DefaultUserEmail = users.FirstOrDefault(x => x.Id == report.UserId).Email;

            return View(report);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Report report = null;

                try
                {
                    report = await _reportsGRUD.FirstOrDefaultByIdAsync(id);

                    if (report != null)
                        return View(report);
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
        public async Task<IActionResult> Edit(Report report)
        {
            if (String.IsNullOrEmpty(report.Comment) && String.IsNullOrWhiteSpace(report.Comment))
                ModelState.AddModelError("Comment", "Поле примечание обязательно для заполнения.");
            else if (report.QuantityOfHours <= 0)
                ModelState.AddModelError("QuantityOfHours", "Поле количество часов не может быть меньше нуля или равняться ему.");
            else if ((report.Date != null) && (report.Date.Year < 2000))
                ModelState.AddModelError("Date", "Поле дата не должно быть меньше 2000 года");

            if (ModelState.IsValid)
            {
                try
                {
                    _reportsGRUD.Update(report);
                    await _reportsGRUD.SaveChanges();
                    _logger.InfoGrud(DateTime.Now, "Report", "Create", "Add", String.Format("{0} {1} {2}", report.Comment, report.Date, report.QuantityOfHours));

                    return RedirectToAction("Index", "User");
                }
                catch (Exception ex)
                {
                    _logger.ErrorMessage(ex.Message);
                    return BadRequest();
                }

            }

            return View(report);
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id != null)
            {
                Report report = null;

                try
                {
                    report = await _reportsGRUD.FirstOrDefaultByIdAsync(id);
                    
                    if (report != null)
                        return View(report);
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
                Report report = null;

                try
                {
                    report = await _reportsGRUD.FirstOrDefaultByIdAsync(id);

                    if (report != null)
                    {
                        _reportsGRUD.Remove(report);
                        await _reportsGRUD.SaveChanges();
                        _logger.InfoGrud(DateTime.Now, "Report", "Create", "Add", String.Format("{0} {1} {2}", report.Comment, report.Date, report.QuantityOfHours));
                        return RedirectToAction("Index", "User");
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

        [HttpGet]
        public async Task<IActionResult> ReportForMonth()
        {
            List<User> users = null;

            try
            {
                users = await _usersGRUD.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.ErrorMessage(ex.Message);
                return BadRequest();
            }

            return View(users);
        }

        [HttpGet]
        public async Task<JsonResult> GetReportsForMonthAndUser(int userId, int numberMonth)
        {
            List<Report> reports = null;

            try
            {
                reports = await _reportsGRUD.GetByExpression(userId, numberMonth);
            }
            catch (Exception ex)
            {
                _logger.ErrorMessage(ex.Message);
                return Json(null);
            }

            return Json(reports);
        }

    }
}
