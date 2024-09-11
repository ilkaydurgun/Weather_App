namespace Weather_App.Models
{
    public class Weather
    {
        public int Id { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public int Temperature { get; set; }
        public string State { get; set; }
    }
}
