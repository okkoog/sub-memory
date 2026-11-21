using Object = UnityEngine.Object;
namespace MidnightPawn;

public static class ModConst
{
	public const string MOD_ID = "lewd_meme_remake";

	public const string MOD_NAME = "淫色深空：暗流交易";

	public const string ITEM_TENTACLE_BODY = "tentacle_body";

	public const string ITEM_MAIDEN = "maiden_lower";

	public const string ITEM_MILK = "milk_wondrous";

	public const string ITEM_SEMEN = "tentacle_semen";

	public const string ITEM_OVULATION = "ovulation_shot";

	public const string TAG_MAIDEN_FEMALE = "MAIDEN_FEMALE";

	public const string TAG_MAIDEN_LOWER = "MAIDEN_LOWER";

	public const string TAG_YOUNG = Game.MaidenAgeFilter.YoungTag;

	public const string TAG_BIG_BREAST = "MAIDEN_BIG_BREAST";

	public const string TAG_TRAIN_VALUE = "TRAIN_VALUE";

	public const string TAG_ORAL_COUNT = "ORAL_COUNT";

	public const string TAG_SEX_COUNT = "SEX_COUNT";

	public const string TAG_ANAL_COUNT = "ANAL_COUNT";

	public const string TAG_LACTATION_LV = "LACTATION_LV";

	public const string TAG_PREGNANT = "MAIDEN_PREGNANT";

	public const string TAG_PREGNANT_DAYS = "MAIDEN_PREGNANT_DAYS";

	public const string TAG_OVULATING = "MAIDEN_OVULATING";

	public const string TAG_FACTION_LOWER = "FACTION_LOWER_LEVEL";

	public const string TAG_JOB_SCAVENGER = "JOB_SCAVENGER";

	public const string TAG_NAME = "MAIDEN_NAME";

	public const string TAG_SPRITE_SRC = "MAIDEN_SPRITE_SRC";

	public const string TAG_MAIDEN_BORN = "MAIDEN_BORN";

	public const string KEY_BODY_LV = "body_lv";

	public const string KEY_BODY_EXP = "body_exp";

	public const string TAG_TENTACLE_BODY = "TENTACLE_BODY_TAG";

	public const string TAG_INPUT_ROLE = "TENTACLE_INPUT";

	public const string TAG_OUTPUT_ROLE = "TENTACLE_OUTPUT";

	public const string PORTRAIT_ATLAS = "lewd_meme_remake.portraits";

	public static string Loc(string key, params object[] args)
	{
		return ModHelper.GetLocalized("lewd_meme_remake", key, args);
	}
}

