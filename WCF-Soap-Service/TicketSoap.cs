using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCF_Soap_Service
{
    public class TicketSoap
    {
        public int TicketId { get; set; }
        public string TicketDescricao { get; set; }
        public DateTime? DataCriacao { get; set; }
        public string TipoAssistencia { get; set; }
        public string EstadoAssistencia { get; set; }
        public int? Avaliacao { get; set; }
        public int ClienteId { get; set; }
        public int? OperadorId { get; set; }
    }
}
