namespace HeroicMud.GameLogic;

public enum SaveResult
{
	Created,
	AlreadyExists,
	Error,
	Updated
}

public enum GameCommand
{
	Look,
	Go,
	Say
}
