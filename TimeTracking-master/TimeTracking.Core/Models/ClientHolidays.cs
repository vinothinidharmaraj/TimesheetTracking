using System.ComponentModel.DataAnnotations;

namespace TimeTracking.Models
{
    public  class ClientHolidays
    {
        [Key]
        public int HolidayYear { get; set; }
        public string PublicHolidays { get; set; }
        public virtual Client Client { get; set; }
    }
}