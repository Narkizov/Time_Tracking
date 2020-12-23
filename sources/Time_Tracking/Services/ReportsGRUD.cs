using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Time_Tracking.Models;

namespace Time_Tracking.Services
{
    public class ReportsGRUD
    {
        private TrackingDbContext _db;

        public ReportsGRUD(TrackingDbContext context)
        {
            _db = context;
        }

        public async Task<List<Report>> GetAllAsync()
        {
            return await _db.Reports.Include(x => x.User).AsNoTracking().ToListAsync();
        }

        public async Task<Report> FirstOrDefaultByIdAsync(int? id)
        {
            return await _db.Reports.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Report>> GetByExpression(int userId, int numberMonth)
        {
            return await _db.Reports.Where(x => x.UserId == userId && x.Date.Month == numberMonth).ToListAsync();
        }

        public void Remove(Report report)
        {
            _db.Reports.Remove(report);
        }

        public void Update(Report report)
        {
            _db.Reports.Update(report);
        }

        public async Task AddAsync(Report report)
        {
            await _db.Reports.AddAsync(report);
        }

        public async Task SaveChanges()
        {
            await _db.SaveChangesAsync();
        }

    }
}
