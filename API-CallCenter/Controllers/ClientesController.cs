using Microsoft.AspNetCore.Mvc;
using SoapService;
using MySql.Data.MySqlClient;

namespace API_CallCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {

        private readonly string _connectionString;

        public ClientesController(IConfiguration configuration)
        {
            // Obtém a connection string do appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        [HttpGet]
        public IActionResult GetClientes()
        {
            List<Models.Clientes> clientes = new List<Models.Clientes>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM cliente"; 

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var cliente = new Models.Clientes
                                {
                                    ClienteId = reader.GetInt32("clienteid"),
                                    Nome = reader.GetString("Nome"),
                                    Contacto = reader.GetString("Contacto"),
                                    Empresa = reader.GetString("Empresa"),
 
                                };
                                clientes.Add(cliente);
                            }
                        }
                    }
                }

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter clientes: {ex.Message}");
            }
        }

        // GET: api/clientes/{id}
        [HttpGet("{id}")]
        public IActionResult GetClienteById(int id)
        {
            Models.Clientes cliente = null;

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM cliente WHERE clienteid = @ClienteId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClienteId", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cliente = new Models.Clientes
                                {
                                    ClienteId = reader.GetInt32("clienteid"),
                                    Nome = reader.GetString("Nome"),
                                    Contacto = reader.GetString("Contacto"),
                                    Empresa = reader.GetString("Empresa"),
                                };
                            }
                        }
                    }
                }

                if (cliente == null)
                {
                    return NotFound($"Cliente com ID {id} não encontrado.");
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter cliente: {ex.Message}");
            }
        }

        // POST: api/clientes
        [HttpPost]
        public IActionResult AddCliente([FromBody] Models.Clientes novoCliente)
        {
            if (novoCliente == null || string.IsNullOrWhiteSpace(novoCliente.Nome))
            {
                return BadRequest("Dados do cliente inválidos.");
            }

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO cliente (Nome, Contacto, Empresa) VALUES (@Nome, @Contacto, @Empresa)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nome", novoCliente.Nome);
                        command.Parameters.AddWithValue("@Contacto", novoCliente.Contacto);
                        command.Parameters.AddWithValue("@Empresa", novoCliente.Empresa);
                        command.ExecuteNonQuery();
                    }
                }

                return CreatedAtAction(nameof(GetClienteById), new { id = novoCliente.ClienteId }, novoCliente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao adicionar cliente: {ex.Message}");
            }
        }

        // PUT: api/clientes/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateCliente(int id, [FromBody] Models.Clientes clienteAtualizado)
        {
            if (clienteAtualizado == null || id != clienteAtualizado.ClienteId)
            {
                return BadRequest("Dados do cliente inválidos.");
            }

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE cliente SET Nome = @Nome, Contacto = @Contacto, Empresa = @Empresa WHERE clienteid = @ClienteId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nome", clienteAtualizado.Nome);
                        command.Parameters.AddWithValue("@Contacto", clienteAtualizado.Contacto);
                        command.Parameters.AddWithValue("@Empresa", clienteAtualizado.Empresa);
                        command.Parameters.AddWithValue("@ClienteId", id);

                        var rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return NotFound($"Cliente com ID {id} não encontrado.");
                        }
                    }
                }

                return Ok(clienteAtualizado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar cliente: {ex.Message}");
            }
        }

        // DELETE: api/clientes/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteCliente(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM cliente WHERE clienteid = @ClienteId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClienteId", id);
                        var rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return NotFound($"Cliente com ID {id} não encontrado.");
                        }
                    }
                }

                return Ok(new { message = $"Cliente com ID {id} foi excluído com sucesso." }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao eliminar cliente: {ex.Message}");
            }
        }
    }
}
  