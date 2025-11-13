using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MinimalApi.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto;
    public VeiculoServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public void Apagar(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        _contexto.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
        _contexto.SaveChanges();
    }

    public Veiculo? BuscaPorId(int id)
    {
        return _contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Incluir(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }

   public List<Veiculo> Todos(int? pagina = 1, string? modelo = null, string? marca = null)
{
    // ✅ MÉTODO SUPER SIMPLES - sem WHERE
    var paginaAtual = pagina ?? 1;
    var itensPorPagina = 10;
    
    return _contexto.Veiculos
        .OrderBy(v => v.Id)
        .Skip((paginaAtual - 1) * itensPorPagina)
        .Take(itensPorPagina)
        .ToList();
}
}