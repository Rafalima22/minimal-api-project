using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.DTOs;

namespace MinimalApi.Endpoints
{
    public static class AdministradorEndpoint
    {
        public static void AddEndpoints(IEndpointRouteBuilder endpoints, string key)
        {
            #region Função para gerar JWT
            string GerarTokenJwt(Administrador administrador)
            {
                if (string.IsNullOrEmpty(key)) return string.Empty;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email", administrador.Email),
                    new Claim("Perfil", administrador.Perfil),
                    new Claim(ClaimTypes.Role, administrador.Perfil),
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            #endregion

            #region Login
            endpoints.MapPost("/administradores/login", (
                [FromBody] LoginDTO loginDTO,
                IAdministradorServico administradorServico) =>
            {
                var adm = administradorServico.Login(loginDTO);
                if (adm != null)
                {
                    string token = GerarTokenJwt(adm);
                    return Results.Ok(new AdministradorLogado
                    {
                        Email = adm.Email,
                        Perfil = adm.Perfil,
                        Token = token
                    });
                }
                else
                    return Results.Unauthorized();
            })
            .AllowAnonymous()
            .WithTags("Administradores");
            #endregion

            #region Listar administradores
            endpoints.MapGet("/administradores", (
                [FromQuery] int? pagina,
                IAdministradorServico administradorServico) =>
            {
                var adms = administradorServico
                    .Todos(pagina)
                    .Select(adm => new AdministradorModelView
                    {
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil
                    }).ToList();

                return Results.Ok(adms);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");
            #endregion

            #region Buscar por ID
            endpoints.MapGet("/administradores/{id}", (
                [FromRoute] int id,
                IAdministradorServico administradorServico) =>
            {
                var administrador = administradorServico.BuscaPorId(id);
                if (administrador == null) return Results.NotFound();

                return Results.Ok(new AdministradorModelView
                {
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                });
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");
            #endregion

            #region Criar novo administrador
            endpoints.MapPost("/administradores", (
                [FromBody] AdministradorDTO administradorDTO,
                IAdministradorServico administradorServico) =>
            {
                var erros = new List<string>();

                if (string.IsNullOrEmpty(administradorDTO.Email))
                    erros.Add("Email não pode ser vazio");

                if (string.IsNullOrEmpty(administradorDTO.Senha))
                    erros.Add("Senha não pode ser vazia");

                if (administradorDTO.Perfil == null)
                    erros.Add("Perfil não pode ser vazio");

                if (erros.Count > 0)
                    return Results.BadRequest(new { Mensagens = erros });

                var administrador = new Administrador
                {
                    Email = administradorDTO.Email,
                    Senha = administradorDTO.Senha,
                    Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
                };

                administradorServico.Incluir(administrador);

                return Results.Created($"/administradores/{administrador.Id}", new AdministradorModelView
                {
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                });
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");
            #endregion
        }
    }
}
