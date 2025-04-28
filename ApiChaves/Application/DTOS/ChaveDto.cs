namespace ApiChaves.Application.DTOS
{
    public class ChaveDto
    {
        public int IdProduto { get; set; }
        public string? CNPJCliente { get; set; }
        public string? Mac { get; set; }
        public string? Licenca { get; set; }
        public bool Status { get; set; }
    }
}
