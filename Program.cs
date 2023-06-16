using Discord;
using Discord.WebSocket;
using OpenAI_API;
using OpenAI_API.Chat;

class Program
{
    private DiscordSocketClient? _client;
    private OpenAIAPI? _api;
    private Conversation? _chat;

    static void Main(string[] args)
        => new Program().RunBotAsync().GetAwaiter().GetResult();

    public async Task RunBotAsync()
    {
        await ConfigBot();

        _client!.MessageReceived += HandleMessageReceived;

        await Task.Delay(-1);
    }

    private async Task ConfigBot()
    {
        DiscordSocketConfig discordSocketConfig = new()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };

        _client = new DiscordSocketClient(discordSocketConfig);

        _api = new OpenAI_API.OpenAIAPI("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");

        var chatRequest = new ChatRequest()
        {
            Temperature = 1,
            MaxTokens = 100,
        };

        _chat = _api.Chat.CreateConversation(chatRequest);

        _client.Log += Log;

        await _client.LoginAsync(TokenType.Bot, "MTExODIzNzAyNDI2OTM4MTgzMw.GYUNAc.QfnmqJVzCAl2jFVLGn36PL_AjXnNlgdkKy-0tk");

        await _client.StartAsync();
    }

    private Task Log(LogMessage arg)
    {
        Console.WriteLine(arg);
        return Task.CompletedTask;
    }

    private async Task HandleMessageReceived(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        var msg = message.Content;

        if(msg.Substring(0, 3) == "/t ")
            _chat!.AppendSystemMessage("Responda todas as respostas como uma Tsundere");

        _chat!.AppendUserInput(msg);

        var response = await _chat.GetResponseFromChatbotAsync();

        await message.Channel.SendMessageAsync(response);
    }
}
