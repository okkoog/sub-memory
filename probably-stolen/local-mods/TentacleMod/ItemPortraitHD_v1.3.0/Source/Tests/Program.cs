using System;
using System.Linq;
using UnityEngine;
using ItemPortraitHD;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
static class Program
{
    static void TraceResult(ref Sprite __result) { }
    static void TraceScratch(ref Sprite buffer) { }
    static int checks;
    static void Check(bool condition, string name) { if(!condition) throw new Exception(name); checks++; Console.WriteLine("PASS " + name); }
    static void Main(string[] args)
    {
        if (args.Length != 2)
            throw new ArgumentException("Usage: Tests <MidnightPawn.dll> <candidate folder with ItemPortraitHD.dll and manifest.xml>");
        CacheRegression.Run(Check);
        CurrentPortraitItemsRegression.Run(Check);
        OriginalSelectionRegression.Run(args[0], Check);
        NameCompatibilityRegression.Run(args[0], Check);
        string traceId = TraceSignatures.SourceId("test:provider", "female/role:1");
        Check(TraceSignatures.TryParseSource(traceId, out string traceProvider, out string traceKey) &&
            traceProvider == "test:provider" && traceKey == "female/role:1", "traced source IDs round trip reserved characters");
        Check(!TraceSignatures.TryParseSource("trace::", out _, out _) && !TraceSignatures.TryParseSource("game:x", out _, out _),
            "invalid traced source IDs are rejected");
        var refreshItemsMethod = typeof(PortraitWindow).GetMethod("RefreshItems", BindingFlags.Instance | BindingFlags.NonPublic);
        var refreshItemsCode = PatchProcessor.GetOriginalInstructions(refreshItemsMethod, (ILGenerator)null);
        Check(refreshItemsCode.Any(i => i.operand is MethodInfo m && m.Name == "Scan" && m.DeclaringType?.Name == "CurrentPortraitItems") &&
            !refreshItemsCode.Any(i => i.operand is MethodInfo m && m.Name == "Live"),
            "manager item list uses current recursive inventory scan instead of stale live UI references");
        Check(TraceSignatures.ResultArgument(typeof(Program).GetMethod("TraceResult", BindingFlags.NonPublic | BindingFlags.Static)) == 0 &&
            TraceSignatures.ResultArgument(typeof(Program).GetMethod("TraceScratch", BindingFlags.NonPublic | BindingFlags.Static)) == -1,
            "only Harmony result arguments enable ref-result tracing");
        foreach (int size in new[]{128,256})
        {
            var input = new Color32[20*30];
            for(int y=5;y<25;y++) for(int x=5;x<15;x++) input[y*20+x]=new Color32(255,80,40,255);
            var result=Rasterizer.FitPixels(input,20,30,size);
            Check(result.Length==size*size,"canvas " + size);
            int left=size,right=-1,bottom=size,top=-1;
            for(int y=0;y<size;y++) for(int x=0;x<size;x++) if(result[y*size+x].a>.01f) { left=Math.Min(left,x); right=Math.Max(right,x); bottom=Math.Min(bottom,y); top=Math.Max(top,y); }
            Check(right-left+1==size/2 && top-bottom+1==size,"aspect preserved " + size);
            Check(left==size/4 && right==size*3/4-1 && bottom==0,"centered transparent padding " + size);
            Check(Math.Abs(result[(size/2)*size+size/2].r-1)<.00001,"color preserved " + size);
        }
        Check(Rasterizer.FitPixels(new Color32[16],4,4,128)==null,"transparent input rejected");
        var edge=new[]{new Color32(255,0,0,255), new Color32(0,255,0,0),new Color32(0,0,255,0),new Color32(255,0,0,255)};
        var pixel=Rasterizer.FitPixels(edge,2,2,1)[0];
        Check(Math.Abs(pixel.r-1)<.00001 && pixel.g==0 && pixel.b==0 && Math.Abs(pixel.a-.5)<.00001,"premultiplied alpha prevents dark fringes");
        var pattern=new Color32[512*512];
        for(int y=0;y<512;y++) for(int x=0;x<512;x++) pattern[y*512+x]=(x+y)%2==0 ? new Color32(255,255,255,255) : new Color32(0,0,0,255);
        var reduced=Rasterizer.FitPixels(pattern,512,512,128);
        Check(reduced.All(p=>Math.Abs(p.r-.5)<.00001 && Math.Abs(p.a-1)<.00001),"area reduction avoids checkerboard aliasing");
        var square=Footprint.Square();
        Check(square.width==4 && square.height==4,"physical footprint is 4x4");
        Check(Enumerable.Range(0,16).All(i=>square.GetLocal(i%4,i/4)==1),"all sixteen cells occupied including transparent borders");
        Check(!Footprint.IsSquare(new GridShapeBuilder(4,4).SetDataFill(0).Build()),"transparent-derived empty shape is rejected");
        var positioned=new GridShapeBuilder(square).SetPosition(5,7).SetOrientation(1).Build();
        Check(Footprint.IsSquare(positioned) && positioned.minX==5 && positioned.minY==7,"square preserves grid position and rotation");
        Check(PortraitEligibility.IsPortrait("male_portrait_1",false),"male mod portrait eligible");
        Check(PortraitEligibility.IsPortrait("female_portrait_1",false),"female mod portrait eligible");
        Check(PortraitEligibility.IsPortrait("neutral_portrait",false),"gender-neutral portrait eligible");
        Check(PortraitEligibility.IsPortrait("mentor",true),"registered character name eligible");
        Check(!PortraitEligibility.IsPortrait("water_bottle",false),"unrelated item icon excluded");
        Check(!PortraitEligibility.IsPortrait(null,false),"empty portrait key excluded");
        Check(PortraitRules.Normalize("cp_portrait_female_Lower1") == PortraitRules.Normalize("femaleLower1"), "replacement prefix and underscores map to registered character");
        Check(PortraitRules.Normalize("femaleLower1") != PortraitRules.Normalize("femaleLower10"), "character matching avoids prefix collisions");
        var retroRows = MappingRules.Retro(new[] { "key,portrait", "femaleLower1,portrait_1", "invalid" }).ToList();
        Check(retroRows.Count == 1 && retroRows[0].Relative == "Portraits/portrait_1.png", "retro mapping preserves game key and file");
        var refinedRows = MappingRules.Refined(new[] { "femaleBM_0\t1\tA.png", "femaleBM_0\t0\tB.png" }).ToList();
        Check(refinedRows.Count == 2 && refinedRows[0].Readable == true && refinedRows[1].Readable == false,
            "refined mapping keeps distinct readable and nonreadable slots");
        Check(MappingRules.MatchesSlot("femaleBM_0", true, "femaleBM_0", true) &&
            !MappingRules.MatchesSlot("femaleBM_0", true, "femaleBM_0", false) &&
            !MappingRules.MatchesSlot("femaleBM_0", true, "femaleBM0", true), "sprite slot matching prevents alias and readability collisions");
        var approvedRows = MappingRules.Approved("{\"version\":1,\"targets\":[{\"sourceModId\":\"\",\"spriteKey\":\"mentor\",\"file\":\"A.png\",\"status\":\"approved\"},{\"sourceModId\":\"other\",\"spriteKey\":\"mentor\",\"file\":\"B.png\",\"status\":\"approved\"},{\"spriteKey\":\"mentor\",\"file\":\"C.png\",\"status\":\"pending\"}]}").ToList();
        Check(approvedRows.Count == 1 && approvedRows[0].Relative == "Approved/A.png", "AllGirls imports approved base-game entries only");
        Check(PortraitRules.Faction("femaleUpper1") == "上层区" && PortraitRules.Faction("maleLower1") == "下层区", "upper and lower faction mapping");
        Check(PortraitRules.Faction("femaleSecCorpo1") == "治安部" && PortraitRules.Faction("maleMerc1") == "黑市" && PortraitRules.Faction("femaleRev1") == "革命军", "security mercenary and revolution mapping");
        Check(PortraitRules.Faction("mentor") == "" && PortraitRules.Faction("augBMMale1") == "" && PortraitRules.Faction("wanted1") == "", "named augmented and wanted characters excluded");
        Check(PortraitRules.FactionWeight("femaleChef1", "上层区") == .5f && PortraitRules.FactionWeight("femaleChef1", "下层区") == .5f, "shared occupation split evenly between upper and lower");
        foreach (var key in new[]{"eleanor", "eleanor_scav", "serpico", "serpico_scav", "cp_portrait_eleanor_scav", "cp_portrait_serpico_scav"})
            Check(new[]{"上层区", "下层区", "治安部", "黑市", "革命军"}.All(f => PortraitRules.FactionWeight(key, f) == 0), "special character excluded from every faction: " + key);
        foreach (var key in new[]{"maleScav1", "femaleScav1", "worker_scav", "cp_portrait_worker_scav"})
            Check(PortraitRules.FactionWeight(key, "下层区") == 1 && PortraitRules.FactionWeight(key, "上层区") == 0 && !PortraitRules.SharedWorker(key),
                "scavenger contributes full weight to lower district only: " + key);
        Check(PortraitRules.Weighted(new[]{"zero","half","full"}, k=> k=="zero" ? 0 : k=="half" ? .5f : 1, .3334f) == "full", "weighted draw skips zero and respects half weight");
        Check(PortraitRules.Weighted(new[]{"zero"}, k=>0, 0) == null, "all zero weights have no selection");
        Check(PortraitRules.EffectiveRoleWeight(9, new[]{2f, 3f}, true) == 5f, "role weight equals sum of replacement portrait weights");
        Check(PortraitRules.EffectiveRoleWeight(9, new[]{0f, 0f}, true) == 0f, "role with all replacement weights zero leaves capture pool");
        Check(PortraitRules.EffectiveRoleWeight(2, Array.Empty<float>(), false) == 2f, "role without replacement uses native role weight");
        Check(PortraitRules.IsMappedReplacementId("mod:A:portrait", true) &&
            PortraitRules.IsMappedReplacementId("game:femaleUpper1", true) == false &&
            PortraitRules.IsMappedReplacementId("local:extra.png", true) == false,
            "role replacement total excludes native and local sources");
        Check(!PortraitRules.LocalHit(0, 0) && PortraitRules.LocalHit(100, 1f), "zero and hundred percent boundaries including inclusive Unity endpoint");
        Check(PortraitRules.LocalHit(30, .29f) && !PortraitRules.LocalHit(30, .30f), "probability threshold");
        var replacements = new[] { "A", "B" };
        Check(PortraitRules.Weighted(replacements, s => s == "A" ? 2 : 1, .66f) == "A" &&
            PortraitRules.Weighted(replacements, s => s == "A" ? 2 : 1, .67f) == "B", "mod replacement draw follows individual 2 to 1 weights");
        int rounds = 0, fallbacks = 0;
        string retry = PortraitRules.RetryPortrait(() => "role" + ++rounds,
            key => (string)null, key => { fallbacks++; return "native:" + key; });
        Check(rounds == 4 && fallbacks == 1 && retry == "native:role4", "four character draws then native of fourth character");
        rounds = 0; fallbacks = 0;
        retry = PortraitRules.RetryPortrait(() => "role" + ++rounds,
            key => rounds == 3 ? "mod:" + key : null, key => { fallbacks++; return "native:" + key; });
        Check(rounds == 3 && fallbacks == 0 && retry == "mod:role3", "successful third round stops without native fallback");
        rounds = 0;
        retry = PortraitRules.RetryPortrait(() => "role" + ++rounds,
            key => "mod:" + key, key => throw new Exception("Unexpected fallback"));
        Check(rounds == 1 && retry == "mod:role1", "first success does not retry");
        rounds = 0;
        retry = PortraitRules.RetryPortrait(() => "role" + ++rounds,
            key => rounds == 4 ? "mod:" + key : null, key => throw new Exception("Fourth success must not fall back"));
        Check(rounds == 4 && retry == "mod:role4", "fourth round still tries weighted mod selection first");
        var randomType = Assembly.Load("ItemPortraitHD").GetType("ItemPortraitHD.RandomPortraits");
        var factionMethod = randomType.GetMethod("PickFactionCore", BindingFlags.Static | BindingFlags.NonPublic);
        var factionIL = PatchProcessor.GetOriginalInstructions(factionMethod, (ILGenerator)null);
        int retryIndex = factionIL.FindIndex(i => i.operand is MethodInfo m && m.Name == "RetryPortrait");
        int factionDrawIndex = factionIL.FindLastIndex(i => i.operand is MethodInfo m && m.Name.Contains("DrawFaction"));
        Check(factionDrawIndex >= 0 && factionDrawIndex < retryIndex, "faction is chosen before character retry loop");
        var drawDelegates = randomType.GetNestedTypes(BindingFlags.NonPublic).SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            .Where(m => m.Name.Contains("<PickFactionCore>b__") && m.ReturnType == typeof(string) && m.GetParameters().Length == 0).ToList();
        Check(drawDelegates.Any(m => { var code = PatchProcessor.GetOriginalInstructions(m, (ILGenerator)null);
            return code.Any(i => i.operand is MethodInfo call && call.Name == "Draw") &&
                !code.Any(i => i.operand is MethodInfo call && call.Name.Contains("DrawFaction")); }), "retry character delegate does not redraw faction");
        Check(PortraitEligibility.CanAdjustWeight(false, true, false, true), "registered replacement allows weight editing");
        Check(!PortraitEligibility.CanAdjustWeight(false, false, false, false), "unmapped or out-of-pool image remains manual only");
        Check(PortraitEligibility.CanAdjustWeight(false, false, true, false), "classified custom folder allows weight editing");
        Check(PortraitEligibility.CanAdjustWeight(true, false, false, false) &&
            !PortraitEligibility.CanAdjustWeight(true, false, false, true), "base role weight is editable only without a mapped replacement");
        Check(PortraitEligibility.Tier(true, true) == 0 && PortraitEligibility.Tier(false, true) == 1 && PortraitEligibility.Tier(false, false) == 2,
            "source ranking places faction weights, manual portraits, then other images");
        var compat=Assembly.Load("ItemPortraitHD").GetType("ItemPortraitHD.OriginalCompat");
        var shapePrefix = compat.Assembly.GetType("ItemPortraitHD.ShapeStatePatch").GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic);
        Check(shapePrefix.GetCustomAttributes(typeof(HarmonyPrefix), false).Length == 1 &&
            PatchProcessor.GetOriginalInstructions(shapePrefix, (ILGenerator)null).Any(i => i.operand is MethodInfo m && m.Name == "Ensure"),
            "shape validation still invokes the full-square footprint prefix");
        var setShapeIL = PatchProcessor.GetOriginalInstructions(typeof(GameItem).GetMethod("SetShape"), (ILGenerator)null);
        var setSpriteShapeIL = PatchProcessor.GetOriginalInstructions(typeof(GameItem).GetMethod("SetSpriteAndShapeFromMod"), (ILGenerator)null);
        Check(setShapeIL.Any(i => i.operand is MethodInfo m && m.Name == "ValidateShapeState") &&
            setSpriteShapeIL.Any(i => i.operand is MethodInfo m && m.Name == "SetShape"),
            "native sprite-to-shape assignment reaches patched shape validation");
        var hitIL = PatchProcessor.GetOriginalInstructions(compat.Assembly.GetType("ItemPortraitHD.DisplaySizePatch")
            .GetMethod("FitItem", BindingFlags.Static | BindingFlags.NonPublic), (ILGenerator)null);
        int alphaIndex = hitIL.FindIndex(i => i.operand is MethodInfo m && m.Name == "set_alphaHitTestMinimumThreshold");
        int raycastIndex = hitIL.FindIndex(i => i.operand is MethodInfo m && m.Name == "set_raycastTarget");
        Check(alphaIndex > 0 && hitIL[alphaIndex - 1].opcode == OpCodes.Ldc_R4 && (float)hitIL[alphaIndex - 1].operand == 0 &&
            raycastIndex > 0 && hitIL[raycastIndex - 1].opcode == OpCodes.Ldc_I4_1,
            "item display explicitly enables raycasts over transparent canvas pixels");
        var windowUpdate = PatchProcessor.GetOriginalInstructions(typeof(PortraitWindow).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic), (ILGenerator)null);
        var enableCode = PatchProcessor.GetOriginalInstructions(typeof(ModEntry).GetMethod("OnEnable"), (ILGenerator)null);
        var disableCode = PatchProcessor.GetOriginalInstructions(typeof(ModEntry).GetMethod("OnDisable"), (ILGenerator)null);
        Check(enableCode.Any(i => i.operand is MethodInfo m && m.Name == "AddGameOptionKeyBind") &&
            disableCode.Any(i => i.operand is MethodInfo m && m.Name == "RemoveGameOptions") &&
            windowUpdate.Any(i => i.operand is MethodInfo m && m.Name == "get_SuppressHotkeys"),
            "native key binding handles capture while manager respects game input suppression");
        var finalType = compat.Assembly.GetType("ItemPortraitHD.FinalPortraits");
        var scanCode = PatchProcessor.GetOriginalInstructions(finalType.GetMethod("Tick", BindingFlags.Static | BindingFlags.NonPublic), (ILGenerator)null);
        Check(scanCode.Any(i => i.operand is MethodInfo m && m.Name == "GetSprite") &&
            scanCode.Any(i => i.operand is MethodInfo m && m.Name == "ObserveFinal") &&
            compat.Assembly.GetType("ItemPortraitHD.ReplacementTrace").GetMethod("Tick", BindingFlags.Static | BindingFlags.NonPublic) == null,
            "one scan queue updates final sprites and replacement observations");
        Check(windowUpdate.Any(i => i.operand is MethodInfo m && m.Name == "OpenManager") &&
            !windowUpdate.Any(i => i.operand is MethodInfo m && m.Name == "Reload"),
            "F7 opens manager without forcing image reload");
        var openCode = PatchProcessor.GetOriginalInstructions(typeof(PortraitWindow).GetMethod("OpenManager", BindingFlags.Instance | BindingFlags.NonPublic), (ILGenerator)null);
        Check(!openCode.Any(i => i.operand is MethodInfo m && (m.Name == "Clear" || m.Name == "Refresh")),
            "ordinary manager opening preserves portrait cache and avoids global item refresh");
        var fitImage = compat.Assembly.GetType("ItemPortraitHD.DisplaySizePatch").GetMethod("Fit", BindingFlags.Static | BindingFlags.NonPublic);
        var fitCode = PatchProcessor.GetOriginalInstructions(fitImage, (ILGenerator)null);
        int typeIndex = fitCode.FindIndex(i => i.operand is MethodInfo m && m.Name == "set_type");
        Check(typeIndex > 0 && fitCode[typeIndex - 1].opcode == OpCodes.Ldc_I4_0,
            "HD item image explicitly uses Simple mode instead of inherited tiling");
        var createdMethod = compat.GetMethod("Created", BindingFlags.Static | BindingFlags.NonPublic);
        var createdInstructions = PatchProcessor.GetOriginalInstructions(createdMethod, (ILGenerator)null);
        Check(createdInstructions.Any(i => i.operand is MethodInfo m && m.Name == "FitItem"),
            "newly captured portraits receive the same immediate display-size fit as manual replacements");
        var renderMethod=compat.Assembly.GetType("ItemPortraitHD.Portraits").GetMethod("Render",BindingFlags.Static|BindingFlags.NonPublic);
        var portraitsType = renderMethod.DeclaringType;
        bool CallsPortraitGet(MethodInfo method) => PatchProcessor.GetOriginalInstructions(method, (ILGenerator)null)
            .Any(i => i.operand is MethodInfo m && m.DeclaringType == portraitsType && m.Name == "Get");
        Check(CallsPortraitGet(compat.GetMethod("InjectPrefix", BindingFlags.Static | BindingFlags.NonPublic)),
            "capture sprite injection uses shared portrait conversion and cache");
        var installCode = PatchProcessor.GetOriginalInstructions(compat.GetMethod("Install", BindingFlags.Static | BindingFlags.NonPublic), (ILGenerator)null);
        Check(installCode.Any(i => i.opcode == OpCodes.Ldstr && (string)i.operand == "InjectPortraitFromName"),
            "guest source observer is installed without overriding selection");
        var publishCode = PatchProcessor.GetOriginalInstructions(compat.GetMethod("Publish", BindingFlags.Static | BindingFlags.NonPublic), (ILGenerator)null);
        var references = PatchProcessor.GetOriginalInstructions(portraitsType.GetMethod("Referenced", BindingFlags.Static | BindingFlags.NonPublic), (ILGenerator)null);
        Check(publishCode.Any(i => i.operand is MethodInfo m && m.Name == "set_Item") &&
            references.Any(i => i.operand is MethodInfo m && m.Name == "Registries"),
            "published runtime sprites are protected against preview cache eviction");
        Check(CallsPortraitGet(portraitsType.GetMethod("For", BindingFlags.Static | BindingFlags.NonPublic)),
            "manual item source resolution uses shared portrait conversion and cache");
        Check(CallsPortraitGet(typeof(PortraitWindow).GetMethod("Draw", BindingFlags.Instance | BindingFlags.NonPublic)),
            "F7 previews use shared portrait conversion and cache");
        var getCode = PatchProcessor.GetOriginalInstructions(portraitsType.GetMethod("Get", BindingFlags.Static | BindingFlags.NonPublic), (ILGenerator)null);
        Check(!getCode.Any(i => i.operand is MethodInfo m && m.DeclaringType == typeof(Sprite) && m.Name == "get_pixelsPerUnit"),
            "normalized HD output does not inherit arbitrary provider pixels-per-unit");
        var nativeSizePatch = compat.Assembly.GetType("ItemPortraitHD.PortraitNativeSizePatch");
        var nativeSizeCode = PatchProcessor.GetOriginalInstructions(nativeSizePatch.GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic), (ILGenerator)null);
        Check(nativeSizeCode.Any(i => i.operand is MethodInfo m && m.DeclaringType.Name == "DisplaySizePatch" && m.Name == "Fit"),
            "late SetNativeSize calls reapply the same portrait canvas clamp");
        Check(fitCode.Count(i => i.operand is MethodInfo m && m.Name == "SetSizeWithCurrentAnchors") == 2 &&
            fitCode.Any(i => i.operand is MethodInfo m && m.Name == "set_localScale"),
            "portrait clamp fixes both actual rectangle axes and image-local scaling");
        var fitItemCode = PatchProcessor.GetOriginalInstructions(fitImage.DeclaringType.GetMethod("FitItem", BindingFlags.Static | BindingFlags.NonPublic), (ILGenerator)null);
        Check(fitItemCode.Count(i => i.operand is MethodInfo m && m.Name == "Normalize") == 2,
            "normal and drag images both normalize unresolved provider sprites before fitting");
        Check(getCode.Count(i => i.operand is MethodInfo m && m == renderMethod) == 1,
            "shared portrait conversion has one crop and resample call");
        var renderInstructions=PatchProcessor.GetOriginalInstructions(renderMethod,(ILGenerator)null);
        int cpuRead = renderInstructions.FindIndex(i => i.operand is MethodInfo m && m.Name == "ReadablePixels");
        int gpuRead = renderInstructions.FindIndex(i => i.operand is MethodInfo m && m.Name == "ReadPixels");
        Check(cpuRead >= 0 && gpuRead > cpuRead && renderInstructions.Skip(cpuRead + 1).Take(gpuRead - cpuRead - 1)
            .Any(i => i.opcode == OpCodes.Brtrue || i.opcode == OpCodes.Brtrue_S),
            "readable input bypasses synchronous GPU readback");
        var applyIndex=renderInstructions.FindIndex(i=>i.operand is MethodInfo m && m.DeclaringType==typeof(Texture2D) && m.Name=="Apply" && m.GetParameters().Length==2);
        Check(applyIndex>=2 && renderInstructions[applyIndex-1].opcode==OpCodes.Ldc_I4_0 && renderInstructions[applyIndex-2].opcode==OpCodes.Ldc_I4_0,
            "published portrait texture keeps CPU pixels: Apply(false,false)");
        var consumer=typeof(GameItem).GetMethod("SetSpriteAndShapeFromMod",BindingFlags.Public|BindingFlags.Instance);
        var consumerInstructions=PatchProcessor.GetOriginalInstructions(consumer,(ILGenerator)null);
        Check(consumerInstructions.Any(i=>i.operand is MethodInfo m && m.DeclaringType==typeof(Texture2D) && m.Name=="GetPixel"),
            "game sprite and shape consumer requires readable pixels during creation and restore");
        var transpile=compat.GetMethod("ToolTranspiler",BindingFlags.Static|BindingFlags.NonPublic);
        var call=new CodeInstruction(OpCodes.Call,typeof(LocGenderHelper).GetMethod("GetCurrentClientGender"));
        var il=new DynamicMethod("LabelTest",typeof(void),Type.EmptyTypes).GetILGenerator();
        call.labels.Add(il.DefineLabel());
        call.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock));
        var instructions=new List<CodeInstruction>{new CodeInstruction(OpCodes.Nop),call,new CodeInstruction(OpCodes.Ldc_I4_2)};
        var rewritten=((IEnumerable<CodeInstruction>)transpile.Invoke(null,new object[]{instructions})).ToList();
        Check(rewritten.Count==3 && ((MethodInfo)rewritten[1].operand).Name=="ToolGender","only original tool gender lookup replaced");
        Check(rewritten[1].labels.Count==1 && rewritten[1].blocks.Count==1,"transpiler preserves branch labels and exception blocks");
        Check(rewritten[0].opcode==OpCodes.Nop && rewritten[2].opcode==OpCodes.Ldc_I4_2,"other tool instructions unchanged");
        bool rejected=false;
        try { transpile.Invoke(null,new object[]{new List<CodeInstruction>{new CodeInstruction(OpCodes.Ret)}}); }
        catch(TargetInvocationException ex) when(ex.InnerException is InvalidOperationException) { rejected=true; }
        Check(rejected,"incompatible tool IL rejected");
        if (args.Length > 0)
        {
            var originalAssembly = Assembly.LoadFrom(args[0]);
            var injector = originalAssembly.GetType("MidnightPawn.Game.SpriteInjector");
            var guestInjection = injector.GetMethod("InjectPortraitFromName", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            Check(guestInjection?.ReturnType == typeof(string) && guestInjection.GetParameters().Select(p => p.ParameterType)
                .SequenceEqual(new[] { typeof(string), typeof(int), typeof(int) }), "dependency guest injection hook signature matches");
            var guestType = compat.Assembly.GetType("ItemPortraitHD.GuestPortraits");
            foreach (var spec in new[]{new[]{"GenderRatio", "OnGenerated", "1"},new[]{"SpriteInjector", "ExportInjected", "2"},new[]{"TentacleEffects", "MarkAffected", "1"},new[]{"TentacleEffects", "ClearAffected", "0"},new[]{"Toast", "Show", "2"}})
            {
                var method = originalAssembly.GetType("MidnightPawn.Game."+spec[0])?.GetMethod(spec[1], BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                Check(method != null && method.GetParameters().Length == int.Parse(spec[2]), "dependency patch entry signature " + spec[0]+"."+spec[1]);
            }
            var patchedTool = ((IEnumerable<CodeInstruction>)compat.GetMethod("ToolTranspiler",BindingFlags.Static | BindingFlags.NonPublic).Invoke(null,new object[]{PatchProcessor.GetOriginalInstructions(originalAssembly.GetType("MidnightPawn.Game.TentacleEffects").GetMethod("TryApplySemen", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic),(ILGenerator)null)})).ToList();
            Check(patchedTool.Count(i=>i.operand is MethodInfo m && m.Name == "ToolGender") == 1, "real dependency tool gender check replaced once");
            var namesType = compat.Assembly.GetType("ItemPortraitHD.NameCompatibility");
            var regionRows = new List<string[]> { new[]{"中国", "姓", "名"}, new[]{"日本", "姓", "名"}, new[]{"韩国", "姓", "名"}, new[]{"英语国家", "姓", "名"} };
            var selectRegions = namesType.GetMethod("SelectRegions", BindingFlags.Static | BindingFlags.NonPublic);
            foreach (var pair in new[]{new[]{"不限制（原版）", "4"}, new[]{"仅日本", "1"}, new[]{"仅中国", "1"}, new[]{"仅中日韩", "3"}})
            {
                var selected = (List<string[]>)selectRegions.Invoke(null, new object[]{regionRows, pair[0]});
                Check(selected.Count == int.Parse(pair[1]) && (pair[0] != "仅日本" || selected[0][0] == "日本") &&
                    (pair[0] != "仅中国" || selected[0][0] == "中国"), "name region restriction " + pair[0]);
            }
            Check(regionRows.Count == 4, "name restrictions preserve original region library");
            foreach (var spec in new[]{new[]{"NameGen", "RandomName", "RegionalNames", "FilterRegions", "4"}})
            {
                var target = originalAssembly.GetType("MidnightPawn.Game." + spec[0]).GetMethod(spec[1], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                var inputCode = PatchProcessor.GetOriginalInstructions(target, (ILGenerator)null);
                var outputCode = ((IEnumerable<CodeInstruction>)namesType.GetMethod(spec[2], BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[]{inputCode})).ToList();
                Check(outputCode.Count(i => i.operand is MethodInfo m && m.Name == spec[3]) == int.Parse(spec[4]), "dependency name compatibility hook " + spec[1]);
            }
            var effectsType = originalAssembly.GetType("MidnightPawn.Game.TentacleEffects");
            var applySemen = effectsType.GetMethod("TryApplySemen", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var semenIL = PatchProcessor.GetOriginalInstructions(applySemen, (ILGenerator)null);
            Check(semenIL.FindIndex(i => i.operand is MethodInfo m && m.Name == "MarkAffected") < semenIL.FindIndex(i => i.opcode == OpCodes.Stfld && i.operand is FieldInfo f && f.Name == "tradeReputationMultiplier"), "semen customer marker occurs before reputation multiplication");
            Check(compat.Assembly.GetType("ItemPortraitHD.SemenLimit") == null, "custom repeated-dose limiter removed entirely");
            var weightsType = compat.Assembly.GetType("ItemPortraitHD.PortraitWeights");
            var weightsIL = PatchProcessor.GetOriginalInstructions(weightsType.GetMethod("Load", BindingFlags.NonPublic | BindingFlags.Static), (ILGenerator)null);
            Check(weightsIL.Any(i => i.operand is MethodInfo m && m.Name == "GetModDataFolder") && weightsIL.Any(i => i.operand is MethodInfo m && m.Name == "Copy"), "personal weights use game user data with legacy migration");
            var exportMethod = injector.GetMethod("ExportInjected", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            var exportCode = PatchProcessor.GetOriginalInstructions(exportMethod, (ILGenerator)null);
            Check(exportMethod.GetParameters().Length == 2 && exportCode.Any(i => i.operand is MethodInfo m && m.Name == "EncodeToPNG") &&
                !exportCode.Any(i => i.operand is MethodInfo m && (m.Name == "ModifyTag" || m.Name == "SetStr")),
                "dependency export is a PNG diagnostic dump with no item persistence writes");
            var factoryType = originalAssembly.GetType("MidnightPawn.Game.ItemFactory");
            const BindingFlags factoryFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            var factory = factoryType.GetMethod("CreateMaidenOfFaction", factoryFlags, null, new[] { typeof(string) }, null) ??
                factoryType.GetMethod("CreateMaiden", factoryFlags);
            Check(factory.GetMethodBody().LocalVariables[0].LocalType == typeof(GameItem), "dependency factory local zero is the created item");
            var originalInstructions = PatchProcessor.GetOriginalInstructions(factory, (ILGenerator)null);
            Check(originalInstructions.Any(i => i.operand is MethodInfo m && m.DeclaringType?.FullName == "MidnightPawn.Game.NameGen" && m.Name == "RandomName"), "scavenging item factory uses the restricted shared name generator");
            var restore = originalAssembly.GetType("MidnightPawn.Game.SpriteInjector").GetMethod("EnsureMaidenSprite", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            Check(restore != null && PatchProcessor.GetOriginalInstructions(restore, (ILGenerator)null).Any(i =>
                i.operand is MethodInfo m && m.DeclaringType == typeof(GameItem) && m.Name == "SetSpriteAndShapeFromMod"),
                "dependency save restore still reaches native sprite-and-shape validation hooks");
            var factoryTranspile = compat.GetMethod("FactoryTranspiler", BindingFlags.NonPublic | BindingFlags.Static);
            var factoryResult = ((IEnumerable<CodeInstruction>)factoryTranspile.Invoke(null, new object[]{originalInstructions})).ToList();
            bool identityPath = PatchProcessor.GetOriginalInstructions(factory, (ILGenerator)null).Any(i => i.operand is MethodInfo m && m.Name == "SpriteFor");
            Check(factoryResult.Count(i => i.operand is MethodInfo m &&
                (m.Name == "PickForItem" || m.Name == "PickPrimary" || m.Name == "PickFallback")) == (identityPath ? 2 : 1),
                "factory primary and fallback selections both use weighted portrait flow");
            Check(!factoryResult.Any(i => i.operand is MethodInfo m && m.DeclaringType?.FullName == "MidnightPawn.Game.MaidenIdentity" && (m.Name == "SpriteFor" || m.Name == "InjectSprite")), "factory cannot bypass HD portrait injection through identity path");
            Check(factoryResult.Count(i=>i.operand is MethodInfo m && m.Name=="PickFaction")==1, "dependency faction call replaced once before item attributes");
            Check(factoryResult.Count(i => i.operand is MethodInfo m && m.Name == "PickBody") == 1, "new capture draws its body through preference weighting once");
            Check(factoryResult.Count(i => i.operand is MethodInfo m && m.Name == "PickBreastForBody" && m.DeclaringType?.FullName == "ItemPortraitHD.AdultBodySources") == 1,
                "new capture routes body-dependent breast selection through configurable picker");
            int bodyPickIndex = factoryResult.FindIndex(i => i.operand is MethodInfo m && m.Name == "PickBody");
            int factionPickIndex = factoryResult.FindIndex(i => i.operand is MethodInfo m && m.Name == "PickFaction");
            Check(bodyPickIndex < factionPickIndex && factoryResult[bodyPickIndex + 1].opcode == OpCodes.Stloc_S &&
                factoryResult[factionPickIndex - 1].opcode == OpCodes.Ldloc_S &&
                Equals(factoryResult[bodyPickIndex + 1].operand, factoryResult[factionPickIndex - 1].operand),
                "body is drawn and stored before faction selection, which receives that same body local");
            var bodyType = compat.Assembly.GetType("ItemPortraitHD.AdultBodySources");
            var eligible = bodyType.GetMethod("Eligible", BindingFlags.Static | BindingFlags.NonPublic);
            var eligibleIL = PatchProcessor.GetOriginalInstructions(eligible, (ILGenerator)null);
            Check(eligibleIL.Any(i => i.operand is MethodInfo m && m.Name == "PoolKey") &&
                eligibleIL.Any(i => i.operand is MethodInfo m && m.Name == "IsMappedReplacementId"),
                "body source list requires mapped replacements with registered five-faction roles");
            Check(PatchProcessor.GetOriginalInstructions(bodyType.GetMethod("PoolKey", BindingFlags.Static | BindingFlags.NonPublic), (ILGenerator)null)
                .Any(i => i.operand is MethodInfo m && m.Name == "IsCaptureKey"),
                "body provider dropdown shares the registered role boundary with automatic selection");

        }
        if (args.Length > 1)
        {
            var scanner = typeof(GameItem).Assembly.GetType("ModScanner");
            var scan = scanner.GetMethod("Scan", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var assemblyPath = System.IO.Path.Combine(args[1], "ItemPortraitHD.dll");
            var allowed = Assembly.LoadFrom(assemblyPath).GetReferencedAssemblies().Select(a=>a.Name).ToArray();
            var scanResult = scan.Invoke(null, new object[]{new[]{assemblyPath}, allowed, args[1], false});
            var resultType = scanResult.GetType();
            var hardFlags = (System.Collections.IList)resultType.GetField("HardFlags").GetValue(scanResult);
            string error = (string)resultType.GetField("Error").GetValue(scanResult);
            if (hardFlags.Count > 0 || !string.IsNullOrEmpty(error))
                throw new Exception("Scanner: " + error + " " + string.Join("; ", hardFlags.Cast<object>()));
            Check(true, "game scanner reports no hard flags on candidate folder");
            var capabilities = (IEnumerable<string>)resultType.GetField("Capabilities").GetValue(scanResult);
            Check(capabilities.All(c=>new[]{"files","reflection-external","dynamic-code"}.Contains(c)), "scanner capabilities covered by manifest");
        }
        Console.WriteLine($"{checks} checks passed");
    }
}


