using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;

namespace RankingVendedores.Servicos.Interfaces
{
    public interface IFuncionarioApiService
    {
        Task<ResultadoOperacao<List<FuncionarioDto>>> ObterFuncionariosAsync();

        Task<FuncionarioDto?> ObterFuncionarioPorIdAsync(int id);

        Task<List<FuncionarioDto>> PesquisarFuncionariosAsync(string nome);

        Task<FuncionarioDto> CriarFuncionarioAsync(CriarFuncionarioDto funcionario);

        Task<FuncionarioDto> AtualizarFuncionarioAsync(AtualizarFuncionarioDto funcionario);

        Task RemoverFuncionarioAsync(int id);

        Task<bool> PodeFuncionarioSerRemovidoAsync(int id);
    }
}
