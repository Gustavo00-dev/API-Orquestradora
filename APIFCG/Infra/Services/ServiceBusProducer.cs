using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using APIFCG.Infra.Model;
using APIFCG.Infra.LogAPI;

namespace APIFCG.Infra.Services
{
    public class ServiceBusProducer : IJogoProducer
    {
        private readonly ServiceBusClient _client;
        private readonly string _queueName;
        private readonly BaseLogger<ServiceBusProducer> _logger;

        public ServiceBusProducer(ServiceBusClient client, IConfiguration configuration, BaseLogger<ServiceBusProducer> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _queueName = configuration?["ServiceBus:QueueName"] ?? throw new ArgumentNullException("ServiceBus:QueueName");
            _logger = logger;
        }

        public async Task EnqueueCadastrarJogoAsync(JogoMessage jogoMessage)
        {
            try
            {
                var sender = _client.CreateSender(_queueName);
                var json = JsonSerializer.Serialize(jogoMessage);
                var message = new ServiceBusMessage(json)
                {
                    ContentType = "application/json"
                };

                await sender.SendMessageAsync(message);
                _logger.LogInformation($"Mensagem para jogo {jogoMessage.Nome} enviada para fila {_queueName}.", "ServiceBus");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao enviar mensagem para fila: {ex.Message}", "ServiceBus");
                throw;
            }
        }
    }
}