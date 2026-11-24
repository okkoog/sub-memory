成龙的电话铃声（已就位）

当前文件：jackie_ringtone.ogg（14.4 秒，Vorbis 44.1kHz 立体声）
来源：B 站 BV1dh3d6UEmb《【本家投稿】yo phon lingin remix (ZZZ Dialyn meme)》
      原音频 14.4s AAC → 已转成 Unity 支持的 OGG（ModLoader 只认 wav / ogg / mp3）

要换铃声：把同名文件换成 jackie_ringtone.wav / .ogg / .mp3 即可，代码不用动。
  · ModLoader 启动时加载成 traits_probably_fun:jackie_ringtone
    （ModLoader.cs:1412-1449）
  · Jackie.RingtonePatch 在 PhoneUIManager.StartCalling() 之前把它塞进原版那个
    phoneCalling AudioSource，所以拨号超时/接通时原版 Stop() 会一起停掉；
  · 文件缺失时保持原版铃声，日志一行：
    [DSP] 成龙：没有找到铃声 Sounds/jackie_ringtone.wav/ogg/mp3，继续用原版铃声
