using AutoMapper;
using Ranking.Aplicacao.DTOs;
using Ranking.Dominio.Entidades;

namespace Ranking.Aplicacao.Mapeamento
{
    public class PerfilDeMapeamento : Profile
    {
        public PerfilDeMapeamento()
        {
            // Funcionario
            CreateMap<Funcionario, FuncionarioDto>();
            CreateMap<FuncionarioDto, Funcionario>();

            CreateMap<CriarFuncionarioDto, Funcionario>()
                .ConstructUsing(dto => new Funcionario(dto.Nome));

            CreateMap<AtualizarFuncionarioDto, Funcionario>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome));

            // Indicador
            CreateMap<Indicador, IndicadorDto>();
            CreateMap<IndicadorDto, Indicador>();

            CreateMap<CriarIndicadorDto, Indicador>()
                .ConstructUsing(dto => new Indicador(dto.Nome, dto.Descricao, dto.Tipo));

            CreateMap<AtualizarIndicadorDto, Indicador>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao));

            // Meta
            CreateMap<Meta, MetaDto>();
            CreateMap<MetaDto, Meta>();

            CreateMap<CriarMetaDto, Meta>()
                .ConstructUsing(dto => new Meta(
                    dto.IndicadorId,
                    dto.Valor,
                    dto.Ativa,
                    dto.DataInicio ?? DateTime.Now, // erro se não for aceitável
                    dto.DataFim ?? DateTime.Now     
                ));

            CreateMap<AtualizarMetaDto, Meta>()
                .ForMember(dest => dest.IndicadorId, opt => opt.MapFrom(src => src.IndicadorId))
                .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor))
                .ForMember(dest => dest.Ativa, opt => opt.MapFrom(src => src.Ativa))
                .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => src.DataInicio.GetValueOrDefault()))
                .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim.GetValueOrDefault()));

            //Venda
            CreateMap<VendaDto, Venda>();
            CreateMap<Venda, VendaDto>();

            CreateMap<CriarVendaDto, Venda>()
                .ConstructUsing(dto => new Venda(
                    dto.FuncionarioId,
                    dto.Valor,
                    dto.DataVenda ?? DateTime.Now, // erro se não for aceitável
                    dto.Descricao ?? string.Empty
                ));

            CreateMap<AtualizarVendaDto, Venda>()
                .ForMember(dest => dest.FuncionarioId, opt => opt.MapFrom(src => src.FuncionarioId))
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao ?? string.Empty))
                .ForMember(dest => dest.DataVenda, opt => opt.MapFrom(src => src.DataVenda.GetValueOrDefault()))
                .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor));
        }
    }
}
