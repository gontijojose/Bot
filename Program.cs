using Bot;
using Discord.WebSocket;

class Program : ConfigBot
{
    public static void Main(string[] args)
    {
        var program = new Program();
        program.RunBotAsync().GetAwaiter().GetResult();
    }

    private async Task RunBotAsync()
    {
        await ConfiguraBot();

        _client!.SlashCommandExecuted += SlashCommandHandler;

        await Task.Delay(-1);
    }

    private async Task SlashCommandHandler(SocketSlashCommand comando)
    {
        await _gerenciadorComandos!.ProcessarComando(comando);
    }
}
