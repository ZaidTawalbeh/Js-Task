namespace TravelMate.Models
{

    public class PaymentDetails
    {
        public Reservation Reservation { get; set; }
        public decimal TotalPrice { get; set; }
        public string CardNumber { get; set; }  // رقم البطاقة
        public DateTime ExpirationDate { get; set; }  // تاريخ انتهاء البطاقة
        public decimal UserId { get; set; }

    }


}
