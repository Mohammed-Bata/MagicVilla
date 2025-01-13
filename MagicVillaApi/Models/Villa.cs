using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVillaApi.Models
{
    public class Villa
    {
        public int Id {  get; set; }
       [Required]
       public string Name {  get; set; }
        [Required]
       public string Details {  get; set; } 
       public string? ImageUrl {  get; set; }
        public string? ImageLocalPath { get; set; }
        public int Occupancy {  get; set; }
       [Required]
       public double Rate {  get; set; }
       public int Sqft {  get; set; }
       public string Amenity {  get; set; }
       public DateTime CreatedAt { get; set; }
       public DateTime UpdatedAt { get; set; }
    }
}
