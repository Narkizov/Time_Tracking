using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Time_Tracking.Models;

namespace Time_Tracking.Services
{
    public class UsersGRUD
    {
        private TrackingDbContext _db;

        public UsersGRUD(TrackingDbContext context)
        {
            _db = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _db.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User> FirstOrDefaultByEmailAsync(User user)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
        }

        public async Task<User> FirstOrDefaultByIdAsync(int? id)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Id == id.Value);
        }

        public async Task<User> CheckUniqueEmail(User user, string currentEmail)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Email != currentEmail && x.Email == user.Email);
        }

        public void Remove(User user)
        {
            _db.Users.Remove(user);
        }

        public void Update(User user)
        {
            _db.Users.Update(user);
        }

        public async Task AddAsync(User user)
        {
            await _db.Users.AddAsync(user);
        }

        public async Task SaveChanges()
        {
            await _db.SaveChangesAsync();
        }

    }
}
