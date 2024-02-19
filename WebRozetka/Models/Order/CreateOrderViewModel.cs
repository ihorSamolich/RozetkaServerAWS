namespace WebRozetka.Models.Order
{

    public class CustomerPersonalData
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }

    public class DepartmentData
    {
        public int AreaId { get; set; }
        public int SettlementId { get; set; }
        public int WarehouseId { get; set; }
    }

    public class PaymentData
    {
        public string PaymentType { get; set; }
    }

    public class CreateOrderViewModel
    {
        public CustomerPersonalData CustomerPersonalData { get; set; }
        public DepartmentData DepartmentData { get; set; }
        public PaymentData PaymentData { get; set; }
    }
}
