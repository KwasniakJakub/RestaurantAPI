namespace RestaurantAPI.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string City { get; set; }
        public int Streer { get; set; }
        public string PostalCode { get; set; }

        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }
    }
}
