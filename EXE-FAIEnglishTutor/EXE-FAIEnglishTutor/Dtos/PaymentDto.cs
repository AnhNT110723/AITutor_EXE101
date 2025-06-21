using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Dtos
{
    public class PaymentDto
    {
        public PaymentDto(){}

        public PaymentDto(Payment p)
        {
            PaymentId = p.PaymentId;
            UserId = p.UserId ?? 0;
            TotalAmount = p.Amount;
            PaymentDate = p.PaymentDate;
            Content = p.Content;
            Status = p.Status;
        }

        public Payment toPayment()
        {
            return new Payment
            {
                PaymentId = this.PaymentId,
                UserId = this.UserId,
                Amount = this.TotalAmount,
                PaymentDate = this.PaymentDate,
                Content = this.Content,
                Status = this.Status
            };
        }
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public DateTime? PaymentDate { get; set; } = DateTime.Now;
        public string? Status { get; set; }
        public string BankId { get; set; } = "MB";
        public string AccountNo { get; set; } = "1231231232131";
        public string AccountName { get; set; } = "NGUYEN TUAN ANH";
        public decimal TotalAmount { get; set; }
        public string Content { get; set; }
        public string QRCodeUrl { get; set; }
    }
}
