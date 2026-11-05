# CorpPresence reflection/no-hooks feasibility — actual DLL metadata
DLL: CorpPresence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

## CorpPresence.Character
F System.String _prefix
P System.String Id get 
P System.String Name get 
P System.String Faction get 
P System.Int32[] Thresholds get 
P System.Int32 CashNeeded get 
P System.String PortraitRef get 
P System.Int32 BuyGoodsNeeded get 
P System.String Prefix get 
P System.Int32 Chapters get 
P System.String[] RefuseIds get 
P System.ValueTuple`2<System.Int32,System.Int32> DefaultDailyGap get 
P System.Boolean HasDaily get 

## CorpPresence.Engine
F static CorpPresence.Character[] All
F static System.Boolean modreq(System.Runtime.CompilerServices.IsVolatile) BuyActive
F static System.Int32 MinStock
F static System.Int32 MaxStock
F static System.Boolean _scheduling
M StoreClient CorpPresence.Engine::Create(CorpPresence.Character,System.String)
M System.Void CorpPresence.Engine::OnDayStart()

## CorpPresence.Engine/<>c
F static CorpPresence.Engine/<>c <>9
F static System.Func`2<System.ValueTuple`2<CorpPresence.Character,CorpPresence.CharacterState>,System.Int32> <>9__17_2
F static System.Func`2<System.ValueTuple`2<CorpPresence.Character,CorpPresence.CharacterState>,System.Int32> <>9__17_3

## CorpPresence.Engine/<>c__DisplayClass11_0
F CorpPresence.CharacterVisitSession visitSession
F System.Boolean done
F CorpPresence.Character c
F CorpPresence.Visit v
F System.String kind
F StoreClient client
F System.Action startTrade
F System.Action <>9__2
M System.Void CorpPresence.Engine/<>c__DisplayClass11_0::<Create>b__0()
M System.Void CorpPresence.Engine/<>c__DisplayClass11_0::<Create>b__1()
M System.Void CorpPresence.Engine/<>c__DisplayClass11_0::<Create>b__2()

## CorpPresence.Engine/<>c__DisplayClass11_1
F System.Boolean choiceMade
F CorpPresence.Engine/<>c__DisplayClass11_0 CS$<>8__locals1

## CorpPresence.Engine/<>c__DisplayClass11_2
F CorpPresence.Choice choice
F System.Action then
F CorpPresence.Engine/<>c__DisplayClass11_1 CS$<>8__locals2
M System.Void CorpPresence.Engine/<>c__DisplayClass11_2::<Create>b__3()

## CorpPresence.Engine/<>c__DisplayClass12_0
F CorpPresence.CharacterVisitSession visitSession
F CorpPresence.Character c

## CorpPresence.Engine/<>c__DisplayClass17_0
F CorpPresence.CorpState state
F System.Int32 tomorrow
F System.Func`2<CorpPresence.Character,System.ValueTuple`2<CorpPresence.Character,CorpPresence.CharacterState>> <>9__0
F System.Func`2<System.ValueTuple`2<CorpPresence.Character,CorpPresence.CharacterState>,System.Boolean> <>9__1

## CorpPresence.Engine/<>c__DisplayClass25_0
F StoreClientManager manager
F System.Action`1<StoreClientManager> extra

## CorpPresence.Engine/<>c__DisplayClass7_0
F CorpPresence.Character c
F CorpPresence.CharacterState s

## CorpPresence.Engine/<>c__DisplayClass9_0
F System.String[] pool
F System.Random rng

## CorpPresence.Visit
F System.String[] Lines
F System.String Exit
F System.Boolean Buy
F System.Boolean Both
F System.Int32 Discount
F System.String[] Stock
F System.String[] Gift
F System.String[] Extra
F System.String Wrong
F System.Boolean TalkOnly
F System.String Ask
F CorpPresence.Choice[] Choices

## CorpPresence.Company
F static System.Int32[] TierAt
F static System.Int32 CardAt
P System.String Id get 
P System.String Name get 
P System.String CharacterId get 
P System.Int64 PhoneNumber get 
P System.String[] TierNames get 
P System.String[] FrontDesk get 
P System.String FrontDeskDone get 
P System.String CardFlavor get 
P System.String DeskName get 
P System.String DeskAsk get 
P CorpPresence.CatalogEntry[] Catalog get 
P System.Boolean Active get 

## CorpPresence.Company/<>c__DisplayClass26_0
F CorpPresence.CompanyState s

## CorpPresence.Company/<>c__DisplayClass28_0
F CorpPresence.CompanyState s

## CorpPresence.Company/<>c__DisplayClass29_0
F System.Func`4<CorpPresence.Company,CorpPresence.CompanyState,System.Int32,System.Int32> f
F CorpPresence.Company <>4__this
F CorpPresence.CompanyState s
F System.Int32 tier

## CorpPresence.Company/<>c__DisplayClass31_0
F System.Func`2<CorpPresence.Company,System.Double> f
F CorpPresence.Company c

