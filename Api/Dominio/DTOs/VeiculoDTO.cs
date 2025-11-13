
namespace MinimalApi.DTOs;
public record VeiculoDTO
{
    public string Modelo { get;set; } = default!;
    public string Marca { get;set; } = default!;
    public int Ano { get; set; } = default!;
    
    //  public int PessoaId { get;set; } = default!;
}