using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;
using System.Reflection;

namespace PlatypexBot
{
    class Commands
    {
        DiscordSocketClient client;
        CommandService service;

        public Commands(DiscordSocketClient client)
        {
            this.client = client;
            service = new CommandService();
            service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            client.MessageReceived += HandleCommandAsync;
        }

        async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage ms = arg as SocketUserMessage;
            if (ms == null) return;
            SocketCommandContext context = new SocketCommandContext(client, ms);

            int posArg = 0;
            if (ms.HasStringPrefix("pp ", ref posArg))
            {
                var result = await service.ExecuteAsync(context, posArg, null);
                if (!result.IsSuccess) await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
