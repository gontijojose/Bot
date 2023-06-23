using Discord.WebSocket;

namespace Bot.Comandos
{
    public class ComandoPing : ComandoBase
    {
        public ComandoPing()
        {
            Name = "ping";
            Description = "Diz Pong!";
        }

        public override async Task Executar(SocketSlashCommand comando)
        {
            await base.Executar(comando);
            await comando.ModifyOriginalResponseAsync(x => x.Content = "Pong!");
        }
    }
}
