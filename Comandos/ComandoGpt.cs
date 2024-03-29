﻿using Discord;
using Discord.WebSocket;

namespace Bot.Comandos
{
    public class ComandoGpt : ComandoBase
    {
        Dictionary<long, string> modos = new Dictionary<long, string>()
        {
            { 1, "Responda todas as respostas normal" },
            { 2, "Responda todas as respostas como o Yoda de Star Wars" }
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
                            .AddChoice("Yoda", 2)
                            .WithType(ApplicationCommandOptionType.Integer));
            globalCommand.AddOption(new SlashCommandOptionBuilder()
                .WithName("versão")
                .WithDescription("Versão utilizada do GPT")
                .WithRequired(true)
                .AddChoice("3.5", 1)
                .AddChoice("4", 2)
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

            var msg = comando.Data.Options.ElementAt(2).Value.ToString();

            var versao = (long)comando.Data.Options.ElementAt(1).Value;

            var chat = new ConfigBot().ChatGPT(versao);
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
