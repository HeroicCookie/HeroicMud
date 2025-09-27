namespace HeroicMud.Game.Content.NPCs
{
	public sealed class DarrelNPC : NPC
	{
		private static readonly DialogueNode _dialogue;
		public override DialogueNode DialogueNode { get; } = _dialogue;

		public DarrelNPC() : base("darrel", "Darrel") { }

		public override string GetDescription(Player player)
		{
			return "Darrel is the tavern keeper of the Windstop Inn. He looks friendly enough.";
		}

		static DarrelNPC()
		{
			_dialogue = new DialogueNode("Darrel",
				[
					"Hello, traveler!",
					"Welcome to my tavern!",
					"Let me know if there is anything I can get you."
				])
				.WithOption(new("How are you?", ["I'm fine!"]))
				.WithOption(new("I know your secret!", ["OH GOD NO."], _ => true))
				.WithOption(
					new DialogueNode("What are you?")
						.WithCondition(_ => false)
						.WithResponse(p => [$"I'm a [p.Name] hater."])
				)
				.WithOption(new("This is another way to add nodes!", ["What??"]), out DialogueNode anotherWay);

			anotherWay.WithOption(
				new DialogueNode("Yeah, cool right?", ["Please get out of here."])
					.WithOption(anotherWay)
			);
		}
	}
}
