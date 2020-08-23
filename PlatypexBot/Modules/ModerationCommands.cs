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
        IRole[] avRoles;
        List<IUserMessage> mess = new List<IUserMessage>();

        [Command("kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickAsync(IGuildUser user)
        {
            var GuildUser = Context.Guild.GetUser(Context.User.Id);
            if (!GuildUser.GuildPermissions.KickMembers)
            {
                await Context.Message.DeleteAsync();
                await ReplyAsync("`I cannot kick this player.`");
                return;
            }
            else
                await user.KickAsync();
        }

        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser user, string reason)
        {
            await Context.Guild.AddBanAsync(user, 0, reason);
            await ReplyAsync($"User {user.Mention} has been banned.");
        }

        public async Task BanID(ulong userID, string reason)
        {
            await Context.Guild.AddBanAsync(userID, 0, reason);
            await ReplyAsync($"User {Context.Guild.Users.ToList().FirstOrDefault(x=> x.Id == userID)} has been banned.");
        }

        [Command("unban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Unban(IGuildUser user, string reason)
        {
            await user.Guild.RemoveBanAsync(user);
            await ReplyAsync($"{user.Username} has been unbanned.");
        }

        public async Task UnbanID(ulong userID, string reason)
        {
            await Context.Guild.RemoveBanAsync(userID);
            await ReplyAsync($"{Context.Guild.Users.ToList().FirstOrDefault(x => x.Id == userID)} has been unbanned.");
        }

        // manual management
        [Command("Role"), Alias("giverole")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task SetNewRole(IGuildUser user, IRole role) => await user.AddRoleAsync(role);

        [Command("getroles")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task AvailableRoles(string title, params IRole[] roles)
        {
            avRoles = roles;
            var embed = new EmbedBuilder();
            var sb = new StringBuilder();
            embed.Title = title;
            embed.WithColor(new Color(0, 255, 0));

            for (int i=0; i<roles.Length; i++)
                sb.AppendLine($"{EmojiStruct.EM[i]} {roles[i].Mention}");

            embed.Description = sb.ToString();
            var message = await ReplyAsync(null, false, embed.Build());
            mess.Add(message);

            for (int i = 0; i < roles.Length; i++)
                await message.AddReactionAsync(EmojiStruct.EM[i]);

            Program.client.ReactionAdded += Client_ReactionAdded;
            Program.client.ReactionRemoved += Client_ReactionRemoved;
        }

        private async Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (mess.Any(x => x.Id == arg1.Id))
            {
                ulong id = 0;
                id = avRoles[EmojiStruct.EM.ToList().IndexOf(EmojiStruct.EM.ToList().FirstOrDefault(a => a.Equals(arg3.Emote)))].Id;
                var role = Context.Guild.Roles.FirstOrDefault(x => x.Id == id);
                if (arg3.UserId != 744662345573204040) await (Context.Guild.Users.ToList().FirstOrDefault(x => x.Id == arg3.UserId) as IGuildUser).RemoveRoleAsync(role);
            }
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (mess.Any(x => x.Id == arg1.Id))
            {
                ulong id = 0;
                id = avRoles[EmojiStruct.EM.ToList().IndexOf(EmojiStruct.EM.ToList().FirstOrDefault(a => a.Equals(arg3.Emote)))].Id;
                var role = Context.Guild.Roles.FirstOrDefault(x => x.Id == id);
                if (arg3.UserId != 744662345573204040) await (Context.Guild.Users.ToList().FirstOrDefault(x => x.Id == arg3.UserId) as IGuildUser).AddRoleAsync(role);
            }
        }

        [Command("deletechannels"), Alias("dc", "deltchans")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        public async Task DeleteChannel(params ulong[] ids)
        {
            foreach (var i in ids)
                await Context.Guild.Channels.ToList().FirstOrDefault(x => x.Id == i).DeleteAsync();
        }

        [Command("deletemessages"), Alias("deltmess", "dm")]
        [RequireBotPermission(GuildPermission.ManageGuild)]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task DeleteMessage(ITextChannel chan, uint count)
        {
            chan = chan as SocketTextChannel;
            var messages = await chan.GetMessagesAsync((int)count).FlattenAsync();
            await chan.DeleteMessagesAsync(messages);
        }

        [Command("ownnick"), Alias("onick")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireUserPermission(GuildPermission.ChangeNickname)]
        public async Task ManageOwnNick(params string[] newNick)
        {
            var user = Context.User as IGuildUser;
            await user.ModifyAsync(x => x.Nickname = $"{string.Join(" ", newNick)}");
            await Context.Channel.SendMessageAsync($"{user.Mention}, your nickname changed successfully!");
        }

        [Command("nick")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireUserPermission(GuildPermission.ManageNicknames)]
        public async Task ManageNick(IGuildUser user, params string[] newNick)
        {
            await user.ModifyAsync(x => x.Nickname = $"{string.Join(" ", newNick)}");
            await Context.Channel.SendMessageAsync($"{user.Mention}, your nickname was changed!");
        }
    }
}
