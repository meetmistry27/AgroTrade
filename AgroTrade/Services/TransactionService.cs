using AgroTrade.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgroTrade.Services
{
    public class TransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions.Include(t => t.Crop).Include(t => t.Buyer).Include(t => t.Seller).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(int userId)
        {
            return await _context.Transactions
                .Where(t =>  t.SellerId == userId)
                .ToListAsync();
        }
        public async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions.Include(t => t.Crop).Include(t => t.Buyer).Include(t => t.Seller).FirstOrDefaultAsync(t => t.TransactionId == id);
        }


        public async Task AcceptTransactionAsync(int transactionId)
        {
            var transaction = await GetTransactionByIdAsync(transactionId);
            if (transaction != null)
            {
                transaction.Status = TransactionStatus.Accepted;

                var crop = await _context.Crops.FindAsync(transaction.CropId);
                if (crop != null)
                {
                    crop.Quantity -= transaction.Quantity; 
                    _context.Crops.Update(crop); 
                }

                await _context.SaveChangesAsync(); 
            }
        }

        public async Task RejectTransactionAsync(int transactionId)
        {
            var transaction = await GetTransactionByIdAsync(transactionId);
            if (transaction != null)
            {
                transaction.Status = TransactionStatus.Rejected;
                await _context.SaveChangesAsync();
            }
        }
    }
}
