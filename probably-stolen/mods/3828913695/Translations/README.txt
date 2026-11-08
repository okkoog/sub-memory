公司增强 1.0 稳定版 — 外部翻译包

默认不需要安装任何翻译包，模组继续使用内置文本。
将翻译JSON放进与CorpPresence.dll同级的Translations文件夹。
每个JSON使用schemaVersion=1、locale语言代码及translations键值字典。

TranslationExamples/ru.json、fr.json为现有俄语、法语参考包。
TranslationExamples/es.template.json包含完整1621个键和英文参考文本，可复制后修改locale及右侧译文。
未完成条目可删除或留空，继续沿用内置及原文回退。不要修改左侧稳定键。
保留{0}、{1}等参数编号；换行写\n，可调整参数顺序但不要丢失编号。

需要选择游戏菜单没有的语言时，将TranslationExamples/language.json.example复制为Translations/language.json，修改其中的locale。
设置locale为auto或删除language.json即可跟随游戏语言。此设置只影响本模组文本。

支持大小写及地区代码，例如pt-BR继承pt。外部语言包缺项时使用父语言、内置翻译和原文回退。
修改文件后重新启用模组或重启游戏。