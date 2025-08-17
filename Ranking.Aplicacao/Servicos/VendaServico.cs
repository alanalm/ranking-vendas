using Aplicacao.Utils;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Interfaces.Repositorios;

namespace Ranking.Aplicacao.Servicos
{
    public class VendaServico : IVendaServico
    {
        private readonly IRepositorioVenda _repositorioVenda;
        private readonly IRepositorioFuncionario _repositorioFuncionario;
        private readonly IValidator<CriarVendaDto> _validadorCriacao;
        private readonly IValidator<AtualizarVendaDto> _validadorAtualizacao;
        private readonly IMapper _mapper;
        private readonly ILogger<VendaServico> _logger;

        public VendaServico(
            IRepositorioVenda repositorioVenda,
            IRepositorioFuncionario repositorioFuncionario,
            IValidator<CriarVendaDto> validadorCriacao,
            IValidator<AtualizarVendaDto> validadorAtualizacao,
            IMapper mapper,
            ILogger<VendaServico> logger) 
        {
            _repositorioVenda = repositorioVenda;
            _repositorioFuncionario = repositorioFuncionario;
            _validadorCriacao = validadorCriacao;
            _validadorAtualizacao = validadorAtualizacao;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<VendaDto>> ObterTodosAsync()
        {
            var vendas = await _repositorioVenda.ObterTodos();
            var funcionarios = await _repositorioFuncionario.ObterTodos();


            var vendasDto = vendas.Select(v => new VendaDto
            {
                Id = v.Id,
                FuncionarioId = v.FuncionarioId,
                NomeFuncionario = funcionarios.FirstOrDefault(f => f.Id == v.FuncionarioId)?.Nome ?? string.Empty,
                Valor = v.Valor,
                DataVenda = v.DataVenda,
                Descricao = v.Descricao,
                DataCriacao = v.DataCriacao,
                DataAtualizacao = v.DataAtualizacao
            }).ToList();

            return vendasDto;
        }

        public async Task<ResultadoOperacao<VendaDto>> ObterPorIdAsync(int id)
        {
            var venda = await _repositorioVenda.ObterQuery()
                .Include(v => v.Funcionario)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null)
                return ResultadoOperacao<VendaDto>.CriarFalha($"Venda com ID {id} não encontrada.");

            return ResultadoOperacao<VendaDto>.CriarSucesso(_mapper.Map<VendaDto>(venda));
        }

        public async Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterPorFuncionarioAsync(int funcionarioId)
        {
            var vendas = await _repositorioVenda.ObterQuery()
                .Include(v => v.Funcionario)
                .Where(v => v.FuncionarioId == funcionarioId)
                .ToListAsync();

            return ResultadoOperacao<IEnumerable<VendaDto>>.CriarSucesso(_mapper.Map<IEnumerable<VendaDto>>(vendas));
        }

        public async Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            var vendas = await _repositorioVenda.ObterQuery()
                .Include(v => v.Funcionario)
                .Where(v => v.DataVenda >= dataInicio && v.DataVenda <= dataFim)
                .ToListAsync();

            return ResultadoOperacao<IEnumerable<VendaDto>>.CriarSucesso(_mapper.Map<IEnumerable<VendaDto>>(vendas));
        }

        public async Task<IEnumerable<VendaDto>> PesquisarAsync(string? termo, DateTime? inicio, DateTime? fim)
        {
            var query = _repositorioVenda.ObterQuery(); // já inclui Funcionario

            if (!string.IsNullOrWhiteSpace(termo))
                query = query.Where(v => v.Funcionario.Nome.Contains(termo));

            if (inicio.HasValue)
                query = query.Where(v => v.DataVenda >= inicio.Value);

            if (fim.HasValue)
                query = query.Where(v => v.DataVenda <= fim.Value);

            var resultado = await query.ToListAsync();

            return resultado.Select(v => new VendaDto
            {
                Id = v.Id,
                FuncionarioId = v.FuncionarioId,
                NomeFuncionario = v.Funcionario.Nome,
                DataVenda = v.DataVenda,
                Valor = v.Valor
            });
        }

        public async Task<ResultadoOperacao<VendaDto>> CriarAsync(CriarVendaDto dto)
        {
            var validacao = await _validadorCriacao.ValidateAsync(dto);
            if (!validacao.IsValid)
                return ResultadoOperacao<VendaDto>.CriarFalha(validacao.Errors.Select(e => e.ErrorMessage).ToList());

            var funcionarioExiste = await _repositorioFuncionario.ObterPorIdAsync(dto.FuncionarioId);
            if (funcionarioExiste == null)
                return ResultadoOperacao<VendaDto>.CriarFalha($"Funcionário com ID {dto.FuncionarioId} não encontrado.");

            var venda = _mapper.Map<Venda>(dto);
            await _repositorioVenda.AdicionarAsync(venda);

            return ResultadoOperacao<VendaDto>.CriarSucesso(_mapper.Map<VendaDto>(venda));
        }

