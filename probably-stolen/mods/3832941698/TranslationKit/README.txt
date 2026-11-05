AI Emporium localization — v1.3.14
=================================

FOR TRANSLATORS

This mod uses the game's native Localization/<locale>.csv files and
ModHelper.GetLocalized("ai.emporium", key). No DLL rebuilding is required.

1. For Korean, copy TranslationKit/ko.csv to Localization/ko.csv.
   For English, copy TranslationKit/en.csv to Localization/en.csv, or rename it
   to the game's language code (for example ru.csv, ja.csv, de.csv, fr.csv,
   es.csv or pl.csv). The template contains CHINESE SOURCE TEXT, not a finished
   translation. Other language filenames follow the selected locale.
2. Translate the second column only. Keep every key in the first column intact.
   Partial translations are supported: remove unfinished rows or leave values
   empty to fall back. Do not replace an untranslated value with its key.
3. Save as UTF-8 CSV, select the matching language in the game and fully restart
   the game. Language files are loaded at startup; there is no file watcher.
4. Test a new visit, telephone call, settings dropdown, business-card tooltip,
   and all three pages of the trade guide. Keep guide text short to fit its cells.

CSV FORMAT

- One physical line per entry: key,value. No column-header row.
- Lines beginning with # are comments.
- If a value contains commas, quote the whole value with double quotes.
- Inside a quoted value, escape a double quote as two double quotes.
- Write a line break as literal \n, not a physical newline inside a CSV cell.
- Keep placeholders such as {0}, and any rich-text tags, intact.
- Keys and their meaning are stable across wording updates. Do not translate
  item IDs, phone numbers, sound IDs, saved configuration values or sprite paths.

Examples (illustrative translations):
dialogue.glm.judge.crim_accept,...Tch. I'll listen to you this time.
settings.enabled.label,Allow visits
phone.appointment.next_day,"All right, I'll stop by tomorrow. See you on day {0}."

LOOKUP AND COMPATIBILITY

Fallback order: exact locale -> parent language (en-US -> en) -> English ->
Chinese CSV -> built-in Chinese. Empty/whitespace values count as missing.
Invalid format placeholders fall back to the Chinese source sentence.

All text lookup is scoped to ai.emporium; other mods and vanilla text keep their
normal behavior. Config dropdowns translate display labels only. Their ordering,
stored values and item/category IDs remain unchanged, preserving existing saves.

Covered: character names, greetings, repeat lines, flavor lines, bargaining,
appraisal, gun reactions, GLM's report/gift event, phone dialogue/options,
appointments, settings labels/tooltips/choices/sections/slider readouts, cards,
folio, trade guide, cassette titles/artists, and mod name/description.

Card artwork keeps fixed English branding and phone numbers. These image pixels
are not CSV text. The card's item name and description ARE localizable.
The game's own cassette quality suffixes and standard UI labels use its locale.
Book subtitle fields in the source JSON are legacy, unused metadata.

Each key belongs to ai.emporium. A separate translation mod's own Localization
folder does not automatically override this mod: install files in this mod's
Localization folder or submit them for inclusion in the main release.
Steam updates may replace locally edited files; keep a copy of your translation.

Key groups:
character.<id>.*             name and base dialogue
dialogue.<id>.first/card     first meeting and card handoff
dialogue.<id>.repeat.<n>      repeat meetings (1-based index)
dialogue.<id>.egg.<n>         extra lines (1-based index)
dialogue.<id>.judge.*         bargaining/appraisal/reactions
dialogue.<id>.phone.<n>       telephone dialogue
dialogue.glm.report.*        first-report intervention and gift
phone.*                     phone choices, appointments and stock descriptions
settings.*                  settings interface, choices and numeric readouts
book.card_<id>.*             business-card item text
book.trade_guide.*           trade-guide cells and labels
item.glm_folio.*             folio item name/description
tape.<track>.*               cassette title and artist
mod.* / debug.* / ui.*       shared UI strings

给中文维护者

Localization/zh.csv 是完整中文对照；TranslationKit/ko.csv 是韩语待翻译模板，en.csv 是英文模板。
译者只需改右侧文字，翻译完成后放到 Localization/对应语言代码.csv。
缺失译文自动回退，玩家不需要开新档。切换语言或改 CSV 后完整重启游戏。
不要重命名已有键、不要更改占位符和物品 ID。纸面交易表文字请保持简短。
未来新增/修改文案时，同步更新 Source/AiLocDefaults.cs、中文 CSV 和模板；
台词原稿仍保留在相应角色文件中，方便维护人设与中文内容。

한국어 번역자를 위한 안내

ko.csv는 한국어 번역용 템플릿입니다. 현재 값은 중국어 원문이며,
완성된 한국어 번역이 아닙니다. 첫 번째 열의 키는 그대로 두고
두 번째 열의 텍스트만 번역해 주세요.

완성된 파일은 모드 폴더의 Localization/ko.csv에 넣으세요.
게임 언어를 한국어로 설정한 뒤 게임을 완전히 종료하고 다시 실행하세요.
DLL을 다시 빌드할 필요는 없습니다. ko-KR 로캘도 ko.csv로 폴백됩니다.

UTF-8로 저장하고 {0} 같은 자리 표시자와 서식 태그를 유지해 주세요.
줄바꿈은 실제 줄바꿈 대신 \n으로 입력하세요. 쉼표가 포함된 값은
큰따옴표로 감싸고, 값 안의 큰따옴표는 두 번 써 주세요.
번역하지 않은 항목은 삭제하거나 값을 비워 두면 원문으로 표시됩니다.
거래 안내표의 칸은 좁으므로 문구는 가능한 한 짧게 작성해 주세요.
