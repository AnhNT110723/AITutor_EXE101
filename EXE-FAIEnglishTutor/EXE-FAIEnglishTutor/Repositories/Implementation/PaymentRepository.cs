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
