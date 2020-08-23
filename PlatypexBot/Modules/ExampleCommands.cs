using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatypexBot.Modules
{
    public partial class ExampleCommands : ModuleBase<SocketCommandContext>
    {
        IGuildUser currentUser;
        [Command("Platy")]
        public async Task What() => await Context.Channel.SendMessageAsync("what?");

        [Command("help")]
        public async Task Help()
        {
            var embed = new EmbedBuilder();
            var sb = new StringBuilder();

            embed.WithColor(new Color(0, 255, 0));
            embed.Title = $"Hello {Context.Message.Author.ToString().Remove(Context.Message.Author.ToString().Length-5)}!";

            sb.AppendLine($"Use `pp [command]` to use any command.");
            sb.AppendLine($":tools: `Configuration`: \n" +
                $"`config (set chan where bot'll send welcome/goodbye messages)`");
            sb.AppendLine($":wrench: `Moderation`: \n" +
                $"`kick, ban, getroles, deletechannel, deletemessages, ownnick (change your own nick), nick (change sb's nick)`");
            embed.Description = sb.ToString();
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("config")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Config()
        {
            Program.client.MessageReceived += Client_MessageReceived;
            await Context.Channel.SendMessageAsync("Where i should send welcome/goodbye messages?");
            currentUser = Context.User as IGuildUser;
        }

        public  async Task Client_MessageReceived(SocketMessage arg)
        {
            if (arg.Content.All(x => char.IsDigit(x)) & arg.Content.Length == 18)
            {
                if (arg.Author == currentUser)
                {
                    await arg.AddReactionAsync(new Emoji("\uD83D\uDC4D"));
                    Program.WelcomeID = Convert.ToUInt64(arg.Content);
                    Program.client.MessageReceived -= Client_MessageReceived;
                    Console.WriteLine($"{Program.WelcomeID}");
                }
            }
        }
    }
}
