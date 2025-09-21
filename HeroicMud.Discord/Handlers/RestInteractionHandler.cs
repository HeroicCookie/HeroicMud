using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using HeroicMud.GameLogic;
using HeroicMud.GameLogic.Data.NPCs;
using HeroicMud.GameLogic.Data.Rooms;

namespace HeroicMud.Discord.Handlers
{
	public class RestInteractionHandler(MudGame game, RoomManager roomManager) : RestInteractionModuleBase<RestInteractionContext>
	{
		

	}
}
