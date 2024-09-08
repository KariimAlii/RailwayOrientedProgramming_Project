namespace API.Models
{
    public class SubmitOrderRequest
    {
        public int CreditCardId { get; set; }
        public int TotalAmount { get; set; }
        public List<LineItem> LineItems { get; set; }
        public int CustomerId { get; set; }
    }
    public class LineItem
    {

    }
}
