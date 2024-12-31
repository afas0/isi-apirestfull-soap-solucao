namespace API_CallCenter.Models
{
    public class Tickets
    {
        public int TicketId { get; set; }
        public string TicketDescricao { get; set; }
        public string TipoAssistencia { get; set; }
        public string EstadoAssistencia { get; set; }
        public int? Avaliacao { get; set; }
        public int ClienteId { get; set; }
        public int? OperadorId { get; set; }
    }
}
