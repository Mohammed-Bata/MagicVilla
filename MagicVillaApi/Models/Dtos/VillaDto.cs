using System.ComponentModel.DataAnnotations;

namespace MagicVillaApi.Models.Dtos
{
    public class VillaDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Details { get; set; }
        public string ImageUrl { get; set; }
        public int Occupancy { get; set; }
        [Required]
        public double Rate { get; set; }
        public int Sqft { get; set; }
        public string Amenity { get; set; }
    }
}
