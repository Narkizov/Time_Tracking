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
using Time_Tracking.Interfaces;

namespace Time_Tracking.Controllers
{
    [Route("[controller]/[action]")]
    public class ReportController : Controller
    {
        private ILogger _logger;

        private IUserRepository _usersGRUD;

        private IReportRepository _reportsGRUD;

        public ReportController(ILogger<ReportController> logger, IUserRepository usersGRUD, IReportRepository reportsGRUD)
        {
            _logger = logger;
            _reportsGRUD = reportsGRUD;
            _usersGRUD = usersGRUD;
        }

        /// <summary>
        /// Возвращает форму в виде представления для создания отчета
        /// </summary>
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
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

        /// <summary>
        /// Создает отчет о пользователе
        /// </summary>
        /// <param name="report">Информация об отчете</param>
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromHeader] Report report)
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

        /// <summary>
        /// Возвращает форму в виде представления для редактирования отчета
        /// </summary>
        /// <param name="id">Уникальный идентификатор отчета</param>
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
        /// <response code="404">Ресурс не найден</response>
        [HttpGet]
        public async Task<IActionResult> Edit([FromQuery] int? id)
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

        /// <summary>
        /// Редактирует отчет 
        /// </summary>
        /// <param name="report">Информация об отчете</param>
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
        [HttpPost]
        public async Task<IActionResult> Edit([FromHeader] Report report)
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

        /// <summary>
        /// Возвращает об удаляемой отчете в виде представления
        /// </summary>
        /// <param name="id">Уникальный идентификатор отчета</param>        
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
        /// <response code="404">Ресурс не найден</response>
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm([FromQuery] int? id)
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

        /// <summary>
        /// Удаляет отчет по идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор отчета</param>
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
        /// <response code="404">Ресурс не найден</response>
        [HttpPost]
        public async Task<IActionResult> Delete([FromQuery]  int? id)
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

        /// <summary>
        /// Возвращает всех пользователей в системе
        /// </summary>
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
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

        /// <summary>
        /// По заданному месяцу и идентификатору пользователя, возвращает все связанные с ним отчеты
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="numberMonth">Месяц (число от 1 до 12)</param>
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
        [HttpGet]
        public async Task<JsonResult> GetReportsForMonthAndUser([FromQuery] int userId, [FromQuery] int numberMonth)
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
