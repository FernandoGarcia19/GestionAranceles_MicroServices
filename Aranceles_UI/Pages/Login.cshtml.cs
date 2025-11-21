using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Aranceles_UI.Domain.Dtos;


namespace Aranceles_UI.Pages;


public class LoginDTO
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginResponseDTO
{
    public bool Ok { get; set; }
    public string Error { get; set; }
    public string Token { get; set; }
    public string TokenType { get; set; }
}

public class LoginModel : PageModel
{
    private readonly HttpClient userClient;

    public LoginModel(IHttpClientFactory clientFactory)
    {
        userClient = clientFactory.CreateClient("userApi");
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [FromQuery(Name = "ReturnUrl")]
    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    public void OnGet() {}
    
    public async Task<IActionResult> OnPostAsync()
    {
        var dto = new LoginDTO()
        {
            Username =  Input.Username,
            Password =  Input.Password
        };
    
        var result = userClient.PostAsJsonAsync<LoginDTO>("api/User/login", dto).Result;
        LoginResponseDTO response = await result.Content.ReadFromJsonAsync<LoginResponseDTO>();
        if (!response.Ok) 
        { 
            ModelState.AddModelError(string.Empty, response.Error??"Credenciales inválidas."); 
            return Page(); 
        }

        string accessToken = response.Token;
        
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(accessToken); 
        var claims = new List<Claim>();
        int userId =int.Parse(jwt.Claims.Where(c => c.Type == JwtRegisteredClaimNames.NameId).First().Value);
        foreach (var c in jwt.Claims)
        {
            claims.Add(c);
        }
        
        claims.Add(new Claim("access_token", accessToken));
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = Input.RememberMe });
        UserDto user = await userClient.GetFromJsonAsync<UserDto>($"api/User/getById/{userId}");
        
        if (user.FirstLogin == 1)
        {
            System.Console.WriteLine("Redirecting to ChangePassword");
            return RedirectToPage("/ChangePassword");
        }

        if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        {
            System.Console.WriteLine($"Redirecting to ReturnUrl: {ReturnUrl}");
            return LocalRedirect(ReturnUrl);
        }

        System.Console.WriteLine("Redirecting to Index");
        return RedirectToPage("/Index");

    }
}
