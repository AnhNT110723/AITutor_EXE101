using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace EXE_FAIEnglishTutor.Repositories.Implementation
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(FaiEnglishContext context) : base(context)
        {
        }

        public async Task<decimal> GetAnnualEarningsAsync()
        {
            var annualEarnings = await _context.Payments
            .Where(p => p.Status == "Completed"
                     && p.PaymentDate.HasValue
                     && p.PaymentDate.Value.Year == DateTime.Now.Year)
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;

            return annualEarnings;
        }

        public  async Task<decimal> GetMonthlyEarningsAsync()
        {
            var currentDate = DateTime.Now;
            var monthlyEarnings = await _context.Payments
                .Where(p => p.Status == "Completed"
                         && p.PaymentDate.HasValue
                         && p.PaymentDate.Value.Year == currentDate.Year
                         && p.PaymentDate.Value.Month == currentDate.Month)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;

            return monthlyEarnings;
        }

        public async Task<List<decimal>> GetMonthlyEarningsForChartAsync()
        {
            var startDate = DateTime.Now.AddMonths(-11); // 12 tháng gần nhất

            var monthlyEarnings = await _context.Payments
                .Where(p => p.Status == "Completed" && p.PaymentDate >= startDate)
                .GroupBy(p => new { p.PaymentDate.Value.Year, p.PaymentDate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(p => p.Amount)
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            // Tạo danh sách 12 tháng với giá trị mặc định là 0
            var result = new List<decimal>(new decimal[12]);
            var currentDate = startDate;
            for (int i = 0; i < 12; i++)
            {
                var year = currentDate.Year;
                var month = currentDate.Month;
                var earnings = monthlyEarnings.FirstOrDefault(e => e.Year == year && e.Month == month)?.Total ?? 0m;
                result[i] = earnings;
                currentDate = currentDate.AddMonths(1);
            }

            return result;
        }

        public Task<PaymentDto> GetPaymentByIdAsync(int id)
        {
            return _context.Payments.Where(x => x.PaymentId == id).Select(x => new PaymentDto(x)).FirstOrDefaultAsync();
        }

        public async Task SavePaymentAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentAsync(PaymentDto paymentDto)
        {
            var payment = await _context.Payments.FindAsync(paymentDto.PaymentId); // Lấy instance đã theo dõi
            if (payment != null)
            {
                payment.Status = paymentDto.Status; // Cập nhật chỉ thuộc tính cần thay đổi
                await _context.SaveChangesAsync();
            }

        }
    }
}
