using System.Reflection;
using Discord.WebSocket;

namespace Bot.Comandos
{
    public class GerenciadorComandos
    {
        public async Task RegistrarComandos(DiscordSocketClient client)
        {
            List<Type> comandos = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Name.Length > 6 && t.Name.Substring(0, 7).Equals("Comando")).ToList();

            foreach (Type comando in comandos)
            {
                if (comando.Name == "ComandoBase")
                    continue;
                ComandoBase? instanciaComando = Activator.CreateInstance(comando) as ComandoBase;

                await client.CreateGlobalApplicationCommandAsync(instanciaComando!.MontaComando().Build());
            }

        }
        public async Task ProcessarComando(SocketSlashCommand comando)
        {
            var nomeComando = comando.Data.Name;

            Type tipoComando = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(t => t.Name.Equals("Comando" + nomeComando, StringComparison.OrdinalIgnoreCase))!;

            if (tipoComando != null && tipoComando.IsSubclassOf(typeof(ComandoBase)))
            {
                ComandoBase? instanciaComando = Activator.CreateInstance(tipoComando) as ComandoBase;
                await instanciaComando!.Executar(comando);
            }
            else
                await comando.RespondAsync("Comando inexistente!");
        }
    }
}
