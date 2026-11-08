using System.Collections.Generic;

namespace CorpPresence;

internal static class TranslationFormat
{
	internal static bool Compatible(string source, string translated)
	{
		if (!TryIndices(translated, out var indices))
		{
			return false;
		}
		if (!TryIndices(source, out var indices2))
		{
			return indices.Count == 0;
		}
		return indices2.SetEquals(indices);
	}

	private static bool TryIndices(string text, out HashSet<int> indices)
	{
		indices = new HashSet<int>();
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] == '}')
			{
				if (i + 1 >= text.Length || text[i + 1] != '}')
				{
					return false;
				}
				i++;
			}
			else
			{
				if (text[i] != '{')
				{
					continue;
				}
				if (i + 1 < text.Length && text[i + 1] == '{')
				{
					i++;
					continue;
				}
				i++;
				if (!Number(text, ref i, out var value))
				{
					return false;
				}
				indices.Add(value);
				Spaces(text, ref i);
				if (i < text.Length && text[i] == ',')
				{
					i++;
					Spaces(text, ref i);
					if (i < text.Length && text[i] == '-')
					{
						i++;
					}
					if (!Number(text, ref i, out var _))
					{
						return false;
					}
					Spaces(text, ref i);
				}
				if (i < text.Length && text[i] == ':')
				{
					for (i++; i < text.Length && text[i] != '}'; i++)
					{
						if (text[i] == '{')
						{
							return false;
						}
					}
				}
				if (i >= text.Length || text[i] != '}')
				{
					return false;
				}
			}
		}
		return true;
	}

	private static bool Number(string text, ref int index, out int value)
	{
		value = 0;
		int num = index;
		while (index < text.Length && text[index] >= '0' && text[index] <= '9')
		{
			value = value * 10 + text[index++] - 48;
			if (value > 1024)
			{
				return false;
			}
		}
		return index != num;
	}

	private static void Spaces(string text, ref int index)
	{
		while (index < text.Length && text[index] == ' ')
		{
			index++;
		}
	}
}
