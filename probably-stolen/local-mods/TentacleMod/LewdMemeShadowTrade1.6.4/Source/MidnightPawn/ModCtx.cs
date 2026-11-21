using Object = UnityEngine.Object;
using UnityEngine;

namespace MidnightPawn;

public static class ModCtx
{
	private static ModLog log;

	public static void Attach(ModLog modLog)
	{
		log = modLog;
	}

	public static void Log(object msg)
	{
		if (log != null)
		{
			log.Log(msg);
		}
		else
		{
			Debug.Log((object)("[淫色深空：暗流交易] " + msg));
		}
	}

	public static void LogWarning(object msg)
	{
		if (log != null)
		{
			log.Warning(msg);
		}
		else
		{
			Debug.LogWarning((object)("[淫色深空：暗流交易] " + msg));
		}
	}

	public static void LogError(object msg)
	{
		if (log != null)
		{
			log.Error(msg);
		}
		else
		{
			Debug.LogError((object)("[淫色深空：暗流交易] " + msg));
		}
	}
}

