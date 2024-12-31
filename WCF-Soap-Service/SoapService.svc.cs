using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace WCF_Soap_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SoapService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SoapService.svc or SoapService.svc.cs at the Solution Explorer and start debugging.
    public class SoapService : InterfaceSoapService
    {

        public void AddProduto(string nome, string descricao)
        {

            using (var connection = new MySqlConnection("Server=database-azure.mysql.database.azure.com;Database=dbpoo;Uid=a20750;Pwd=a!WdsFd2d3d31!;SslMode=Required;"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO produto (Nome, Descricao) VALUES (@Nome, @Descricao)";
                command.Parameters.AddWithValue("@Nome", nome);
                command.Parameters.AddWithValue("@Descricao", descricao);

                command.ExecuteNonQuery();
            }

            Console.WriteLine($"Produto {nome} inserido com sucesso.");
        }
        public void AddTicket(string descricao, string tipoAssistencia, string estadoAssistencia, int? avaliacao, int clienteId, int? operadorId)
        {
            using (var connection = new MySqlConnection("Server=database-azure.mysql.database.azure.com;Database=dbpoo;Uid=a20750;Pwd=a!WdsFd2d3d31!;SslMode=Required;"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO ticket 
                (TicketDescricao, DataCriacao, TipoAssistencia, EstadoAssistencia, avaliacao, clienteid, operadorid) 
                VALUES (@Descricao, @DataCriacao, @TipoAssistencia, @EstadoAssistencia, @Avaliacao, @ClienteId, @OperadorId)";
                command.Parameters.AddWithValue("@Descricao", descricao);
                command.Parameters.AddWithValue("@DataCriacao", DateTime.Now);
                command.Parameters.AddWithValue("@TipoAssistencia", tipoAssistencia);
                command.Parameters.AddWithValue("@EstadoAssistencia", estadoAssistencia);
                command.Parameters.AddWithValue("@Avaliacao", avaliacao);//.HasValue ? (object)avaliacao.Value : DBNull.Value);
                command.Parameters.AddWithValue("@ClienteId", clienteId);
                command.Parameters.AddWithValue("@OperadorId", operadorId);
                command.ExecuteNonQuery();
            }
            Console.WriteLine($"Ticket inserido com sucesso para o cliente ID: {clienteId}");
        }

        public async Task<List<TicketSoap>> GetTickets()
        {
            var tickets = new List<TicketSoap>();
            using (var connection = new MySqlConnection("Server=database-azure.mysql.database.azure.com;Database=dbpoo;Uid=a20750;Pwd=a!WdsFd2d3d31!;SslMode=Required;"))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM ticket";

                using (var command = new MySqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tickets.Add(new TicketSoap
                        {
                            TicketId = reader.GetInt32(reader.GetOrdinal("TicketId")),
                            TicketDescricao = reader.GetString(reader.GetOrdinal("TicketDescricao")),
                            DataCriacao = reader.GetDateTime(reader.GetOrdinal("DataCriacao")),
                            TipoAssistencia = reader.GetString(reader.GetOrdinal("TipoAssistencia")),
                            EstadoAssistencia = reader.GetString(reader.GetOrdinal("estadoAssistencia")),
                            Avaliacao = reader.IsDBNull(reader.GetOrdinal("avaliacao")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("avaliacao")),
                            ClienteId = reader.GetInt32(reader.GetOrdinal("clienteid")),
                            OperadorId = reader.IsDBNull(reader.GetOrdinal("operadorid")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("operadorid"))

                        });
                    }
                }
            }

            return tickets;
        }


    }
}
