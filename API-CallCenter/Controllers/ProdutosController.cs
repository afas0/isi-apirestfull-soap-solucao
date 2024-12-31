using System;
using System.ServiceModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoapService;

namespace API_CallCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        //private readonly ProdutoServiceClient _produtoServiceClient;
        private readonly InterfaceSoapServiceClient _soapServiceClient;
        private static List<Models.Produtos> ProdutosMock = new List<Models.Produtos>();


        public ProdutosController()
        {           
            try
            {
                // Configuração explícita para usar HTTPS
                var binding = new BasicHttpsBinding
                {
                    Security = new BasicHttpsSecurity
                    {
                        Mode = BasicHttpsSecurityMode.Transport
                    }
                };

                // URL do endpoint SOAP publicado no Azure
                var endpointAddress = new EndpointAddress("https://soap-service-hkc8gzhra4fcdyd0.spaincentral-01.azurewebsites.net/SoapService.svc");

                // Instância do cliente SOAP configurada
                _soapServiceClient = new InterfaceSoapServiceClient(binding, endpointAddress);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing SOAP client: {ex.Message}");
                throw;
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProduto([FromBody] Models.Produtos produto)
        {
            if (produto == null || string.IsNullOrEmpty(produto.Nome) || string.IsNullOrEmpty(produto.Descricao))
            {
                return BadRequest("Dados do produto inválidos.");
            }

            try
            {
                // Chama o método AddProdutoAsync do serviço SOAP
                await _soapServiceClient.AddProdutoAsync(produto.Nome, produto.Descricao);

                // Retorna uma resposta de sucesso
                return Ok("Produto adicionado com sucesso.");
            }
            catch (Exception ex)
            {
                // Se ocorrer um erro, retorna um erro com a mensagem
                return StatusCode(500, $"Erro ao adicionar produto: {ex.Message}");
            }
        }
        /*
        [HttpGet("{id}")]
        public IActionResult GetProdutoById(int id)
        {
            // Simula a busca no "banco de dados"
            var produto = ProdutosMock.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(produto);
        }

        [HttpGet]
        [Authorize] // Only authenticated users can access this endpoint
        public IActionResult GetProdutos()
        {
            return Ok(new { message = "Authorized access only" });
        }
        */
    }
}
