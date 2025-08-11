using Microsoft.EntityFrameworkCore;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Interfaces.Repositorios;
using Ranking.Infraestrutura.Persistencia;

namespace Ranking.Infraestrutura.Repositorios
{
    /// Implementação do repositório de funcionários.
    /// Herda operações básicas e implementa operações específicas para funcionários.
    public class FuncionarioRepositorio : RepositorioBase<Funcionario>, IRepositorioFuncionario
    {
        public async Task<List<Funcionario>> ObterTodos()
        {
            return await _contexto.Funcionarios.ToListAsync();
        }

        /// Construtor que recebe o contexto do Entity Framework.
        /// <param name="contexto">Contexto do Entity Framework.</param>
        public FuncionarioRepositorio(RankingVendasContexto contexto) : base(contexto)
        {
        }

        /// Obtém um funcionário por nome.
        /// <param name="nome">Nome do funcionário.</param>
        /// <returns>Funcionário encontrado ou null se não existir.</returns>
        public async Task<Funcionario?> ObterPorNomeAsync(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return null;

            return await _dbSet
                .FirstOrDefaultAsync(f => f.Nome.ToLower() == nome.ToLower().Trim());
        }

        /// <summary>
        /// Obtém funcionários cujo nome contém o texto especificado.
        /// </summary>
        /// <param name="textoNome">Texto a ser pesquisado no nome.</param>
        /// <returns>Coleção de funcionários que atendem ao critério.</returns>
        public async Task<IEnumerable<Funcionario>> PesquisarPorNomeAsync(string textoNome)
        {
            if (string.IsNullOrWhiteSpace(textoNome))
                return await ObterTodosAsync();

            var textoPesquisa = textoNome.ToLower().Trim();

            return await _dbSet
                .Where(f => f.Nome.ToLower().Contains(textoPesquisa))
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém um funcionário com suas vendas carregadas.
        /// </summary>
        /// <param name="id">Identificador do funcionário.</param>
        /// <returns>Funcionário com vendas carregadas ou null se não existir.</returns>
        public async Task<Funcionario?> ObterComVendasAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _dbSet
                .Include(f => f.Vendas)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        /// <summary>
        /// Obtém todos os funcionários com suas vendas carregadas.
        /// </summary>
        /// <returns>Coleção de funcionários com vendas carregadas.</returns>
        public async Task<IEnumerable<Funcionario>> ObterTodosComVendasAsync()
        {
            return await _dbSet
                .Include(f => f.Vendas)
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém funcionários que realizaram vendas no período especificado.
        /// </summary>
        /// <param name="dataInicio">Data de início do período.</param>
        /// <param name="dataFim">Data de fim do período.</param>
        /// <returns>Coleção de funcionários que realizaram vendas no período.</returns>
        public async Task<IEnumerable<Funcionario>> ObterComVendasNoPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            if (dataInicio > dataFim)
                throw new ArgumentException("A data de início deve ser anterior à data de fim.");

            return await _dbSet
                .Include(f => f.Vendas.Where(v => v.DataVenda >= dataInicio && v.DataVenda <= dataFim))
                .Where(f => f.Vendas.Any(v => v.DataVenda >= dataInicio && v.DataVenda <= dataFim))
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }

        /// Obtém funcionários ordenados por total de vendas (decrescente).
        /// <param name="dataInicio">Data de início do período (opcional).</param>
        /// <param name="dataFim">Data de fim do período (opcional).</param>
        /// <returns>Coleção de funcionários ordenados por total de vendas.</returns>
        public async Task<IEnumerable<Funcionario>> ObterRankingPorTotalVendasAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var query = _dbSet.AsNoTracking().Include(f => f.Vendas).AsQueryable();

            if (dataInicio.HasValue && dataFim.HasValue)
            {
                if (dataInicio.Value > dataFim.Value)
                    throw new ArgumentException("A data de início deve ser anterior à data de fim.");

                query = query.Select(f => new Funcionario
                {
                    // Criar uma projeção temporária para calcular o total
                }).Select(f => f);
            }

            // Usar uma consulta mais eficiente com GroupBy
            var funcionariosComTotais = await query
                .Select(f => new
                {
                    Funcionario = f,
                    TotalVendas = dataInicio.HasValue && dataFim.HasValue
                        ? f.Vendas.Where(v => v.DataVenda >= dataInicio.Value && v.DataVenda <= dataFim.Value).Sum(v => v.Valor)
                        : f.Vendas.Sum(v => v.Valor)
                })
                .OrderByDescending(x => x.TotalVendas)
                .ThenBy(x => x.Funcionario.Nome)
                .ToListAsync();

            return funcionariosComTotais.Select(x => x.Funcionario);
        }

        /// Obtém funcionários ordenados por quantidade de vendas (decrescente).
        /// <param name="dataInicio">Data de início do período (opcional).</param>
        /// <param name="dataFim">Data de fim do período (opcional).</param>
        /// <returns>Coleção de funcionários ordenados por quantidade de vendas.</returns>
        public async Task<IEnumerable<Funcionario>> ObterRankingPorQuantidadeVendasAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var query = _dbSet.Include(f => f.Vendas).AsQueryable();

            if (dataInicio.HasValue && dataFim.HasValue)
            {
                if (dataInicio.Value > dataFim.Value)
                    throw new ArgumentException("A data de início deve ser anterior à data de fim.");
            }

            // Usar uma consulta mais eficiente com GroupBy
            var funcionariosComQuantidades = await query
                .Select(f => new
                {
                    Funcionario = f,
                    QuantidadeVendas = dataInicio.HasValue && dataFim.HasValue
                        ? f.Vendas.Count(v => v.DataVenda >= dataInicio.Value && v.DataVenda <= dataFim.Value)
                        : f.Vendas.Count()
                })
                .OrderByDescending(x => x.QuantidadeVendas)
                .ThenBy(x => x.Funcionario.Nome)
                .ToListAsync();

            return funcionariosComQuantidades.Select(x => x.Funcionario);
        }

        /// Verifica se já existe um funcionário com o nome especificado.
        /// <param name="nome">Nome do funcionário.</param>
        /// <param name="idExcluir">ID do funcionário a ser excluído da verificação (para atualizações).</param>
        /// <returns>True se já existe um funcionário com o nome, false caso contrário.</returns>
        public async Task<bool> JaExisteComNomeAsync(string nome, int? idExcluir = null)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return false;

            var nomeNormalizado = nome.ToLower().Trim();
            var query = _dbSet.Where(f => f.Nome.ToLower() == nomeNormalizado);

            if (idExcluir.HasValue)
                query = query.Where(f => f.Id != idExcluir.Value);

            return await query.AnyAsync();
        }

        /// Sobrescrita do método ObterTodosAsync para incluir ordenação por nome.
        /// <returns>Coleção de todos os funcionários ordenados por nome.</returns>
        public override async Task<IEnumerable<Funcionario>> ObterTodosAsync()
        {
            return await _dbSet
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }
    }
}
