using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Dominio.ModelViews;

namespace MinimalApi.Endpoints
{
    public static class VeiculoEndpoint
    {
        public static void AddEndpoints(IEndpointRouteBuilder endpoints)
        {
            #region Função de validação
            ErrosDeValidacao ValidaDTO(VeiculoDTO veiculoDTO)
            {
                var validacao = new ErrosDeValidacao
                {
                    Mensagens = new List<string>()
                };

                if (string.IsNullOrEmpty(veiculoDTO.Modelo))
                    validacao.Mensagens.Add("O modelo não pode ser vazio");

                if (string.IsNullOrEmpty(veiculoDTO.Marca))
                    validacao.Mensagens.Add("A marca não pode ficar em branco");

                if (veiculoDTO.Ano < 1950)
                    validacao.Mensagens.Add("Veículo muito antigo, aceito somente anos superiores a 1950");

                return validacao;
            }
            #endregion

            #region Criar veículo
            endpoints.MapPost("/veiculos", (
                [FromBody] VeiculoDTO veiculoDTO,
                IVeiculoServico veiculoServico) =>
            {
                var validacao = ValidaDTO(veiculoDTO);
                if (validacao.Mensagens.Count > 0)
                    return Results.BadRequest(validacao);

                var veiculo = new Veiculo
                {
                    Modelo = veiculoDTO.Modelo,  // ✅ CORRETO
                    Marca = veiculoDTO.Marca,
                    Ano = veiculoDTO.Ano
                };

                veiculoServico.Incluir(veiculo);
                return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
            .WithTags("Veiculos");
            #endregion

            #region Listar veículos
            endpoints.MapGet("/veiculos", (
                [FromQuery] int? pagina,
                IVeiculoServico veiculoServico) =>
            {
                var paginaAtual = pagina ?? 1;
                var veiculos = veiculoServico.Todos(paginaAtual);
                return Results.Ok(veiculos);
            })
            .AllowAnonymous()
            .WithTags("Veiculos");
            #endregion

            #region Buscar veículo por ID
            endpoints.MapGet("/veiculos/{id}", (
                [FromRoute] int id,
                IVeiculoServico veiculoServico) =>
            {
                var veiculo = veiculoServico.BuscaPorId(id);
                if (veiculo == null) return Results.NotFound();
                return Results.Ok(veiculo);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
            .WithTags("Veiculos");
            #endregion

            #region Atualizar veículo
            endpoints.MapPut("/veiculos/{id}", (
                [FromRoute] int id,
                [FromBody] VeiculoDTO veiculoDTO,
                IVeiculoServico veiculoServico) =>
            {
                var veiculo = veiculoServico.BuscaPorId(id);
                if (veiculo == null) return Results.NotFound();

                var validacao = ValidaDTO(veiculoDTO);
                if (validacao.Mensagens.Count > 0)
                    return Results.BadRequest(validacao);

                veiculo.Modelo = veiculoDTO.Modelo;  // ✅ CORRETO
                veiculo.Marca = veiculoDTO.Marca;
                veiculo.Ano = veiculoDTO.Ano;

                veiculoServico.Atualizar(veiculo);
                return Results.Ok(veiculo);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Veiculos");
            #endregion

            #region Deletar veículo
            endpoints.MapDelete("/veiculos/{id}", (
                [FromRoute] int id,
                IVeiculoServico veiculoServico) =>
            {
                var veiculo = veiculoServico.BuscaPorId(id);
                if (veiculo == null) return Results.NotFound();

                veiculoServico.Apagar(veiculo);
                return Results.NoContent();
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Veiculos");
            #endregion
        }
    }
}