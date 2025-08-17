using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RankingVendedores;
using RankingVendedores.Servicos.Api;
using RankingVendedores.Servicos.Interfaces;
using RankingVendedores.ViewModels;
using System.Net.Http.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Cria HttpClient temporário para ler o appsettings.json
        var tempClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
        var settings = await tempClient.GetFromJsonAsync<Dictionary<string, string>>("appsettings.json");

        // Lê a URL base da API ou usa padrão
        var apiBaseUrl = settings?["ApiBaseUrl"] ?? "https://localhost:7061/";

        // Registra HttpClient definitivo
        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(apiBaseUrl)
        });

        // Adicionar MudBlazor
        builder.Services.AddMudServices();

        // Registrar serviços
        builder.Services.AddScoped<IFuncionarioApiService, FuncionarioApiService>();
        builder.Services.AddScoped<IIndicadorApiService, IndicadorApiService>();
        builder.Services.AddScoped<IMetaApiService, MetaApiService>();
        builder.Services.AddScoped<IVendaApiService, VendaApiService>();
        builder.Services.AddScoped<IRankingApiService, RankingApiService>();

        // Registrar ViewModels
        builder.Services.AddScoped<FuncionarioViewModel>();
        builder.Services.AddScoped<IndicadorViewModel>();
        builder.Services.AddScoped<MetaViewModel>();
        builder.Services.AddScoped<VendaViewModel>();
        builder.Services.AddScoped<RankingViewModel>();

        await builder.Build().RunAsync();

    }
}
