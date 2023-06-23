using Discord;
using Discord.WebSocket;

namespace Bot.Comandos
{
    public class ComandoGpt : ComandoBase
    {
        Dictionary<long, string> modos = new Dictionary<long, string>()
        {
            { 1, "Responda todas as respostas normal" },
            { 2, "Responda todas as respostas como uma Tsundere" }
        };
        public ComandoGpt()
        {
            Name = "gpt";
            Description = "Manda uma mensagem para o ChatGPT Responder!";
        }

        public override SlashCommandBuilder MontaComando()
        {
            var globalCommand = base.MontaComando();
            globalCommand.AddOption(new SlashCommandOptionBuilder()
                            .WithName("modo")
                            .WithDescription("Modo como o GPT irá responder")
                            .WithRequired(true)
                            .AddChoice("Padrão", 1)
                            .AddChoice("Tsundere", 2)
                            .WithType(ApplicationCommandOptionType.Integer));
            globalCommand.AddOption("mensagem", ApplicationCommandOptionType.String, "Mensagem que será enviada para ChatGPT", isRequired: true);

            return globalCommand;
        }

        public override async Task Executar(SocketSlashCommand comando)
        {
            await base.Executar(comando);

            if (comando.User.Id != 240234921254191104 && comando.User.Id != 1031543099824033802)
            {
                await comando.ModifyOriginalResponseAsync(x => x.Content = "Sem Permissão");
                return;
            }

            var msg = comando.Data.Options.ElementAt(1).Value.ToString();

            var chat = new ConfigBot().ChatGPT();
            var modo = modos.GetValueOrDefault((long)comando.Data.Options.First().Value);
            chat.AppendSystemMessage(modo);
            chat.AppendUserInput(msg);
            var response = await chat.GetResponseFromChatbotAsync();

            var embedBuilder = new EmbedBuilder()
                .WithAuthor(comando.User)
                .WithTitle("ChatGPT")
                .WithDescription("Mensagem: " + msg + "\n" + "ChatGPT: " + response)
                .WithColor(Color.Green)
                .WithCurrentTimestamp();

            await comando.ModifyOriginalResponseAsync(x => x.Embed = embedBuilder.Build());
        }
    }
}
