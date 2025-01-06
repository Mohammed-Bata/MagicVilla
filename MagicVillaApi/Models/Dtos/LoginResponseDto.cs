namespace MagicVillaApi.Models.Dtos
{
    public class LoginResponseDto
    {
        public UserDto user { get; set; }
        public string token {  get; set; }
    }
}
