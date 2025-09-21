namespace HeroicMud.GameLogic.Data.NPCs
{
	public class DarrelNPC
	{
		public static DialogueNode Dialogue;

		static DarrelNPC()
		{
			Dialogue = new DialogueNode("Darrel", 
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
