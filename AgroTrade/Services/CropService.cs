using AgroTrade.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgroTrade.Services
{
    public class CropService
    {
        private readonly ApplicationDbContext _context;

        public CropService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Crop>> GetAllCropsAsync()
        {
            return await _context.Crops.Include(c => c.User).ToListAsync();
        }

        public async Task<Crop> GetcropByid(int id)
        {
            return await _context.Crops.FirstOrDefaultAsync(c => c.CropId == id);
        }

        public async Task CreateCropAsync(Crop crop)
        {
            _context.Crops.Add(crop);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCropAsync(Crop crop)
        {
            _context.Crops.Update(crop);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCropAsync(int id)
        {
            var crop = await GetcropByid(id); 

            if (crop != null)
            {
                _context.Crops.Remove(crop);
                await _context.SaveChangesAsync(); 
            }
        }

    }
}
