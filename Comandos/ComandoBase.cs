using Discord;
using Discord.WebSocket;

namespace Bot.Comandos
{
    public abstract class ComandoBase
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public virtual Task Executar(SocketSlashCommand comando)
        {
            return comando.DeferAsync();
        }

        public virtual SlashCommandBuilder MontaComando()
        {
            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName(Name);
            globalCommand.WithDescription(Description);

            return globalCommand;
        }
    }
}
