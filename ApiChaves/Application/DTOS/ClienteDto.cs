namespace ApiChaves.Application.DTOS
{
    public class ClienteDto
    {
        public string? CNPJ { get; set; }
        public string? RazaoSocial { get; set; }
        public string? NomeFantasia { get; set; }
        public bool Status { get; set; }
    }
}
