namespace HeroicMud.GameLogic.Data.NPCs;

public class DarrelNPC
{
    public static DialogueNode Dialogue;

    static DarrelNPC()
    {
        Dialogue = new DialogueNode("Darrel", ["Hello, travelers!"])
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

public class DialogueNode(string option)
{
    public string Name { get => Option; }
    public string Option = option;

    public Func<Player, string[]> Response = _ => [];
    public Func<Player, bool> Condition = _ => true;
    public Action<Player> Selected = _ => { };

    public List<DialogueNode> Children = [];

    public DialogueNode(string option, string[] response) : this(option)
    {
        Response = _ => response;
    }

    public DialogueNode(string option, Func<Player, bool> condition) : this(option)
    {
        Condition = condition;
    }

    public DialogueNode(string option, string[] response, Func<Player, bool> condition) : this(option)
    {
        Response = _ => response;
        Condition = condition;
    }

    public DialogueNode WithOption(DialogueNode node)
    {
        Children.Add(node);
        return this;
    }

    public DialogueNode WithOption(DialogueNode node, out DialogueNode target)
    {
        Children.Add(node);
        target = node;
        return this;
    }

    public DialogueNode WithCondition(Func<Player, bool> condition)
    {
        Condition = condition;
        return this;
    }

    public DialogueNode WithResponse(Func<Player, string[]> response)
    {
        Response = response;
        return this;
    }

    public DialogueResponse Next(Player player, string? option)
    {
        Selected(player);

        if (option is not null)
            foreach (DialogueNode node in Children)
                if (node.Option == option)
                    return node.Next(player, null);

        List<string> options = [];
        foreach (DialogueNode node in Children.Where(n => n.Condition(player) is true))
            options.Add(node.Option);

        return new(this, options);
    }
}

public struct DialogueResponse(DialogueNode node, List<string> options)
{
    public DialogueNode Node = node;
    public List<string> Options = options;
}