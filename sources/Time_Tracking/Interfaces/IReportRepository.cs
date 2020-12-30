using System.Collections.Generic;
using System.Threading.Tasks;
using Time_Tracking.Models;

namespace Time_Tracking.Interfaces
{
    public interface IReportRepository
    {
        public Task<List<Report>> GetAllAsync();

        public Task<Report> FirstOrDefaultByIdAsync(int? id);

        public Task<List<Report>> GetByExpression(int userId, int numberMonth);

        public void Remove(Report report);

        public void Update(Report report);

        public Task AddAsync(Report report);

        public  Task SaveChanges();

    }
}
