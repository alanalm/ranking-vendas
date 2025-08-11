using Microsoft.EntityFrameworkCore;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Interfaces.Repositorios;
using Ranking.Infraestrutura.Persistencia;

namespace Ranking.Infraestrutura.Repositorios
{
    /// Implementação do repositório de vendas.
    /// Herda operações básicas e implementa operações específicas para vendas.
    public class VendaRepositorio : RepositorioBase<Venda>, IRepositorioVenda
    {

        public IQueryable<Venda> ObterQuery()
        {
            return _contexto.Vendas.Include(v => v.Funcionario);
        }

        public async Task<List<Venda>> ObterTodos()
        {
            var lista = await _contexto.Vendas.ToListAsync();
            Console.WriteLine($"[RepositorioVenda] Total vendas carregadas do banco: {lista.Count}");
            return lista;
        }

        /// Construtor que recebe o contexto do Entity Framework.
        /// <param name="contexto">Contexto do Entity Framework.</param>
        public VendaRepositorio(RankingVendasContexto contexto) : base(contexto)
        {
        }

        /// Obtém vendas por funcionário.
        /// <param name="funcionarioId">Identificador do funcionário.</param>
        /// <returns>Coleção de vendas do funcionário.</returns>
        public async Task<IEnumerable<Venda>> ObterPorFuncionarioAsync(int funcionarioId)
        {
            if (funcionarioId <= 0)
                return Enumerable.Empty<Venda>();

            return await _dbSet
                .Where(v => v.FuncionarioId == funcionarioId)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }

