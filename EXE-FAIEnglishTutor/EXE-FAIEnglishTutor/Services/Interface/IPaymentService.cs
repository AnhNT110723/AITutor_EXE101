using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Services.Interface
{
    public interface IPaymentService
    {
        Task SavePaymentAsync(Payment payment);
        Task<PaymentDto> GetPaymentByIdAsync(int id);
        Task UpdatePaymentAsync(PaymentDto p);
        Task<decimal> GetMonthlyEarningsAsync();
        Task<decimal> GetAnnualEarningsAsync();
        Task<List<decimal>> GetMonthlyEarningsForChartAsync();
    }
}