## CorpPresence.CompanyVisitors
F static System.Random Rng
F static System.String[] PastelGoods
F static System.Int32 MaxCratesPerDelivery
P System.Int32 Day get 
M System.Action CorpPresence.CompanyVisitors::CreateCourierLeave(StoreClient)
M System.Action CorpPresence.CompanyVisitors::CreateDeliveryReceipt(CorpPresence.Company,CorpPresence.CompanyState,System.Collections.Generic.List`1<CorpPresence.Order>,System.Boolean)

## CorpPresence.CompanyVisitors/<>O
F static System.Func`1<System.Collections.Generic.List`1<System.String>> <0>__RollModBox

## CorpPresence.CompanyVisitors/<>c
F static CorpPresence.CompanyVisitors/<>c <>9
F static System.Func`2<CorpPresence.Order,System.Boolean> <>9__4_0
F static System.Func`2<CorpPresence.Visitor,System.Boolean> <>9__4_1
F static System.Func`2<CorpPresence.Order,System.Int32> <>9__13_0
F static System.Func`2<CorpPresence.Order,System.Boolean> <>9__13_1
F static System.Func`2<CorpPresence.Order,System.Boolean> <>9__13_2
F static System.Func`2<CorpPresence.Order,System.Boolean> <>9__13_3
F static System.Func`2<CorpPresence.Order,System.Boolean> <>9__13_4
F static System.Action <>9__14_1
F static System.Action <>9__15_2
F static System.Func`2<CorpPresence.Order,System.Int32> <>9__21_0

## CorpPresence.CompanyVisitors/<>c__DisplayClass14_0
F System.Boolean placed
F System.String[] stock
F StoreClient client

## CorpPresence.CompanyVisitors/<>c__DisplayClass15_0
F System.Collections.Generic.HashSet`1<System.String> list
F StoreClient client

## CorpPresence.CompanyVisitors/<>c__DisplayClass17_0
F System.Boolean done
F CorpPresence.StoreSession session
F StoreClient client
M System.Void CorpPresence.CompanyVisitors/<>c__DisplayClass17_0::<CreateCourierLeave>b__0()

## CorpPresence.CompanyVisitors/<>c__DisplayClass18_0
F System.Int64 generation
F PlayerStore store
F CorpPresence.CorpState root
F CorpPresence.Company c
F CorpPresence.CompanyState s
F System.Collections.Generic.List`1<CorpPresence.Order> orders
F System.String run
F System.Boolean done
F System.Boolean card
F System.Func`2<CorpPresence.Order,System.Boolean> <>9__2
M System.Boolean CorpPresence.CompanyVisitors/<>c__DisplayClass18_0::<CreateDeliveryReceipt>g__IsCurrent|0()
M System.Void CorpPresence.CompanyVisitors/<>c__DisplayClass18_0::<CreateDeliveryReceipt>b__1()
M System.Boolean CorpPresence.CompanyVisitors/<>c__DisplayClass18_0::<CreateDeliveryReceipt>b__2(CorpPresence.Order)

## CorpPresence.CompanyVisitors/<>c__DisplayClass21_0
F System.Collections.Generic.HashSet`1<System.String> failed
F System.Func`2<System.String,System.Boolean> <>9__2
F System.Func`2<CorpPresence.Order,System.Boolean> <>9__1

## CorpPresence.CompanyVisitors/<>c__DisplayClass24_0
F System.String id

## CorpPresence.Hooks
F static System.Action _nightly
F static System.String _reportedId
F static System.Int32 _revBefore
M System.Void CorpPresence.Hooks::AfterInitPhones(System.Collections.Generic.Dictionary`2<System.Int64,StorePhoneClient>)
M System.Boolean CorpPresence.Hooks::BeforeLoadFromAtlas(System.String,UnityEngine.Sprite&)
M System.Void CorpPresence.Hooks::AfterLoadGame(System.Boolean)

## CorpPresence.Hooks/<>O
F static System.Action <0>__OnNightlyServices
F static System.Action <1>__OnEndDay
F static System.Action <2>__OnEndDay
F static System.Action <3>__OnDayEnd
F static System.Action <4>__CloseAll
F static System.Action <5>__Save
F static System.Action <6>__BeforeSave
F static System.Action <7>__RemoveRestoredMedicinePurity

## CorpPresence.Hooks/<>c
F static CorpPresence.Hooks/<>c <>9
F static System.Action <>9__8_0
F static System.Action <>9__11_0
F static System.Action <>9__40_0

## CorpPresence.Hooks/<>c__DisplayClass13_0
F StoreClient __instance
F GameItem item
F System.Int32 cost

## CorpPresence.Hooks/<>c__DisplayClass14_0
F StoreClient __instance
F GameItem item
F System.Int32 soldAmount

