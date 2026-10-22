using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.DTOs;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Cursos;

public sealed record ListarCursosQuery()
    : IRequest<Result<IReadOnlyList<CursoDto>>>;

public sealed class ListarCursosQueryHandler(
    IRepositorioCurso repositorioCurso
) : IRequestHandler<ListarCursosQuery, Result<IReadOnlyList<CursoDto>>>
{
    public async Task<Result<IReadOnlyList<CursoDto>>> Handle(
        ListarCursosQuery query,
        CancellationToken cancellationToken)
    {
        var cursos = await repositorioCurso.SelecionarTodosAsync(cancellationToken);

        IReadOnlyList<CursoDto> resultado = cursos.Select(curso => new CursoDto(
            curso.Id,
            curso.Nome,
            curso.Descricao,
            curso.CargaHoraria,
            curso.DataConclusao
        )).ToList();

        return Result.Ok(resultado);
    }
}