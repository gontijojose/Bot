using Bot.Comandos;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using OpenAI_API.Chat;

namespace Bot
{
    public class ConfigBot
    {
        public DiscordSocketClient? _client;
        public GerenciadorComandos? _gerenciadorComandos;
        public IConfiguration? _configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        protected async Task ConfiguraBot()
        {
            DiscordSocketConfig discordSocketConfig = new()
            {
                UseInteractionSnowflakeDate = false,
                //GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };
            _client = new DiscordSocketClient(discordSocketConfig);

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

        public Conversation ChatGPT()
        {
            var api = new OpenAI_API.OpenAIAPI(_configuration!["GPT_KEY"]);
            var chatRequest = new ChatRequest()
            {
                Temperature = 0.9,
                MaxTokens = 100,
            };
            return api.Chat.CreateConversation(chatRequest);
        }
    }
}
