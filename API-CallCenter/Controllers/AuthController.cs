using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API_CallCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly string _clientId = "ENffUTiN2sTGg4UKE0WRODuaNbZAt7rt";
        private readonly string _clientSecret = "BBJUgVukX6EAZLoCQVXF-sxJTO6OeRV3rt3cGNQgaslL9jWCY3FgU4QZqplYCxlh";
        private readonly string _auth0Domain = "https://dev-aykzsyocig5der37.us.auth0.com";
        private readonly string _redirectUri = "https://api-call-center-ayhwesggb2eeh8fp.spaincentral-01.azurewebsites.net/api/auth/callback";  // URL de callback da API

        [HttpGet("login")]
        public IActionResult Login()
        {
            var authUrl = $"{_auth0Domain}/authorize?client_id={_clientId}&response_type=code&redirect_uri={_redirectUri}&scope=openid profile&audience=https://dev-aykzsyocig5der37.us.auth0.com/api/v2/&prompt=login"; //&prompt=login para pedir sempre login
            return Redirect(authUrl);  // Redireciona o user para o Auth0 para login
        }

        // Este endpoint trata o callback do Auth0
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            // Troca o código de autorização por um token de acesso
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"{_auth0Domain}/oauth/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "code", code },
                { "redirect_uri", _redirectUri },
                { "grant_type", "authorization_code" }
            })
            };

            var client = new HttpClient();
            var tokenResponse = await client.SendAsync(tokenRequest);
            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();

            // Extrai o token de acesso da resposta
            var accessToken = ExtractAccessToken(tokenContent); // Analisa o token de acesso da resposta

            // Redireciona para a app Windows Forms com o token
            var redirectUrl = $"http://localhost:3000/callback?token={accessToken}";

            return Redirect(redirectUrl);

        }

        // Método auxiliar para extrair o token de acesso
        private string ExtractAccessToken(string response)
        {
            var tokenObject = JsonSerializer.Deserialize<JsonElement>(response);
            return tokenObject.GetProperty("access_token").GetString();
        }

    }
}
