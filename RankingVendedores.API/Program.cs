using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Ranking.Aplicacao.Interfaces;
using Ranking.Aplicacao.Servicos;
using Ranking.Aplicacao.Validacoes;
using Ranking.Aplicacao.Mapeamento;
using Ranking.Dominio.Interfaces.Repositorios;
using Ranking.Infraestrutura.Persistencia;
using Ranking.Infraestrutura.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Ranking API", Version = "v1" });
});

// Configurar Entity Framework
builder.Services.AddDbContext<RankingVendasContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Registrar repositórios
builder.Services.AddScoped<IRepositorioFuncionario, FuncionarioRepositorio>();
builder.Services.AddScoped<IRepositorioIndicador, IndicadorRepositorio>();
builder.Services.AddScoped<IRepositorioMeta, MetaRepositorio>();
builder.Services.AddScoped<IRepositorioVenda, VendaRepositorio>();

// Registrar serviços
builder.Services.AddScoped<IFuncionarioServico, FuncionarioServico>();
builder.Services.AddScoped<IIndicadorServico, IndicadorServico>();
builder.Services.AddScoped<IMetaServico, MetaServico>();
builder.Services.AddScoped<IVendaServico, VendaServico>();
builder.Services.AddScoped<IRankingServico, RankingServico>();
builder.Services.AddScoped<ICalculadoraDesempenhoServico, CalculadoraDesempenhoServico>();
builder.Services.AddScoped<IConfiguracoesService, ConfiguracoesService>();

// Configurar FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorCriarFuncionario>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorAtualizarFuncionario>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorCriarIndicador>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorAtualizarIndicador>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorCriarMeta>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorAtualizarMeta>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorCriarVenda>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorAtualizarVenda>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorFiltroRanking>();

// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(PerfilDeMapeamento));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// EF
app.Services.CreateScope().ServiceProvider
    .GetRequiredService<RankingVendasContexto>()
    .Database.EnsureCreated();

app.Run();
