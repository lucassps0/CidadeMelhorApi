using Microsoft.AspNetCore.Mvc;

namespace CidadeMelhorApi.Controllers
{
    [Route("api/cep")]
    [ApiController]
    public class CepController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public CepController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("{cep}")]
        public async Task<IActionResult> Buscar(string cep)
        {
            var cepLimpo = new string(cep.Where(char.IsDigit).ToArray());

            if (cepLimpo.Length != 8)
                return BadRequest("CEP invalido.");

            var resposta = await _httpClient.GetAsync($"https://viacep.com.br/ws/{cepLimpo}/json/");

            if (!resposta.IsSuccessStatusCode)
                return BadRequest("Nao foi possivel buscar o CEP.");

            var conteudo = await resposta.Content.ReadAsStringAsync();
            return Content(conteudo, "application/json");
        }
    }
}
