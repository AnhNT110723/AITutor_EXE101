using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface;
using EXE_FAIEnglishTutor.Repositories.Interface;
using EXE_FAIEnglishTutor.Dtos;

namespace EXE_FAIEnglishTutor.Services.Implementaion
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        public PaymentService(IPaymentRepository paymentRepository) {
            _paymentRepo = paymentRepository;
        }

        public async Task<decimal> GetAnnualEarningsAsync()
        {
            var annualEarnings = await _paymentRepo.GetAnnualEarningsAsync();
            return annualEarnings;
        }

        public async Task<decimal> GetMonthlyEarningsAsync()
        {
            var monthlyEarnings = await _paymentRepo.GetMonthlyEarningsAsync();
            return monthlyEarnings;
        }

        public async Task<List<decimal>> GetMonthlyEarningsForChartAsync()
        {
           return await _paymentRepo.GetMonthlyEarningsForChartAsync();
        }

        public async Task<PaymentDto> GetPaymentByIdAsync(int id)
        {
            return await _paymentRepo.GetPaymentByIdAsync(id);
        }

        public async Task SavePaymentAsync(Payment payment)
        {
            await _paymentRepo.SavePaymentAsync(payment);

        }

        public async Task UpdatePaymentAsync(PaymentDto p)
        {
            await _paymentRepo.UpdatePaymentAsync(p);
        }
    }
}
