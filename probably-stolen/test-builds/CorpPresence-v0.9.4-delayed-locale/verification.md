# CorpPresence v0.9.4 delayed-initialization TEST build

Status: actual DLL built; static IL comparison succeeded; NOT game-runtime tested.

- Input SHA256: 7e34d05ad2269a0a0403fd5dcd3cea78f8b629af47f956a8366a807eea4ff668
- Output SHA256: 29092ff53eb944be2ad6f4b3b9938431fdb27cec338a5056076dd0bd8ad00aac
- Original bytes: 6961664
- Test DLL bytes: 6959104
- Preserved assembly identity: CorpPresence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
- Exactly two ResourceBootstrap.Tick calls replaced by stack-equivalent pop:

  - System.Void CorpPresence.Main::OnEnable() instruction #9: callvirt method:System.Void CorpPresence.Runtime.ResourceBootstrap::Tick() => pop
  - System.Void CorpPresence.Runtime.ResourceBootstrap::<Configure>b__3_0(ModItemDirectory) instruction #1: call method:System.Void CorpPresence.Runtime.ResourceBootstrap::Tick() => pop

The Main.OnEnable immediate Tick and Configure item-directory delegate Tick are deferred.
ResourceBootstrap.Update Tick remains intact, so polling begins in a later Unity frame.
The asset initialization failure policy in GameResources.Poll has NOT been changed.
No other method instruction is modified; verification tested all 2191 method bodies.
Original user-uploaded CorpPresence.dll remains intact.

Requires actual game validation before production use. Test the originally reported conflict pairs.