        public async Task<ResultadoOperacao<VendaDto>> AtualizarAsync(AtualizarVendaDto dto)
        {
            var validacao = await _validadorAtualizacao.ValidateAsync(dto);
            if (!validacao.IsValid)
                return ResultadoOperacao<VendaDto>.CriarFalha(validacao.Errors.Select(e => e.ErrorMessage).ToList());

            var venda = await _repositorioVenda.ObterPorIdAsync(dto.Id);
            if (venda == null)
                return ResultadoOperacao<VendaDto>.CriarFalha($"Venda com ID {dto.Id} não encontrada.");

            var funcionarioExiste = await _repositorioFuncionario.ObterPorIdAsync(dto.FuncionarioId);
            if (funcionarioExiste == null)
                return ResultadoOperacao<VendaDto>.CriarFalha($"Funcionário com ID {dto.FuncionarioId} não encontrado.");

            _mapper.Map(dto, venda);
            await _repositorioVenda.AtualizarAsync(venda);

            return ResultadoOperacao<VendaDto>.CriarSucesso(_mapper.Map<VendaDto>(venda));
        }

        public async Task<ResultadoOperacao> RemoverAsync(int id)
        {
            var venda = await _repositorioVenda.ObterPorIdAsync(id);
            if (venda == null)
                return ResultadoOperacao.CriarFalha($"Venda com ID {id} não encontrada.");

            await _repositorioVenda.RemoverAsync(id);
            return ResultadoOperacao.CriarSucesso();
        }

        public async Task<EstatisticasVendasDto> CalcularEstatisticasAsync(DateTime inicio, DateTime? fim)
        {
            var vendas = await _repositorioVenda.ObterQuery()
                .Where(v => v.DataVenda >= inicio && (!fim.HasValue || v.DataVenda <= fim.Value))
                .ToListAsync();

            var dto = new EstatisticasVendasDto
            {
                TotalVendas = vendas.Count,
                ValorTotal = vendas.Sum(v => v.Valor),
                ValorMedio = vendas.Any() ? vendas.Average(v => v.Valor) : 0,
                VendasHoje = vendas.Count(v => v.DataVenda.Date == DateTime.Today),
                VendasUltimos7Dias = vendas.Count(v => v.DataVenda >= DateTime.Today.AddDays(-7)),
                MelhorFuncionarioNome = vendas
                    .Where(v => v.Funcionario != null)
                    .GroupBy(v => v.FuncionarioId)
                    .OrderByDescending(g => g.Sum(v => v.Valor))
                    .Select(g => g.First().Funcionario!.Nome)
                    .FirstOrDefault() ?? "N/A",
                MelhorFuncionarioValor = vendas.Any()
                    ? vendas.GroupBy(v => v.FuncionarioId).Max(g => g.Sum(v => v.Valor))
                    : 0m
            };

            return dto;
        }

        public async Task<EstatisticasVendasDto> ObterEstatisticasVendasAsync(DateTime? dataInicio, DateTime? dataFim)
        {
            var vendas = await _repositorioVenda.ObterVendasPorPeriodoAsync(dataInicio, dataFim);

            var quantidade = vendas.Count;
            var valorTotal = vendas.Sum(v => v.Valor);
            var valorMedio = quantidade > 0 ? valorTotal / quantidade : 0m;

            return new EstatisticasVendasDto
            {
                QuantidadeVendas = quantidade,
                ValorTotal = valorTotal,
                ValorMedio = valorMedio
            };
        }


        // Métodos que ainda serão implementados
        public Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterPorFuncionarioEPeriodoAsync(int funcionarioId, DateTime dataInicio, DateTime dataFim)
            => Task.FromResult(ResultadoOperacao<IEnumerable<VendaDto>>.CriarFalha("Não implementado."));

        public Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterPorMesAsync(int mes, int ano)
            => Task.FromResult(ResultadoOperacao<IEnumerable<VendaDto>>.CriarFalha("Não implementado."));

        public Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterPorAnoAsync(int ano)
            => Task.FromResult(ResultadoOperacao<IEnumerable<VendaDto>>.CriarFalha("Não implementado."));

        public Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterMaioresVendasAsync(int quantidade, DateTime? dataInicio = null, DateTime? dataFim = null)
            => Task.FromResult(ResultadoOperacao<IEnumerable<VendaDto>>.CriarFalha("Não implementado."));

        public Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterVendasRecentesAsync(int? quantidade = null)
            => Task.FromResult(ResultadoOperacao<IEnumerable<VendaDto>>.CriarFalha("Não implementado."));

        public Task<ResultadoOperacao<Dictionary<int, (decimal TotalVendas, int QuantidadeVendas)>>> CalcularEstatisticasPorFuncionarioAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
            => Task.FromResult(ResultadoOperacao<Dictionary<int, (decimal, int)>>.CriarFalha("Não implementado."));

        public Task<ResultadoOperacao<decimal>> CalcularTotalVendasPorFuncionarioAsync(int funcionarioId, DateTime? dataInicio = null, DateTime? dataFim = null)
            => Task.FromResult(ResultadoOperacao<decimal>.CriarFalha("Não implementado."));

        public Task<ResultadoOperacao<int>> CalcularQuantidadeVendasPorFuncionarioAsync(int funcionarioId, DateTime? dataInicio = null, DateTime? dataFim = null)
            => Task.FromResult(ResultadoOperacao<int>.CriarFalha("Não implementado."));
    }
}
