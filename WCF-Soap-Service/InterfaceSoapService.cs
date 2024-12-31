using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WCF_Soap_Service
{
    [ServiceContract]
    public interface InterfaceSoapService
    {
        [OperationContract]
        void AddProduto(string nome, string descricao);

        [OperationContract]
        void AddTicket(string descricao, string tipoAssistencia, string estadoAssistencia, int? avaliacao, int clienteId, int? operadorId);

        [OperationContract]
        Task<List<TicketSoap>> GetTickets();
    }
}
