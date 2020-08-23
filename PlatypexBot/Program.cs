using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
namespace PlatypexBot
{
    class Program
    {
        public static ulong WelcomeID;
        public static DiscordSocketClient client;
        Commands commands;

        static void Main(string[] args)
        {
            WelcomeID = 0;
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        public async Task StartAsync()
        {
            client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.Bot, "cool_token");
            await client.StartAsync();
            commands = new Commands(client);

            client.UserJoined += Client_UserJoined;
            client.UserLeft += Client_UserLeft;
            await Task.Delay(-1);
        }

        public async Task Client_UserLeft(SocketGuildUser arg)
        {
            SocketTextChannel channel;
            if (WelcomeID != 0)
            {
                channel = client.GetChannel(WelcomeID) as SocketTextChannel;
                await channel.SendMessageAsync($":wave: Bye {arg.Mention}!");
            }
            else Console.WriteLine("WelcomeID not found.");
        }

        public async Task Client_UserJoined(SocketGuildUser arg)
        {
            SocketTextChannel channel;
            if (WelcomeID != 0)
            {
                channel = client.GetChannel(WelcomeID) as SocketTextChannel;
                await channel.SendMessageAsync($"Hello {arg.Mention}!");
            }
            else Console.WriteLine("WelcomeID not found.");
        }
    }
}
