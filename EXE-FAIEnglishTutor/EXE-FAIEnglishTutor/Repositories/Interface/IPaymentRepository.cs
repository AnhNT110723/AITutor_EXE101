using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Repositories.Interface
{
    public interface IPaymentRepository : IBaseRepository<Payment>
    {
        Task SavePaymentAsync(Payment payment);
        Task<PaymentDto> GetPaymentByIdAsync(int id);
        Task UpdatePaymentAsync(PaymentDto paymentDto);
        Task<decimal> GetMonthlyEarningsAsync();
        Task<decimal> GetAnnualEarningsAsync();
        Task<List<decimal>> GetMonthlyEarningsForChartAsync();
    }
}
