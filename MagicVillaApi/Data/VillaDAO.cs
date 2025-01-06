using MagicVillaApi.Models;
using MagicVillaApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVillaApi.Data
{
    public class VillaDAO
    {
        private readonly ApplicationDbContext _dbContext;
        public VillaDAO(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Villa>> GetAll()
        {
            return await _dbContext.Villas.ToListAsync();
        }
        public async Task<Villa> Get(Expression<Func<Villa, bool>> filter,bool tracked=true)
        {
            if (tracked)
            {
                return await _dbContext.Villas.FirstOrDefaultAsync(filter);
            }
            else
            {
                return await _dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(filter);
            }
        }
        public async Task CreateVilla(Villa villa)
        {
            await _dbContext.AddAsync(villa);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteVilla(Villa villa)
        {
            _dbContext.Remove(villa);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateVilla(Villa villa)
        {
            villa.UpdatedAt = DateTime.Now;
            _dbContext.Update(villa);
            await _dbContext.SaveChangesAsync();
        }
    }
}
