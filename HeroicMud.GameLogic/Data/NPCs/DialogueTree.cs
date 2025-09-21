namespace HeroicMud.GameLogic.Data.NPCs;

public class DialogueNode(string option)
{
    public string Name { get => Option; }
    public string Option = option;

    public Func<Player, List<string>> Response = _ => [];
    public Func<Player, bool> Condition = _ => true;
    public Action<Player> Selected = _ => { };

    public List<DialogueNode> Children = [];

    public DialogueNode(string option, List<string> response) : this(option)
    {
        Response = _ => response;
    }

    public DialogueNode(string option, Func<Player, bool> condition) : this(option)
    {
        Condition = condition;
    }

    public DialogueNode(string option, List<string> response, Func<Player, bool> condition) : this(option)
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

    public DialogueNode WithResponse(Func<Player, List<string>> response)
    {
        Response = response;
        return this;
    }

    public DialogueResponse Next(Player player, string? option)
    {
        if (option is not null)
            foreach (DialogueNode node in Children)
                if (node.Option == option)
                    return node.Next(player, null);

        Selected(player);

        List<string> options = [];
        foreach (DialogueNode node in Children.Where(n => n.Condition(player) is true))
            options.Add(node.Option);

        return new(options.Count > 0 ? this : null, Response(player), options);
    }
}

public class DialogueResponse(DialogueNode? node, List<string> text, List<string> options)
{
    public DialogueNode? Node = node;
    public List<string> Text = text;
    public List<string> Options = options;
}