using API_CallCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SoapService;
using System.ServiceModel;

namespace API_CallCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private static List<Models.Tickets> Tickets = new List<Models.Tickets>();

        private readonly InterfaceSoapServiceClient _soapServiceClient;
        private static List<Models.Produtos> ProdutosMock = new List<Models.Produtos>();

        public TicketsController()
        {
            try
            {
                var binding = new BasicHttpBinding
                {
                    MaxBufferSize = int.MaxValue,
                    MaxReceivedMessageSize = int.MaxValue,
                    AllowCookies = true,
                    Security = new BasicHttpSecurity
                    {
                        Mode = BasicHttpSecurityMode.Transport // Transport for HTTPS
                    }
                };

                var endpointAddress = new EndpointAddress("https://soap-service-hkc8gzhra4fcdyd0.spaincentral-01.azurewebsites.net/SoapService.svc");
                _soapServiceClient = new InterfaceSoapServiceClient(binding, endpointAddress);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing SOAP client: {ex.Message}");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            try
            {
                var tickets = await _soapServiceClient.GetTicketsAsync();
                return Ok(tickets);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Erro MySQL: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro: {ex.Message}, Detalhes: {ex.StackTrace}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTicket([FromBody] Tickets ticket)
        {
            if (ticket == null || string.IsNullOrWhiteSpace(ticket.TicketDescricao) ||
                string.IsNullOrWhiteSpace(ticket.TipoAssistencia) ||
                ticket.ClienteId <= 0)
            {
                return BadRequest("Dados do ticket inválidos. Descrição, tipo de assistência, ID do cliente são obrigatórios.");
            }

            try
            {
                // Chama o método AddTicket do serviço SOAP para inserir os dados 
                await _soapServiceClient.AddTicketAsync(
                    ticket.TicketDescricao,
                    ticket.TipoAssistencia,
                    ticket.EstadoAssistencia,
                    ticket.Avaliacao,
                    ticket.ClienteId,
                    ticket.OperadorId
                );

                // Retorna uma mensagem de sucesso
                return Ok(new { message = "Ticket adicionado com sucesso." });
            }
            catch (Exception ex)
            {
                // Captura e retorna o erro em caso de falha
                return StatusCode(500, new { error = $"Erro ao adicionar ticket: {ex.Message}" });
            }
        }
        /*
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var ticket = Tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }
        */
    }

}
