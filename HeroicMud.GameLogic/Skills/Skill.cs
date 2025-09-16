using System.Collections.Immutable;

namespace HeroicMud.GameLogic.Skills;

public abstract class Skill
{
	public abstract string Name { get; }
	public int Experience { get; private set; } = 0;
	private static readonly ImmutableDictionary<int, int> experienceTable = new Dictionary<int, int>()
	{
		{ 2, 100 },
		{ 3, 210 },
		{ 4, 330 },
		{ 5, 463 },
		{ 6, 609 },
		{ 7, 768 },
		{ 8, 944 },
		{ 9, 1_137 },
		{ 10, 1_349 },
		{ 11, 1_581 },
		{ 12, 1_837 },
		{ 13, 2_117 },
		{ 14, 2_426 },
		{ 15, 2_764 },
		{ 16, 3_136 },
		{ 17, 3_545 },
		{ 18, 3_993 },
		{ 19, 4_486 },
		{ 20, 5_027 },
		{ 21, 5_622 },
		{ 22, 6_275 },
		{ 23, 6_992 },
		{ 24, 7_779 },
		{ 25, 8_645 },
		{ 26, 9_595 },
		{ 27, 10_639 },
		{ 28, 11_785 },
		{ 29, 13_044 },
		{ 30, 14_427 },
		{ 31, 15_946 },
		{ 32, 17_614 },
		{ 33, 19_447 },
		{ 34, 21_460 },
		{ 35, 23_670 },
		{ 36, 26_099 },
		{ 37, 28_766 },
		{ 38, 31_695 },
		{ 39, 34_912 },
		{ 40, 38_446 },
		{ 41, 42_327 },
		{ 42, 46_591 },
		{ 43, 51_273 },
		{ 44, 56_416 },
		{ 45, 62_065 },
		{ 46, 68_270 },
		{ 47, 75_084 },
		{ 48, 82_569 },
		{ 49, 90_791 },
		{ 50, 99_820 },
		{ 51, 109_738 },
		{ 52, 120_632 },
		{ 53, 132_597 },
		{ 54, 145_739 },
		{ 55, 160_173 },
		{ 56, 176_027 },
		{ 57, 193_440 },
		{ 58, 212_567 },
		{ 59, 233_574 },
		{ 60, 256_647 },
		{ 61, 281_990 },
		{ 62, 309_826 },
		{ 63, 340_399 },
		{ 64, 373_980 },
		{ 65, 410_863 },
		{ 66, 451_374 },
		{ 67, 495_870 },
		{ 68, 544_742 },
		{ 69, 598_421 },
		{ 70, 657_379 },
		{ 71, 722_137 },
		{ 72, 793_264 },
		{ 73, 871_387 },
		{ 74, 957_193 },
		{ 75, 1_051_439 },
		{ 76, 1_154_955 },
		{ 77, 1_268_653 },
		{ 78, 1_393_533 },
		{ 79, 1_530_696 },
		{ 80, 1_681_350 },
		{ 81, 1_846_822 },
		{ 82, 2_028_569 },
		{ 83, 2_228_192 },
		{ 84, 2_447_449 },
		{ 85, 2_688_272 },
		{ 86, 2_952_781 },
		{ 87, 3_243_306 },
		{ 88, 3_562_406 },
		{ 89, 3_912_892 },
		{ 90, 4_297_851 },
		{ 91, 4_720_673 },
		{ 92, 5_185_082 },
		{ 93, 5_695_169 },
		{ 94, 6_255_426 },
		{ 95, 6_870_788 },
		{ 96, 7_546_675 },
		{ 97, 8_289_039 },
		{ 98, 9_104_421 },
		{ 99, 10_000_000 }
	}.ToImmutableDictionary();
	public int Level => GetCurrentLevel();
	public event Action<int>? LeveledUp;
	public int ExperienceToNextLevel => GetExperienceToNextLevel();

	public void AddExperience(int amount)
	{
		if (amount < 0)
		{
			throw new ArgumentException("Experience amount cannot be negative.");
		}
		int currentLevel = GetCurrentLevel();
		Experience += amount;
		int newLevel = GetCurrentLevel();

		if (newLevel > currentLevel)
		{
			LeveledUp?.Invoke(newLevel);
		}
	}

	private int GetCurrentLevel()
	{
		int level = 1;
		foreach (var kvp in experienceTable)
		{
			if (Experience >= kvp.Value)
			{
				level = kvp.Key;
			}
			else
			{
				break;
			}
		}
		return level;
	}

	private int GetExperienceToNextLevel()
	{
		int currentLevel = GetCurrentLevel();
		if (currentLevel >= 99)
		{
			return 0;
		}
		return experienceTable[currentLevel + 1] - Experience;
	}
	
	public override string ToString()
	{
		return $"{Name} - Level {Level} ({Experience} XP, {ExperienceToNextLevel} XP to next level)";
	}
}