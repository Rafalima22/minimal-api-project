using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Dominio.Entidades;

public class Pessoa
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Nome { get; set; } = default!;

    [Required]
    [StringLength(14)]
    public string Cpf { get; set; } = default!;

    [Required]
    [StringLength(255)]
    public string Email { get; set; } = default!;

    [StringLength(20)]
    public string? Telefone { get; set; }

    // Relação: uma pessoa pode ter vários carros
    public ICollection<Veiculo>? Veiculos { get; set; }
}