## CorpPresence.Hooks/<>c__DisplayClass15_0
F System.Collections.Generic.Dictionary`2<System.Int64,StorePhoneClient> __result
M System.Void CorpPresence.Hooks/<>c__DisplayClass15_0::<AfterInitPhones>b__0()

## CorpPresence.Hooks/<>c__DisplayClass26_0
F System.Action`2<System.String,System.Int32> f
F System.String id
F System.Int32 delta

## CorpPresence.Hooks/<>c__DisplayClass43_0
F System.Int64 currentNumber

## CorpPresence.Hooks/<>c__DisplayClass48_0
F StoreClientManager __instance

## CorpPresence.Hooks/<>c__DisplayClass49_0
F PlayerStore __instance

## CorpPresence.Hooks/<>c__DisplayClass53_0
F GameItem __instance
F System.Func`2<GameItem,System.Double> bf

## CorpPresence.ItemRegistry
F static System.Collections.Generic.Dictionary`2<System.String,System.Func`1<GameItem>> Factories
F static System.Collections.Generic.HashSet`1<System.String> Registered
F static System.Collections.Generic.HashSet`1<System.String> Reported
F static ModItemDirectory _directory
F static ModItemDirectory _pendingDirectory
F static System.Action`1<ModItemDirectory> _onInit
M System.Void CorpPresence.ItemRegistry::AfterInitDirectory(ModItemDirectory)
M System.Void CorpPresence.ItemRegistry::Register(ModItemDirectory)
M GameItem CorpPresence.ItemRegistry::Create(System.String)

## CorpPresence.ItemRegistry/<>O
F static System.Action`1<ModItemDirectory> <0>__Register

## CorpPresence.ItemRegistry/<>c__DisplayClass10_0
F CorpPresence.ItemIdentity/Entry entry
M GameItem CorpPresence.ItemRegistry/<>c__DisplayClass10_0::<Register>b__0()

## CorpPresence.Main
F static System.String Version
F static System.String DisplayName
F HarmonyLib.Harmony _harmony
F UnityEngine.GameObject _host
F UnityEngine.GameObject _bootstrapObject
F CorpPresence.Runtime.ResourceBootstrap _bootstrap
F CorpPresence.Runtime.RuntimeLifetime _runtime
M System.Void CorpPresence.Main::Init(ModManifest)
M System.Void CorpPresence.Main::CompleteStartup()
M System.Void CorpPresence.Main::CreateBootstrap()
M System.Void CorpPresence.Main::CreateHost()

## CorpPresence.Main/<>O
F static System.Action <0>__Begin
F static System.Action <1>__Reset
F static System.Action <2>__Install
F static System.Action <3>__Uninstall
F static System.Action <4>__InstallResources
F static System.Action <5>__UninstallResources
F static System.Action <6>__RegisterPlant
F static System.Action <7>__UnregisterPlant
F static System.Action <8>__InstallEvents
F static System.Action <9>__UninstallEvents
F static System.Action <10>__Install
F static System.Action <11>__Uninstall
F static System.Action <12>__Noop
F static System.Action <13>__ReleaseUi
F static System.Action <14>__ResetCachedFont
F static System.Action <15>__ResetSceneReferences
F static System.Action <16>__Destroy
F static System.Action <17>__Destroy
F static System.Action <18>__ReleaseUi

## CorpPresence.Main/<>c
F static CorpPresence.Main/<>c <>9
F static System.Func`2<CorpPresence.Character,System.String> <>9__11_0
F static System.Action`1<System.Exception> <>9__18_0

## CorpPresence.L
F static System.Int32 _frame
F static System.Boolean _en
F static System.Boolean _ready
P System.Boolean En get 

## CorpPresence.Phone
F static System.Int32 Regular
F static System.Int32 NotResponding
F static System.Int32 Supplier
F static System.Collections.Generic.HashSet`1<System.Int64> Warned
F static System.Int64 _callGeneration
F static System.String Shunned
M System.Void CorpPresence.Phone::Register(System.Collections.Generic.Dictionary`2<System.Int64,StorePhoneClient>)
M System.Void CorpPresence.Phone::RegisterCurrent()

## CorpPresence.Phone/<>c__DisplayClass16_0
F System.Func`2<CorpPresence.Company,Dialogue> custom
F CorpPresence.Company c
F System.Int64 generation
F CorpPresence.StoreSession session
F System.Boolean opened
F System.Boolean hungUp

## CorpPresence.Phone/<>c__DisplayClass17_0
F System.Boolean done
F System.Int64 generation
F CorpPresence.StoreSession session
F CorpPresence.Company c
F System.Action action

## CorpPresence.UI.MenuLabels
F static System.String[][] Chinese
F static System.String[][] English
F static System.String[] _lastBase
F static System.String _lastExtra
F static System.String[] _lastCombined
