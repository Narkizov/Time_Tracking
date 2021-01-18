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
using Time_Tracking.Interfaces;

namespace Time_Tracking.Controllers
{

    [Route("[controller]/[action]")]
    public class UserController : Controller
    {
        private ILogger _logger;

        private IUserRepository _usersGRUD;

        private IReportRepository _reportsGRUD;

        public UserController(ILogger<UserController> logger, IUserRepository usersGRUD, IReportRepository reportsGRUD)
        {
            _logger = logger;
            _reportsGRUD = reportsGRUD;
            _usersGRUD = usersGRUD;
        }

        /// <summary>
        /// Возвращает всех пользователей в системе
        /// </summary>
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
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
                _logger.ErrorMessage("При обращении к БД произошла ошибка", ex.Message, ex.StackTrace, DateTime.Now, ex.TargetSite.ReflectedType.DeclaringType.Name, ex.HResult);
                return BadRequest();
            }           

            IndexModelUserPerson usersAndPersons = new IndexModelUserPerson()
            {
                Reports = reports,
                Users = users
            };            

            return View(usersAndPersons);
        }

        /// <summary>
        /// Возвращает форму в виде представления для создания пользователя в системе
        /// </summary>
        /// <response code="200">Удачное выполнение запроса</response>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Создает пользователя в системе
        /// </summary>
        /// <param name="user">Создаваемый пользователь</param>
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromHeader] User user)
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
                    _logger.ErrorMessage("Не удалось создать пользователя", ex.Message, ex.StackTrace, DateTime.Now, ex.Source, ex.HResult);
                    return BadRequest();
                }
                
            }

            return View(user);

        }

        /// <summary>
        /// Возвращает форму в виде представления для редактирования информации о пользователе
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
        /// <response code="404">Ресурс не найден</response>
        [HttpGet]
        public async Task<IActionResult> Edit([FromQuery] int? id)
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
                _logger.ErrorMessage("Ошибка при возврате формы для редактирования пользоветелей", ex.Message, ex.StackTrace, DateTime.Now, ex.Source, ex.HResult);
                return BadRequest();
            }

            return NotFound();

        }

        /// <summary>
        /// Редактирует заданного пользователя
        /// </summary>
        /// <param name="user">Отредактированные данные</param>
        /// <param name="currentEmail">Старый email пользователя</param>
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
        [HttpPost]
        public async Task<IActionResult> Edit([FromHeader] User user, string currentEmail)
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
                    _logger.ErrorMessage("При редактировании пользователя возникла ошибка", ex.Message, ex.StackTrace, DateTime.Now, ex.Source, ex.HResult);
                    return BadRequest();
                }

                ModelState.AddModelError("EMail", "Данный email уже используется в системе");

            }

            return View(user);
        }

        /// <summary>
        /// Возвращает информацию об удаляемом пользователе в виде представления
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>        
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
        /// <response code="404">Ресурс не найден</response>
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete([FromQuery] int? id)
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
                    _logger.ErrorMessage("Ошибка при возврате формы для удаления пользователя", ex.Message, ex.StackTrace, DateTime.Now, ex.Source, ex.HResult);
                    return BadRequest();
                }

            }

            return NotFound();
        }

        /// <summary>
        /// Удаляет пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <response code="200">Удачное выполнение запроса</response>
        /// <response code="400">Ошибка при выполнения запроса</response>
        /// <response code="404">Ресурс не найден</response>
        [HttpPost]
        public async Task<IActionResult> Delete([FromQuery] int? id)
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
                    _logger.ErrorMessage("Ошибка при удалении пользователя", ex.Message, ex.StackTrace, DateTime.Now, ex.Source, ex.HResult);
                    return BadRequest();
                }
            }

            return NotFound();
        }

    }
}
