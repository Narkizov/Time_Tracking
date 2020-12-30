using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Time_Tracking.Interfaces;
using Time_Tracking.Models;
using Time_Tracking.Services;

namespace Time_Tracking.Tests
{
    [TestFixture]
    public class ReportGrudTests
    {

        private DbContextOptions<TrackingDbContext> CreateOptionBuilder()
        {
            var builder = new DbContextOptionsBuilder<TrackingDbContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            return builder.Options;
        }

        private void FillContext(DbContextOptions<TrackingDbContext> options)
        {
            using (var context = new TrackingDbContext(options))
            {

                context.Reports.AddRange(new Report[]
                {
                    new Report { Id = 1, Comment = "Comment 1", QuantityOfHours = 12, Date = new DateTime(2020, 12, 1), UserId = 1 },
                    new Report { Id = 2, Comment = "Comment 2", QuantityOfHours = 19, Date = new DateTime(2020, 12, 2), UserId = 1 },
                    new Report { Id = 3, Comment = "Comment 3", QuantityOfHours = 98, Date = new DateTime(2020, 12, 3), UserId = 1 },
                    new Report { Id = 4, Comment = "Comment 4", QuantityOfHours = 21, Date = new DateTime(2020, 12, 3), UserId = 1 },
                    new Report { Id = 5, Comment = "Comment 5", QuantityOfHours = 23, Date = new DateTime(2020, 12, 4), UserId = 1 }
                });

                context.Users.AddRange(new User[]
                {
                    new User { Email = "test@mail.ru", FName = "Alex", LName = "Jey", MName = "Merfy" }
                });

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Возвращает ли метод GetAll класса ReportsGRUD все Report
        /// </summary>
        [Test, TestCase(5)]
        public async Task GetAllAsync_ReturnAllReport_True(int countReport)
        {
            // Arrange
            DbContextOptions<TrackingDbContext> options = this.CreateOptionBuilder();
            FillContext(options);

            using (var context = new TrackingDbContext(options))
            {

                IReportRepository repo = new ReportsGRUD(context);

                // Act
                List<Report> allUsers = await repo.GetAllAsync();
                          
                // Assert
                Assert.AreEqual(countReport, allUsers.Count);
            }
        }

        /// <summary>
        /// Возвращет ли метод FirstOrDefaultByIdAsync объект с заданным Id
        /// </summary>
        [Test, TestCase(1), TestCase(2), TestCase(3), TestCase(4), TestCase(5)]        
        public async Task FirstOrDefaultByIdAsync_FindById_IsNotNull(int id)
        {
            // Arrange
            DbContextOptions<TrackingDbContext> options = this.CreateOptionBuilder();
            FillContext(options);

            using (var context = new TrackingDbContext(options))
            {

                IReportRepository repo = new ReportsGRUD(context);

                // Act
                Report report = await repo.FirstOrDefaultByIdAsync(id);

                // Assert
                Assert.AreEqual(id, report.Id);
            }
        }

        /// <summary>
        /// Возвращает ли метод FirstOrDefaultByIdAsync значение null если не находит отчет
        /// </summary>
        [Test, TestCase(2341), TestCase(1239094)]
        public async Task FirstOrDefaultByIdAsync_FindById_IsNull(int id)
        {

            // Arrange
            DbContextOptions<TrackingDbContext> options = this.CreateOptionBuilder();
            FillContext(options);

            using (var context = new TrackingDbContext(options))
            {

                IReportRepository repo = new ReportsGRUD(context);

                // Act
                var notFound = await repo.FirstOrDefaultByIdAsync(id);

                // Assert
                Assert.IsNull(notFound);
            }
        }

        /// <summary>
        /// Возвращает ли метод GetByExpression по заданному пользователю и месяцу рапорта
        /// </summary>
        [Test, TestCase(1, 12, ExpectedResult = 5), TestCase(2, 12, ExpectedResult = 0)]
        public async Task<int> GetByExpression_GetAllReportForUserForMounth_Success(int userId, int monthNumber)
        {
            // Arrange
            DbContextOptions<TrackingDbContext> options = this.CreateOptionBuilder();
            FillContext(options);

            using (var context = new TrackingDbContext(options))
            {

                IReportRepository repo = new ReportsGRUD(context);

                // Act
                var notFound = await repo.GetByExpression(userId, monthNumber);

                // Assert
                return notFound.Count;
            }
        }

        /// <summary>
        /// Проверяет, добавляются ли отчеты
        /// </summary>
        [Test, TestCase(6)]
        public async Task AddAsync_AddInDb_Success(int id)
        {
            // Arrange
            DbContextOptions<TrackingDbContext> options = this.CreateOptionBuilder();
            FillContext(options);

            // Act
            using (var contextAdd = new TrackingDbContext(options))
            {

                contextAdd.Users.AddRange(
                    new User { 
                        Email = "test@mail.ru", 
                        FName = "Alex", 
                        LName = "Jey", 
                        MName = "Merfy"                 
                    });

                contextAdd.SaveChanges();
            }

            using (var contextAdd = new TrackingDbContext(options))
            {

                IReportRepository repo = new ReportsGRUD(contextAdd);
                Report report = new Report()
                {
                    Comment = "Comment",
                    QuantityOfHours = 12,
                    Date = new DateTime(2019, 11, 21),
                    UserId = 0
                };

                await repo.AddAsync(report);
                contextAdd.SaveChanges();

                int result = contextAdd.Reports.Count();

                // Assert
                Assert.AreEqual(id, result);
            }
        }

        /// <summary>
        /// Проверет, отредактирован ли отчет
        /// </summary>
        [Test, TestCase(1), TestCase(2), TestCase(3)]
        public async Task Update_EditReport_Success(int id)
        {

            // Arrange
            DbContextOptions<TrackingDbContext> options = this.CreateOptionBuilder();
            FillContext(options);

            using (var context = new TrackingDbContext(options))
            {

                IReportRepository repo = new ReportsGRUD(context);

                // Act
                Report report = await repo.FirstOrDefaultByIdAsync(id);
                report.Comment = "Change comment";
                repo.Update(report);

                Report reportEdit = await repo.FirstOrDefaultByIdAsync(id);

                // Assert
                Assert.AreEqual(report.Comment, reportEdit.Comment);
            }
        }

        /// <summary>
        /// Проверка на удаление объекта
        /// </summary>
        [Test]
        public async Task Remove_RemoveReport_Success()
        {
            // Arrange
            DbContextOptions<TrackingDbContext> options = this.CreateOptionBuilder();
            FillContext(options);

            using (var context = new TrackingDbContext(options))
            {

                IReportRepository repo = new ReportsGRUD(context);

                // Act
                Report report = context.Reports.First();
                repo.Remove(report);
                context.SaveChanges();
                var result = await repo.FirstOrDefaultByIdAsync(report.Id);

                // Assert
                Assert.IsNull(result);
            }
        }

        /// <summary>
        /// Проверка на сохранение изменений
        /// </summary>
        [Test]
        public async Task SaveChanges_SaveUsers_Success()
        {

            // Arrange
            DbContextOptions<TrackingDbContext> options = this.CreateOptionBuilder();
            FillContext(options);

            Report report = new Report()
            {
                Comment = "New1",
                QuantityOfHours = 12,
                Date = DateTime.Now,
                UserId = 1
            };

            int countRecordBeforeInsert = 0;
            int countRecordAfterInsert = 0;

            // Act
            using (var context = new TrackingDbContext(options))
            {
                countRecordBeforeInsert = context.Reports.Count();
                IReportRepository repo = new ReportsGRUD(context);
                                
                await repo.AddAsync(report);
                await repo.SaveChanges();

                countRecordAfterInsert = context.Reports.Count();
            }

            countRecordBeforeInsert++;

            // Assert
            Assert.AreEqual(countRecordBeforeInsert, countRecordAfterInsert);
        }

    }
}