        /// Obtém uma venda com o funcionário carregado.
        /// <param name="id">Identificador da venda.</param>
        /// <returns>Venda com funcionário carregado ou null se não existir.</returns>
        public async Task<Venda?> ObterComFuncionarioAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _dbSet
                .Include(v => v.Funcionario)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        /// <summary>
        /// Obtém todas as vendas com funcionários carregados.
        /// </summary>
        /// <returns>Coleção de vendas com funcionários carregados.</returns>
        public async Task<IEnumerable<Venda>> ObterTodosComFuncionarioAsync()
        {
            return await _dbSet
                .Include(v => v.Funcionario)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém vendas realizadas no período especificado.
        /// </summary>
        /// <param name="dataInicio">Data de início do período.</param>
        /// <param name="dataFim">Data de fim do período.</param>
        /// <returns>Coleção de vendas realizadas no período.</returns>
        public async Task<IEnumerable<Venda>> ObterPorPeriodoAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var query = _dbSet.Include(v => v.Funcionario).AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(v => v.DataVenda >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(v => v.DataVenda <= dataFim.Value);

            return await query
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém vendas de um funcionário no período especificado.
        /// </summary>
        /// <param name="funcionarioId">Identificador do funcionário.</param>
        /// <param name="dataInicio">Data de início do período.</param>
        /// <param name="dataFim">Data de fim do período.</param>
        /// <returns>Coleção de vendas do funcionário no período.</returns>
        public async Task<IEnumerable<Venda>> ObterPorFuncionarioEPeriodoAsync(int funcionarioId, DateTime dataInicio, DateTime dataFim)
        {
            if (funcionarioId <= 0)
                return Enumerable.Empty<Venda>();

            if (dataInicio > dataFim)
                throw new ArgumentException("A data de início deve ser anterior à data de fim.");

            return await _dbSet
                .Include(v => v.Funcionario)
                .Where(v => v.FuncionarioId == funcionarioId &&
                           v.DataVenda >= dataInicio &&
                           v.DataVenda <= dataFim)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém vendas realizadas em um mês específico.
        /// </summary>
        /// <param name="mes">Mês (1-12).</param>
        /// <param name="ano">Ano.</param>
        /// <returns>Coleção de vendas realizadas no mês.</returns>
        public async Task<IEnumerable<Venda>> ObterPorMesAsync(int mes, int ano)
        {
            if (mes < 1 || mes > 12)
                throw new ArgumentException("O mês deve estar entre 1 e 12.", nameof(mes));

            if (ano < 1900 || ano > DateTime.UtcNow.Year + 10)
                throw new ArgumentException("Ano inválido.", nameof(ano));

            return await _dbSet
                .Include(v => v.Funcionario)
                .Where(v => v.DataVenda.Month == mes && v.DataVenda.Year == ano)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém vendas realizadas em um ano específico.
        /// </summary>
        /// <param name="ano">Ano.</param>
        /// <returns>Coleção de vendas realizadas no ano.</returns>
        public async Task<IEnumerable<Venda>> ObterPorAnoAsync(int ano)
        {
            if (ano < 1900 || ano > DateTime.UtcNow.Year + 10)
                throw new ArgumentException("Ano inválido.", nameof(ano));

            return await _dbSet
                .Include(v => v.Funcionario)
                .Where(v => v.DataVenda.Year == ano)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venda>> PesquisarAsync(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return await ObterTodosComFuncionarioAsync();

            texto = texto.ToLower();

            return await _dbSet
                .Include(v => v.Funcionario)
                .Where(v =>
                    v.Descricao.ToLower().Contains(texto) ||
                    v.Funcionario.Nome.ToLower().Contains(texto))
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }


        /// <summary>
        /// Calcula o total de vendas por funcionário no período especificado.
        /// </summary>
        /// <param name="funcionarioId">Identificador do funcionário.</param>
        /// <param name="dataInicio">Data de início do período (opcional).</param>
        /// <param name="dataFim">Data de fim do período (opcional).</param>
        /// <returns>Valor total das vendas do funcionário no período.</returns>
        public async Task<decimal> CalcularTotalVendasPorFuncionarioAsync(int funcionarioId, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            if (funcionarioId <= 0)
                return 0;

            var query = _dbSet.Where(v => v.FuncionarioId == funcionarioId);

            if (dataInicio.HasValue)
                query = query.Where(v => v.DataVenda >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(v => v.DataVenda <= dataFim.Value);

            return await query.SumAsync(v => v.Valor);
        }

        /// <summary>
        /// Calcula a quantidade de vendas por funcionário no período especificado.
        /// </summary>
        /// <param name="funcionarioId">Identificador do funcionário.</param>
        /// <param name="dataInicio">Data de início do período (opcional).</param>
        /// <param name="dataFim">Data de fim do período (opcional).</param>
        /// <returns>Quantidade de vendas do funcionário no período.</returns>
        public async Task<int> CalcularQuantidadeVendasPorFuncionarioAsync(int funcionarioId, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            if (funcionarioId <= 0)
                return 0;

            var query = _dbSet.Where(v => v.FuncionarioId == funcionarioId);

            if (dataInicio.HasValue)
                query = query.Where(v => v.DataVenda >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(v => v.DataVenda <= dataFim.Value);

            return await query.CountAsync();
        }

        /// <summary>
        /// Obtém estatísticas de vendas por funcionário.
        /// </summary>
        /// <param name="dataInicio">Data de início do período (opcional).</param>
        /// <param name="dataFim">Data de fim do período (opcional).</param>
        /// <returns>Dicionário com estatísticas por funcionário (ID do funcionário, total de vendas, quantidade).</returns>
        public async Task<Dictionary<int, (decimal TotalVendas, int QuantidadeVendas)>> ObterEstatisticasPorFuncionarioAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var query = _dbSet.AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(v => v.DataVenda >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(v => v.DataVenda <= dataFim.Value);

            // Validação das datas
            if (dataInicio.HasValue && dataFim.HasValue && dataInicio > dataFim)
                throw new ArgumentException("A data de início não pode ser posterior à data de fim.");


            var estatisticas = await query
                .GroupBy(v => v.FuncionarioId)
                .Select(g => new
                {
                    FuncionarioId = g.Key,
                    TotalVendas = g.Sum(v => v.Valor),
                    QuantidadeVendas = g.Count()
                })
                .ToListAsync();

            return estatisticas.ToDictionary(
                e => e.FuncionarioId,
                e => (e.TotalVendas, e.QuantidadeVendas)
            );
        }

        /// <summary>
        /// Obtém as maiores vendas do período.
        /// </summary>
        /// <param name="quantidade">Quantidade de vendas a retornar.</param>
        /// <param name="dataInicio">Data de início do período (opcional).</param>
        /// <param name="dataFim">Data de fim do período (opcional).</param>
        /// <returns>Coleção das maiores vendas ordenadas por valor decrescente.</returns>
        public async Task<IEnumerable<Venda>> ObterMaioresVendasAsync(int quantidade, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            if (quantidade <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));

            var query = _dbSet.Include(v => v.Funcionario).AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(v => v.DataVenda >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(v => v.DataVenda <= dataFim.Value);

            return await query
                .OrderByDescending(v => v.Valor)
                .ThenByDescending(v => v.DataVenda)
                .Take(quantidade)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém vendas ordenadas por data (mais recentes primeiro).
        /// </summary>
        /// <param name="quantidade">Quantidade de vendas a retornar (opcional).</param>
        /// <returns>Coleção de vendas ordenadas por data decrescente.</returns>
        public async Task<IEnumerable<Venda>> ObterVendasRecentesAsync(int? quantidade = null)
        {
            var query = _dbSet
                .Include(v => v.Funcionario)
                .OrderByDescending(v => v.DataVenda)
                .ThenByDescending(v => v.Id);

            if (quantidade.HasValue && quantidade.Value > 0)
                query = (IOrderedQueryable<Venda>)query.Take(quantidade.Value);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Sobrescrita do método ObterTodosAsync para incluir ordenação por data.
        /// </summary>
        /// <returns>Coleção de todas as vendas ordenadas por data decrescente.</returns>
        public override async Task<IEnumerable<Venda>> ObterTodosAsync()
        {
            return await _dbSet
                .Include(v => v.Funcionario)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }

        public async Task<List<Venda>> ObterVendasPorPeriodoAsync(DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _contexto.Vendas.AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(v => v.DataVenda >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(v => v.DataVenda <= dataFim.Value);

            return await query.ToListAsync(); 
        }

        public async Task<bool> TemVendasAsync(int funcionarioId)
        {
            return await _dbSet.AnyAsync(v => v.FuncionarioId == funcionarioId);
        }
    }
}
