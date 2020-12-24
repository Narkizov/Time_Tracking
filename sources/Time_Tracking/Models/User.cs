using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Time_Tracking.Models
{
    public class User
    {
        /// <summary>
        /// Уникальный идентификатор пользователя
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Email пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле EMail обязательно для заполнения.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        [StringLength(100, ErrorMessage = "Длина строки не должна превышать 100 символов")]
        public string Email { get; set; }

        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле фамилия обязательно для заполнения.")]
        [StringLength(35, ErrorMessage = "Длина строки не должна превышать 35 символов и быть меньше 3 символов")]
        public string FName { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле имя обязательно для заполнения.")]
        [StringLength(35, ErrorMessage = "Длина строки не должна превышать 35 символов и быть меньше 3 символов")]
        public string MName { get; set; }

        /// <summary>
        /// Отчество пользователя
        /// </summary>
        [StringLength(35, ErrorMessage = "Длина строки не должна превышать 35 символов")]
        public string LName { get; set; }


        public List<Report> Reports { get; set; }
    }
}
