//using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeradorDeCertificados.Aplicacao;

public static class DependencyInjection
{
    public static void AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        var rabbitMqConnectionString = configuration.GetConnectionString("RabbitMq")
            ?? throw new InvalidOperationException("A ConnectionString \"RabbitMq\" não foi configurada");

        services.AddMassTransit(config =>
        {
            // Configura a injeção dos Consumers
            //config.AddConsumer<>();

            config.UsingRabbitMq((context, rabbitMq) =>
            {
                rabbitMq.Host(new Uri(rabbitMqConnectionString));

                rabbitMq.ReceiveEndpoint("certificados-criados", endpoint =>
                {
                    endpoint.PrefetchCount = 4; // Quantas mensagens o RabbitMQ deve carregar adiantado
                    endpoint.ConcurrentMessageLimit = 2; // Quantos consumers serão instanciados em paralelo

                    //endpoint.ConfigureConsumer<>(context);
                });
            });
        });

        services.Configure<MassTransitHostOptions>(options =>
        {
            options.WaitUntilStarted = true;
            options.StartTimeout = TimeSpan.FromSeconds(30);
        });
    }
}