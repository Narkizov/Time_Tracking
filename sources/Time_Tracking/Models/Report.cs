using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Time_Tracking.Models
{
    public class Report
    {
        /// <summary>
        /// Уникальный идентификатор отчета
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Комментарий к отчету
        /// </summary>
        [Required(ErrorMessage = "Поле примечание обязательно для заполнения.")]
        [StringLength(1000, ErrorMessage = "Поле примечание не может быть больше 1000 символов")]
        public string Comment { get; set; }

        /// <summary>
        /// Количество отработанных часов
        /// </summary>
        [Required(ErrorMessage = "Поле количество часов обязательно для заполнения.")]
        public int QuantityOfHours { get; set; }

        /// <summary>
        /// Дата написания отчета
        /// </summary>
        [Required(ErrorMessage = "Поле дата обязательно для заполнения.")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Уникальный идентификатор пользователя
        /// </summary>
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
