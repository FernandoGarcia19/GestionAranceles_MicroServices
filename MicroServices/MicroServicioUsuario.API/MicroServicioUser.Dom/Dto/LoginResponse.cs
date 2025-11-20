namespace MicroServicioUser.Dom.Dto;

public class LoginResponse
{
    public bool Ok { get; set; }
    public string Error { get; set; }
    public string Token { get; set; }
    public string TokenType { get; set; }
}

