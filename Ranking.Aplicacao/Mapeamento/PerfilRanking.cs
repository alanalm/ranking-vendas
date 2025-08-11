using AutoMapper;
using Ranking.Aplicacao.DTOs;
using Ranking.Dominio.Entidades;

namespace Ranking.Aplicacao.Mapeamento
{
    public class PerfilRanking : Profile
    {
        public PerfilRanking()
        {
            // Mapeia Funcionario -> FuncionarioDto (já deve existir)
            CreateMap<Funcionario, FuncionarioDto>();

            // Mapeia DadosRanking -> RankingDto (dados já calculados)
            CreateMap<DadosRanking, RankingDto>()
                .ForMember(dest => dest.Funcionario, opt => opt.MapFrom(src => src.Funcionario))
                .ForMember(dest => dest.Vendas, opt => opt.MapFrom(src => src.Vendas))
                .ForMember(dest => dest.Metas, opt => opt.MapFrom(src => src.Metas))
                .ForMember(dest => dest.Desempenho, opt => opt.MapFrom(src => src.Desempenho))
                .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => src.DataInicio))
                .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim));
        }
    }
}
