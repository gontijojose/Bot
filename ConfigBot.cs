using Bot.Comandos;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI_API.Chat;

namespace Bot
{
    public class ConfigBot
    {
        public DiscordSocketClient? _client;
        public GerenciadorComandos? _gerenciadorComandos;
        public DiscordSocketConfig? _discordSocketConfig = new()
        {
            UseInteractionSnowflakeDate = false,
            //GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };
        public IConfiguration? _configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        private readonly IServiceProvider _serviceProvider;
        public ConfigBot()
        {
            _serviceProvider = CreateProvider();
        }

        private IServiceProvider CreateProvider()
        {
            var collection = new ServiceCollection()
                .AddSingleton(_discordSocketConfig!)
                .AddSingleton<DiscordSocketClient>();

            return collection.BuildServiceProvider();
        }

        protected async Task ConfiguraBot()
        {
            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();

            _gerenciadorComandos = new GerenciadorComandos();

            _client.Log += Log;
            _client.Ready += Client_Ready;
            await _client.LoginAsync(TokenType.Bot, _configuration!["DISCORD_KEY"]);
            await _client.StartAsync();
        }
        private async Task Client_Ready()
        {
            await _gerenciadorComandos!.RegistrarComandos(_client!);
        }
        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public Conversation ChatGPT(long versao)
        {
            var api = new OpenAI_API.OpenAIAPI(_configuration!["GPT_KEY"]);
            var chatRequest = new ChatRequest()
            {
                Temperature = 1,
                Model = versao == 1 ? "gpt-3.5-turbo-1106" : "gpt-4-1106-preview"
            };
            return api.Chat.CreateConversation(chatRequest);
        }
    }
}
