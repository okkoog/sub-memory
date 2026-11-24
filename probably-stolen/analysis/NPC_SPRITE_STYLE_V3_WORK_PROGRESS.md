# All Girls / More Girls — NPC sprite art system V3
Checkpoint: 2026-11-24 (KST)

## Scope and state
Goal: recreate a consistent game NPC dialogue-sprite art system for new adult characters, without uploading dozens of reference images into chat. Source repository is the sub-memory. Work on generic NPC dialogue sprites only; exclude circular portraits, event CG, props, icons and unrelated variants. Only clearly adult, non-explicit references are in scope. Do not modify original mods or localization files.

Prior session reported visually examining 30 sprites (11 initial + 19 additional). This is inherited analysis, not a new independent visual verification:
- All Girls (19): 1, 2, 10, 16, 20, 24, 56, 100, 101, 104, 110, 120, 125, 139, 143, 150, 164, 175, 183.png.
- More Girls (11): knight, knight_royal, green_cloak, elf, machine_girl, dark_elf, angel, ghost, jester, idol, ballet.png.
- Source directories: probably-stolen/mods/3838352006/Textures/Items and probably-stolen/mods/3838820208/Textures/Items.
- Current tree check: 201 and 189 PNG files respectively (390 combined; NOT 390 NPCs). Tree SHAs: 92f68105698728af4bcf74820471ccaf25d94b4b, 740160bc06c346fddf98cf67f2b8473473d41eb4.
- Current session attempted PNG byte access via GitHub fetch and fetch_blob; both failed on non-UTF-8 binary. Direct raw URL from runtime failed DNS, and web open failed DisabledError. No new pixel measurements were obtained at that checkpoint. Superseded by the measured checkpoint appended below.

## Inherited visual findings
- Registration/overall visible height seems more consistent than anatomical lower crop. Waist, hip, upper thigh crops vary with pose, costume, and character.
- Compact stylized adult anatomy: relatively small head, short neck, smooth shoulders, simple contours, silhouette-first design; individual physique varies. Do not impose a universal head/body width ratio.
- Mostly frontal eye-level dialogue pose; gestures, held items, seated poses can vary. Avoid action/cinematic perspective.
- Upper lashes are the strongest shared feature: dark burgundy or near-black purple, tapered outer corners; layered irises, compact facial features, subtle diagonal blush. Expression and eye shape depend on personality.
- Broad flat base colors, curved primary cel-shadow boundaries, restrained contact shadows, small opaque highlights; localized gradient permitted. 110 and 139 were reported as mixed shading examples.
- Hair: large overlapping locks, sparse fine inner strands, tapered ends, irregular compact highlights following locks.
- Material treatment varies: matte cloth broad/simple, glossy fabric crisp irregular reflections, metal sharp color bands, translucent cloth controlled transparency, lace/embroidery repeat patterns, gems geometric contrast.
- All Girls reported more varied faces, poses, and shading; More Girls more consistent face rendering, frontal composition and distinct material handling.
- Highest priorities: registration/silhouette, eye/lash rendering, body proportions, shading; then hair, colored line art, materials; then fine costume details and pose/expression.

## V3 modular prompt — provisional, not pixel-verified
### A. Fixed shared style
Create a clearly adult anime character belonging to a unified NPC dialogue sprite collection. Maintain a consistent visual language across different identities without forcing identical anatomy, facial expressions or poses.

Use compact, stylized adult anime anatomy with a relatively small head, short neck, smoothly rounded shoulders, simplified anatomical contours and clearly differentiated body regions. Preserve character-specific physique within the shared visual system. Construct the silhouette using broad, continuous curves rather than realistic anatomical surface details. Avoid elongated fashion-model anatomy, excessive muscle definition, prominent skeletal landmarks and realistic skin folds.

Use predominantly frontal, eye-level conversational compositions. Allow subtle character-specific head tilts, shoulder movements, hand gestures, held accessories and seated poses when appropriate. Avoid cinematic perspective, extreme foreshortening and dynamic action compositions.

Preserve individual personality through eye angle, eyebrow shape and mouth expression. Use solid dark burgundy or near-black purple upper lash shapes, sharply tapered outer corners, layered irises, compact facial features and subtle diagonal blush strokes. Do not force identical eye shapes.

Use predominantly flat color regions with broad, clearly separated cel-shaded shadows. Construct shadows according to curvature and overlapping forms. Use limited secondary shadows and subtle localized gradients only where they improve material definition. Keep illuminated areas broad and uncluttered. Use compact opaque highlights, small reflective marks and irregular short highlight strokes instead of realistic continuous specular rendering. Use restrained colored contours and fine internal details.

Matte fabric: broad color regions, restrained highlights, simplified folds. Glossy synthetic fabric: strong local contrast, sharp reflection boundaries, irregular reflective streaks. Metal: distinct light and dark color bands, crisp edge highlights, simplified reflections. Translucent fabric: controlled transparency, visible underlying colors, sharply defined folds. Lace and embroidery: clean repeating patterns with fine colored outlines. Jewelry and gemstones: compact geometric highlights and saturated color separation.

Build hairstyles from large flowing locks, clearly separated overlapping layers, thin internal strand lines and tapered tips. Use irregular clusters of bright reflective strokes following each lock's curvature.

### B. Character-specific module
Character: [adult identity]. Physique: [individual adult proportions]. Personality/expression: [description]. Hair, face, costume, colors, accessories: [recognizable details]. Pose: [appropriate dialogue pose].
Use supplied reference only for identity, hairstyle, colors, costume, accessories and recognizable personality. Reconstruct composition, anatomy, line art, eye rendering, shading and materials according to shared NPC sprite system. Do not reproduce the reference as a standalone promotional illustration.

### C. Output registration (provisional)
Target 1152 x 844 landscape canvas; transparent RGBA background. Single character horizontally centered, anchored to the bottom. Uppermost visible point near Y=188, continuing to the bottom boundary. Depending on physique, costume, headwear and pose, lower boundary may naturally intersect waist, hips or upper thighs. Do not force identical anatomical cropping. Keep overall registered height consistent. No scene, text, logo, border or separate panel. Target coordinates are a prior visual hypothesis, not measured results.

## Original quantitative verification plan (see updated checkpoint below)
1. Obtain binary PNG data from repository in a runtime that can read it without displaying each image in chat. Verify source revision, actual file type, dimensions, alpha channel, and candidate scope. If access fails, report failure instead of inventing data.
2. Reuse the 30-name manifest. For each eligible adult sprite, calculate alpha bbox at thresholds alpha>=1, >=128 and >=250. Use inclusive bounds or clearly state exclusive bounds. Record canvas dimensions, bbox coordinates, margins, visible height/width, center offset, bottom contact, and alpha coverage. Separate headwear/accessory outliers. Use mod-wise medians, ranges and IQR; verify Y=188 hypothesis and bottom alignment. Do not assume all PNGs have alpha.
3. Measure semantic landmarks (head width/height, face width, shoulder width, torso width) only where reliably annotated from images. Alpha bbox is not equivalent to anatomical landmarks. Record pose, crop, occlusion and confidence. Do not infer anatomy from the bounding box alone.
4. Color: exclude transparent pixels; distinguish overall palette from manually marked skin/hair/fabric regions; compare color quantization, edge contrast, shade transitions and highlight frequency. Pixel counts alone do not establish artistic technique.
5. Revise V3 only after measured evidence; label measured, visually inferred, and provisional statements separately. Save compact TSV and markdown results in this analysis area, no copies of PNGs, no bulk image attachments. Update checkpoint after completed batches; resume from first incomplete stage.
6. Test prompt on distinct adult character identities and compare registration, facial technique, shading, silhouette and identity fidelity. Keep generated results separate from source measurements.

## 2026-11-24 — quantitative checkpoint (30/30 complete for pixel geometry)
Access resolved using GitHub fetch_file encoding=base64. PNGs were decoded within the processing runtime, without displaying or copying them into chat. Decoder verifies decompressed length and zlib Adler-32. Five JSON batches and consolidated NPC_SPRITE_PIXEL_METRICS_20261124.tsv saved under this directory. Reusable helper: npc_sprite_png_measure.js (function-body source used by the processing runtime).

Measured results, alpha>=1, zero-based inclusive coordinates:
| Metric | All Girls (19) | More Girls (11) | Combined (30) |
|---|---:|---:|---:|
| Canvas | 1152x844, 19/19 | 1152x844, 11/11 | 1152x844, 30/30 |
| PNG format | 8-bit RGBA | 8-bit RGBA | 8-bit RGBA |
| Top Y | 188 | 188 | 188 |
| Bottom Y | 843 | 843 | 843 |
| Visible height | 656 | 656 | 656 |
| Width min/median/max | 448/598/800 | 553/647/656 | 448/634/800 |
| Bbox center offset from canvas center | -0.5 to 0 px | -0.5 to 0 px | -0.5 to 0 px |
| Visible alpha coverage min/median/max | 21.17/26.95/38.94% | 23.94/29.02/32.09% | 21.17/28.42/38.94% |
| RGB 4-bit/channel occupied bins min/median/max (alpha>=250) | 412/729/1142 | 196/801/1425 | 196/730.5/1425 |
| Top 5 RGB bins median coverage (alpha>=250) | 44.05% | 41.64% | 42.64% |

Alpha>=128: top Y 188–190, bottom 841–843; alpha>=250: top 188–191, bottom 840–843. Exactly alpha=255 is not a reliable mask: median fraction of visible pixels that are exactly 255 is only 0.61%; 20.png is an outlier at 93.82%. The high-alpha pixels nevertheless dominate most silhouettes. Do not infer material translucency simply from this alpha distribution. Palette bins are coarse RGB groups, not count of painted shades; mixed materials and costume colors confound comparison.

### V4 output registration module (measured replacement for V3 module C)
Use an exact 1152 x 844 pixel landscape RGBA canvas with a fully transparent background. Reserve rows 0–187 as transparent top padding. The nonzero-alpha silhouette must occupy rows 188–843 inclusive (656 pixels high). Center the complete visible silhouette horizontally, including hair, headwear, and accessories; do not center the face alone. Bounding-box center must be within 0.5 pixel of the canvas center. Preserve aspect ratio and character-specific silhouette width; do not stretch the character or force one crop across characters. Typical observed width is 559.5–654 pixels (combined interquartile range), with observed extremes of 448–800 pixels; these are descriptive, not mandatory limits. Keep the waist/hip/thigh crop character-dependent. Use clean antialiased alpha edges, no background, logo, text, frame or scenery. For exact production registration, crop the nonzero-alpha bounding box, scale proportionally to 656 pixels high, center and place on a transparent canvas at y=188; inspect whether resampling or stray pixels affect bounds. Do not intentionally make interior opaque colors transparent solely to match alpha statistics. If generation alone cannot guarantee pixel registration, treat post-processing as required for exact delivery.

### Remaining work (not completed)
- Semantic anatomical measurements: head/face/shoulder/torso landmark annotation on clearly adult general NPC sprites; alpha geometry cannot identify body landmarks. Need reliable direct visual inspection, small batches or local image processing. Do not extrapolate anatomical ratios from bbox widths.
- Local skin/hair/fabric color segmentation and edge/shadow analysis; global palette measurements complete, material-specific color analysis not complete.
- V4 full style prompt: fixed style and character modules remain provisional V3, measured registration module above replaces V3 C. Finalize after anatomical/material verification.
- Generation testing and comparison not started. No additional visual samples claimed in this session. No original mod files modified.


## 2026-11-24 11:14 KST — semantic/material verification, batch 1 (6 sprites)
This is a new direct visual inspection of six already-manifested sprites for **previously unfinished semantic and material stages**, not a repeat of the completed alpha-geometry stage. No additional unique samples are added to the 30-image manifest. Source blobs:
- More Girls knight ef7c7e6e34598f87003faa12dd03811c798a380a; knight_royal f4df19d9af99a33f2da21505386c451259c3a3f5; green_cloak 7fc4230e4e00a4f0331c0b714acda11d0a8209d0.
- All Girls 1 801068f3bc5443704097c671a03fe4ffc3ea94f5; 56 0e08497b71072144600d0f5bbf9ee11500642c85; 139 a0d6bdf2bb2c749d8a88118e9c2104b66ef4e6c2.
Adult fictional NPC scope is the project premise; age is not inferred from physique. Analysis limited to non-explicit visual design, face, clothing, pose and color technique. No PNG files added to repository.

### Semantic landmark annotation (approximate visual readings, NOT automated exact anatomical measurements)
Coordinates in original 1152x844 canvas, approximate to ~5–10 px. Eye positions refer to iris centers, not lash extents. Crown = visible main hair mass top, excluding isolated ahoge; not an observed skull landmark. X pair denotes left/right as viewed. H=chin Y minus crown Y. Face boundaries and anatomical shoulder joints are obscured by hair, garment or pose; leave those measurements unreported rather than inventing them.
| Sprite | Main hair crown Y | Chin Y | Iris centers X | Iris Y | Inter-iris separation | H | Pose / occlusion |
|---|---:|---:|---|---:|---:|---:|---|
| knight | ~188 | ~431 | ~532,620 | ~349 | ~88 | ~243 | frontal, one held accessory, shoulders partly covered by hair |
| knight_royal | ~230 | ~436 | ~543,615 | ~365 | ~72 | ~206 | frontal, arms bent; armor masks anatomical shoulders; separate ahoge reaches y188 |
| green_cloak | NA | ~452 | ~542,618 | ~379 | ~76 | NA | frontal, both hands extended; hood hides crown and shoulders |
| 1 | ~188 | NA | ~540,616 | ~342 | ~76 | NA | frontal, raised collar masks chin, long hair covers body sides |
| 56 | NA | ~431 | ~544,620 | ~363 | ~76 | NA | frontal, veil masks crown; shoulders partly visible but hair interrupts contour |
| 139 | ~202 | NA | ~582,660 | ~365 | ~78 | NA | frontal, face shifted right of canvas center; flute crosses lower face; hands raised |
- Two valid visible hair-crown-to-chin spans: approximately 206 and 243 px, 31–37% of registered silhouette height; illustrative only, not a whole-head anatomical ratio. Inter-iris separation ~72–88 px in these six samples. In the two unoccluded hair-crown/chin examples, inter-iris separation / H ~0.35–0.36 and iris-to-chin / H ~0.34. Small n, hair mass differs, not mandatory generation targets.
- Do NOT equate top registration with skull crown or face center. Royal's isolated hair curl sets upper extent, cloak and veil set upper extent in other sprites, 139's face is offset by flute pose. This directly supports centering the complete silhouette rather than centering the face.
- Body outline is not an anatomical width: cape, long hair, armor, elbows and hand gestures may dominate horizontal silhouette. Actual shoulder and torso width require more suitably unobstructed adult samples; no population-level physique ratio established. Preserve individual build and natural crop.
- Facial consistency: dark burgundy/black-purple upper lash silhouette with outward-pointing tapered ends; irises layered with dark upper band, bright lower colors and multiple discrete reflections; narrow simplified nose, compact mouth, fine diagonal cheek strokes. Even within this subset eye aperture, mouth and eyebrow shape differ. No single expression template.
- Material visual verification: knight gold hair has short jagged light marks along lock curvature, royal blue armor has broad high-contrast blue value regions with compact white reflections and gold edge bands; green cloak has broad dark-green fold masses and a fine gold repeat border; 1 has relatively flat lilac collar and gold trim; 56 contrasts dark smooth veil with pale layered hair; 139 has softly transitioning hair/skin tones, small defined highlights and sharply layered metallic flute reflections. All Girls 139 is a genuine mixed-rendering example within this subset, not proof of the whole mod's distribution.

### Local color/edge measurement — 21 manually selected rectangles
Decoded PNG with prior verified decoder (inflate length + Adler-32); retained raw RGBA instead of counting only whole-image histogram. Rectangles chosen by visual inspection to sample face/hairstyle/garment/accessory, not full semantic masks. Bounds [x0,y0,x1,y1) in source coordinates, alpha>=250. RGB bins 4 bits/channel. Adjacent right/down pixel pairs with both alpha>=250; d=max absolute RGB-channel difference (0–255); p50/p90 use floor-index sorted quantiles; edge16 is percentage of pairs d>=16. Coverage is qualifying-pixel fraction. Dominant bin percentage is local concentration, not number of artist-painted shades. All rectangles have 100% qualifying coverage except flute (97.1%). ROI labels descriptive; small rectangles may include material boundaries, blush, highlights, trim.
| Sprite | Region | ROI | n | Bins | Top bin (%) | d50 | d90 | edge16% |
|---|---|---|---:|---:|---|---:|---:|---:|
| knight | hair | 490,245,520,275 | 900 | 37 | fd8 (56.7) | 6 | 63 | 34.7 |
| knight | cheek | 521,367,549,382 | 420 | 6 | fcc (27.1) | 6 | 17 | 12.5 |
| knight | collar cloth | 550,446,569,459 | 247 | 29 | dbc (76.5) | 1 | 36 | 15.4 |
| knight_royal | hair | 562,255,594,285 | 960 | 21 | f88 (70.8) | 1 | 13 | 8.6 |
| knight_royal | cheek | 549,376,570,391 | 315 | 9 | fdc (81.6) | 1 | 25 | 17.7 |
| knight_royal | blue armor | 425,451,453,475 | 672 | 23 | 7cf (52.2) | 1 | 10 | 7.4 |
| knight_royal | gold trim | 466,430,474,456 | 208 | 21 | fd6 (28.4) | 3 | 40 | 20.7 |
| green_cloak | hair | 580,285,610,312 | 810 | 28 | fff (43.0) | 3 | 26 | 15.7 |
| green_cloak | cheek | 539,396,558,411 | 285 | 12 | e97 (35.1) | 2 | 20 | 16.0 |
| green_cloak | hood + trim boundary | 650,278,680,306 | 840 | 72 | 562 (18.1) | 7 | 100 | 36.3 |
| 1 | hair | 550,212,570,237 | 500 | 21 | c99 (64.2) | 1 | 5 | 6.0 |
| 1 | cheek | 553,350,568,365 | 225 | 2 | fed (66.7) | 1 | 1 | 0.0 |
| 1 | purple collar | 549,390,593,413 | 1012 | 4 | ece (93.9) | 1 | 1 | 0.0 |
| 56 | hair | 552,282,580,313 | 868 | 18 | fff (34.1) | 2 | 18 | 12.4 |
| 56 | cheek | 551,380,566,398 | 270 | 5 | fed (91.9) | 1 | 4 | 3.9 |
| 56 | dark veil | 536,207,564,229 | 616 | 27 | 334 (75.5) | 1 | 13 | 9.3 |
| 56 | blue collar | 550,442,565,462 | 300 | 18 | 88b (71.7) | 2 | 41 | 19.1 |
| 139 | hair | 574,236,606,265 | 928 | 24 | 545 (40.4) | 1 | 14 | 8.7 |
| 139 | cheek | 580,384,601,400 | 336 | 4 | fdc (74.1) | 1 | 7 | 0.0 |
| 139 | metallic flute | 775,411,800,418 | 170 | 34 | fff (17.1) | 15 | 76 | 49.4 |
| 139 | blue collar | 600,453,614,469 | 224 | 16 | 98a (27.7) | 6 | 16 | 12.7 |

Interpretation: 16/21 ROIs have d50 <=3, consistent with broad locally stable areas and discrete edges. All Girls 1 purple collar (93.9% dominant bin; d90=1) contrasts with metal flute (17.1% dominant bin; d90=76) and royal gold trim (d90=40). Knight's hair ROI samples high-contrast highlight edges, while 1's hair ROI is comparatively flat. The hood ROI is explicitly a decorated boundary; it is NOT evidence that matte green cloth itself has the highest edge contrast. The cheek ROIs mix flat skin, blush, lash/hair or neighboring contours depending on framing; not directly comparable as pure skin regions. Sample selection is manual, not randomized. These data support region-dependent rendering and restrained local gradients, not a universal number of shades or an All Girls vs More Girls statistical conclusion.

### V4 style-module amendment — evidence from batch 1 (still provisional)
Use a clearly adult character with an individualized build and recognizable identity. Prefer a compact eye-level dialogue portrait, with a relatively compact face and short neck, not a fixed head/body ratio. Treat hair mass, headwear and held accessories as separate from anatomy. Center the entire sprite; allow the face to move with conversational pose and preserve the character's individual silhouette. Avoid long fashion-illustration proportions, extreme perspective and unnecessary realistic anatomy.
For eyes, use a strong continuous dark wine or purple-black upper lash shape, tapered outer-corner spikes, a darker upper iris and brighter colored lower iris with compact multiple reflection marks. Use compact nose and mouth, and fine diagonal cheek hatching when appropriate. Maintain distinct eye shapes and expressions per identity.
For shading, build broad coherent local color masses; keep most interior areas locally stable, reserve high-contrast transitions for overlap/contact, contours, hair glints, metallic bands, and trim. Combine cel-like shadow shapes with limited smooth shading when appropriate. Do not flatten every material or apply gradients everywhere. Use broad and thin tapered hair locks with irregular clustered highlight strokes following the direction of each lock. Matte cloth should have simpler fold shadows; polished armor and metal should have separate reflection bands and compact high-contrast highlights. Fine trim may be deliberately high-frequency, but do not apply that treatment to plain fabric. Keep translucent or lace rendering character-specific, pending additional verification.
Use existing V3 character-specific module and measured V4 registration module unchanged. This amendment supersedes only overlapping style guidance, not the entire provisional V3 prompt.

### Resume point after this batch
- Pixel geometry and global palette: complete, 30/30; do not repeat.
- Semantic/material batch 1: 6/30 previously manifested sprites directly inspected; 21 local ROIs measured, 2 valid visible hair-crown-to-chin pairs. No exact shoulder/torso measurement yet; do not mark stage complete. Next select clearly adult non-explicit sprites with less occluded head/shoulders and diverse attire/pose from remaining manifest (not these 6); annotate face landmarks, observable garment/body contours separately, and compare matched plain-material vs boundary patches.
- Remaining manifest: All Girls 2,10,16,20,24,100,101,104,110,120,125,143,150,164,175,183; More Girls elf,machine_girl,dark_elf,angel,ghost,jester,idol,ballet. Assess eligibility before using; skip ambiguous or non-general images. Do not claim inspection before doing it.
- Full V4 finalization and generation comparison remain incomplete. No generated test images in this batch. No original mod files touched.


## 2026-11-24 KST — semantic/material verification, batch 2 (5 eligible, 2 deferred)
Continued remaining manifest only, without repeating global pixel geometry or batch 1 annotations. Seven files screened. Five selected for non-explicit face, posture, hair, costume, and material analysis; All Girls 10 and 100 deferred from semantic/material use because adult-reference eligibility is not established with sufficient confidence. Do not infer age from proportions or use those two as prompt references. No additional PNG files added.
Source blobs:
- All Girls 2: 5e198915452c7c774e67f1bf722c6536359597a6
- All Girls 16: e035a25cfbac64d0f27ed9334179ca9819e9bdf7
- All Girls 110: be4decfa433b51711876196d998872813660d1f0
- More Girls machine_girl: 730ffaa198eea3f69c4c78466095efdecd3ae865
- More Girls idol: 348d17c5b343af1feca4e18f04d33c2cdb4e5741
- Deferred 10: 322441ef9beb332f73344df522646fc853a5430a; 100: 7b7f33ce14e62e402d2c60b294768aa0cd2ccc48.

### Additional semantic landmarks (manual approximation, ~5–10 px)
Same definitions as batch 1; iris Y may differ due to head tilt. Hair crown is not skull crown. These are approximate rendering landmarks, not measured human anatomical proportions.
| Sprite | Main hair crown Y | Chin Y | Iris centers X | Iris center Y | Inter-iris horizontal separation | H | Pose / occlusion |
|---|---:|---:|---|---|---:|---:|---|
| 2 | ~188 | ~411 | ~540,607 | ~342,342 | ~67 | ~223 | frontal, raised hands hold drape, shoulder outlines partially visible |
| 16 | ~188 | ~382 | ~537,603 | ~315,313 | ~66 | ~194 | asymmetric arm elevation, jacket and hair interrupt shoulders |
| machine_girl | ~188 | ~399 | ~536,615 | ~315,315 | ~79 | ~211 | frontal, headset and overlapping hair; shoulders partly visible |
| 110 | ~198 | ~440 | ~534,620 | ~367,352 | ~86 | ~242 | forward leaning, head tilt and central hair lock; do not treat as neutral standing pose |
| idol | ~205 | ~493 | ~510,599 | ~406,404 | ~89 | ~288 | frontal, hat sets top registration, hair and costume ruffles interrupt shoulders |

- Seven valid visible crown-to-chin spans across both batches: 194, 206, 211, 223, 242, 243, 288 px (median 223, range 194–288, 29.6–43.9% of 656 px registered height). Five batch-2 horizontal iris separations ~66–89 px. All 11 inspected samples span ~66–89 px horizontally. These are approximate illustrative ranges, not target tolerances.
- Approximate crown-to-chin / visible iris separation depends on hair volume, camera framing, head tilt and expression. Do not use the observed span to enforce a single scale for the entire character.
- Observable outer shoulder/upper-arm contour only (NOT shoulder joint width): 2 at y~480: x~444–706, ~262 px; idol at y~570: x~390–715, ~325 px; machine_girl has shoulder contour interruptions from hair, so omit. These two measurements are at different pose-dependent heights and cannot establish a comparable anatomical ratio. For 16, asymmetry prevents meaningful same-row shoulder comparison; 110 leans forward, so neutral standing comparison is invalid. No shoulder/torso population statistics claimed.
- 110 provides a clear counterexample to exclusively upright dialogue composition: face remains eye-level readable but head and shoulders lean/tilt, and hands terminate at bottom. Allow restrained conversational posture variation rather than forcing mirrored frontal symmetry.
- machine_girl is a material-specific sci-fi example: pale cool hair, dark technical collar, cyan circuitry, bright compact emissive shapes; preserve localized colored light and discrete circuit patterns rather than applying a uniform neon wash.
- 2 features irregular dark hair locks, dense textile glitter and pale layered drape. 16 uses very sharp irregular reflection streaks on dark sleeves. Idol uses clean hat/ruffle silhouettes, repeated dot patterns and distinct bow stripes. 110 shows softer low-contrast transitions on hair and clothing with defined fold edges. All reinforce material-dependent variation.

### Local pixel measurements (31 additional ROIs)
Same method as batch 1, [x0,y0,x1,y1) exclusive, alpha>=250, RGB4 bins, adjacent-pair d=max channel difference. Every rectangle has 100% qualifying coverage. Labels describe intended sampling, not verified full material segmentation. In particular, 16's initial hair-plain rectangle intersects a face/hair boundary (dominant bin fdd), and 2's nominal plain textile contains dense reflections; exclude these from clean plain-vs-boundary conclusions. Keep raw data for audit.
| Sprite | Region | ROI | n | Bins | Top bin (%) | d50 | d90 | edge16% |
|---|---|---|---:|---:|---|---:|---:|---:|
| 2 | hair local | 645,274,657,300 | 312 | 17 | 225 (32.7) | 5 | 26 | 17.2 |
| 2 | hair glint | 550,249,578,273 | 672 | 40 | 547 (34.8) | 5 | 38 | 27.2 |
| 2 | cheek | 543,365,557,376 | 154 | 1 | fba (100) | 1 | 1 | 0 |
| 2 | blue glitter textile A | 524,700,544,719 | 380 | 55 | 138 (19.7) | 10 | 56 | 41.7 |
| 2 | blue glitter textile B | 512,743,543,763 | 620 | 69 | 138 (21) | 8 | 55 | 34.2 |
| 2 | white drape | 340,736,362,762 | 572 | 22 | cce (19.1) | 4 | 27 | 17.5 |
| 2 | neck jewelry | 556,437,586,458 | 630 | 210 | faa (12.9) | 26 | 84 | 61.2 |
| 16 | hair/face mixed boundary (not plain hair) | 586,251,601,271 | 300 | 56 | fdd (43) | 5 | 65 | 33.6 |
| 16 | hair glint | 526,231,556,252 | 630 | 15 | fff (31.7) | 3 | 22 | 16.9 |
| 16 | cheek | 572,335,586,345 | 140 | 1 | fdd (100) | 1 | 1 | 0 |
| 16 | sleeve local | 783,389,799,401 | 192 | 29 | 335 (31.3) | 4 | 35 | 23.3 |
| 16 | sleeve reflection | 751,352,781,380 | 840 | 52 | fff (16.1) | 11 | 59 | 43.7 |
| 16 | collar hardware | 551,393,585,404 | 374 | 64 | 112 (13.4) | 22 | 126 | 58.5 |
| machine_girl | hair plain | 550,252,561,275 | 253 | 6 | 89c (90.5) | 1 | 6 | 4.7 |
| machine_girl | hair glint | 543,226,574,247 | 651 | 20 | 89c (35.5) | 4 | 24 | 19.8 |
| machine_girl | cheek | 548,343,565,351 | 136 | 3 | c9a (58.8) | 1 | 3 | 0 |
| machine_girl | collar local | 609,418,621,432 | 168 | 36 | 348 (45.8) | 5 | 55 | 25.2 |
| machine_girl | cyan circuit boundary | 551,420,596,453 | 1485 | 78 | 113 (8.8) | 21 | 122 | 55.9 |
| machine_girl | headset | 468,319,480,343 | 288 | 98 | fff (6.3) | 36 | 120 | 71.9 |
| 110 | hair plain | 547,270,560,288 | 234 | 3 | caa (66.2) | 1 | 1 | 0 |
| 110 | hair glint | 535,235,555,258 | 460 | 11 | dcc (42.4) | 2 | 15 | 9.5 |
| 110 | cheek | 550,402,566,414 | 192 | 1 | eaa (100) | 1 | 1 | 0 |
| 110 | light fabric | 568,744,582,764 | 280 | 10 | 767 (50.4) | 1 | 12 | 8.7 |
| 110 | fabric fold | 548,704,581,725 | 693 | 22 | a89 (54.5) | 3 | 26 | 20 |
| idol | hair plain | 454,300,470,325 | 400 | 8 | 868 (68) | 2 | 10 | 3.2 |
| idol | hair glint | 514,269,549,292 | 805 | 36 | 868 (24.5) | 6 | 39 | 26.1 |
| idol | cheek | 527,432,543,444 | 192 | 1 | fdc (100) | 1 | 1 | 0 |
| idol | bow local | 506,535,523,546 | 187 | 18 | 48e (55.1) | 2 | 62 | 18.8 |
| idol | bow seam | 510,552,542,567 | 480 | 80 | 48e (45) | 5 | 77 | 35.3 |
| idol | hat band | 590,234,620,249 | 450 | 52 | 336 (37.1) | 4 | 71 | 29.6 |
| idol | hat patterned cloth | 594,209,620,224 | 390 | 16 | aac (44.1) | 3 | 14 | 7.6 |

Paired-region evidence (descriptive only, not random samples): hair edge16% local to highlight increases for 2 (17.2 to 27.2), machine_girl (4.7 to 19.8), 110 (0 to 9.5), idol (3.2 to 26.1). Dark sleeve local to reflection for 16: 23.3 to 43.7. 110 fabric local to fold: 8.7 to 20.0. Idol bow local to seam: 18.8 to 35.3. Five deliberately selected quiet cheek ROIs all have d50=1 and edge16=0. These are not whole-face statistics. The two glitter patches in 2 have high local edge rates (34.2–41.7%), showing that a decorated textile may intentionally violate the flat plain-cloth rule. Emissive circuitry and headset in machine_girl have very high edge rates (55.9,71.9%); localized jewelry/hardware in 2/16 likewise (61.2,58.5%). Do not globally imitate those texture densities.

### V4 incremental refinement (batch 2)
- Keep the fixed registration and adult character identity, but treat head scale, hair volume, headwear and pose as separate controls. Use approximate semantic measurements only for diagnostic comparison, not universal mandatory targets.
- Retain readable conversational face and controlled pose; small forward lean, head tilt and asymmetrical hand positions can be appropriate.
- Distinguish low-frequency base color/shadow construction from high-frequency decoration. Plain interior surfaces should not be covered by noise or randomly scattered highlights; hair glints are clustered on individual locks, reflective garment strokes follow fold curvature, gems and metallic edges form compact sharp shapes, glitter is restricted to the textile that calls for it.
- For technical clothing, localize saturated emissive lines and small bright cores to circuits, devices and seams; use darker surrounding material for contrast. Do not turn the whole sprite into neon lighting.
- Preserve character-dependent makeup, expression, iris color and hair palette. Avoid copying specific reference identities, outfits or accessories into unrelated characters.
- Full V4 prompt still pending further eligible samples and validation.

### Resume point after batch 2
- Completed stage 2 geometry/global color: 30/30, unchanged; do not rerun.
- Semantic/material stage: 13/30 manifest entries screened; 11 analyzed (6 previous + 5 new), 2 deferred (10,100), 17 not yet screened. Total 52 local ROIs (21+31), including flagged mixed boundaries. Seven approximate visible crown/chin pairs. Comparable anatomical shoulder/torso ratios still not established.
- Remaining unscreened All Girls: 20,24,101,104,120,125,143,150,164,175,183 (11). More Girls: elf,dark_elf,angel,ghost,jester,ballet (6).
- Next: screen remaining eligible adult non-explicit NPC references; prioritize visibly different outfits and clearer shoulder contours, small inspection batches, record evidence not raw images. Then consolidate findings and finalize prompt before generation testing. No original mods changed, no test generation completed.


## 2026-11-24 KST — semantic/material verification, batch 3 and V4 consolidation

All 17 previously unscreened manifest entries fetched and screened. Four directly inspected at source resolution for nonsexual face, hair and head-accessory rendering; thirteen screened on two 1:4 overview sheets only. No source PNGs or sheets saved. Adult-reference scope retained; age is not inferred from anatomy. Revealing costume/body regions are not used to derive anatomical prescriptions.

### Source provenance and screening
| Sprite | Blob SHA | Scope / overview observation |
|---|---|---|
| 20 | 0de3d2edb0a1e00226e31c56bd3a8b7c41b3e75d | Detailed face/hair |
| 24 | 47c753f14b7d2b56fd75f220622735975e7d9b57 | Detailed face/hair/headgear |
| elf | ff370e8110d28f47301078c67074d5e3ccd1e0f8 | Detailed face/hair/flower |
| jester | 441a9acab0c7a2735c1f15859af706e432b46203 | Detailed face/hair/hat/ruffle |
| 101 | 5431dd3f03a9872d0af5f5699e4f870174bcb59d | Overview: flower, warm curls |
| 104 | 044540d6df1e22d2196bf63a63bdc4690c5a9390 | Overview: coral hair, obscured eye |
| 120 | 479fb083ee73c65f9816379c922ed5eacfd5861a | Overview only; eligibility not established |
| 125 | 881d0109084850d8f223386539db93bf0e0ba68e | Overview: teacup masks lower face |
| dark_elf | 829516a503000d0a5cb1e2b39788ce8ab907faa7 | Overview: dark palette, red eyes, pointed ears |
| angel | 306ed547d83643c639e336f7f1f1570f9a849bbb | Overview: halo and wings |
| 143 | bb5cec2965c438cf7bd6d0afeb8c6ce5a7a5a321 | Overview: golden curls, blue/gold trim |
| 150 | dae6190e082bc2a026b8ecaf59fead0d5f6f817e | Overview: dark bob, purple palette |
| 164 | 3a7f368cc5ae03c704c23a5058cc6f20af0b3f86 | Overview: seated pose and wheelchair |
| 175 | 7df86373c748eb6b65cb3bfba749d3a8a00ae2eb | Overview only; eligibility not established |
| 183 | 611aa464087df669932faaab2e6efd056975e887 | Overview: flowing pale hair and red ornaments |
| ghost | 399ebf6ba7369ba9b377dfc07d6a96bbb928519c | Overview: supernatural palette/wisps; not baseline anatomy |
| ballet | 9323009c1d71ea0b56b0fde94ff96315db8d67f8 | Overview only; eligibility not established |

### Approximate landmarks (face/hair only, source coordinates)
Same definitions as preceding batches. H=visible main hair crown to chin, NOT skull height. Approximate to ~5–10 px.
| Sprite | Crown Y | Chin Y | Iris X | Iris Y | Horizontal separation | H |
|---|---:|---:|---|---|---:|---:|
| 20 | 188 | 380 | 527,588 | 320,321 | 61 | 192 |
| 24 | 194 | 407 | 515,NA | 337,NA | NA | 213 |
| elf | 188 | 418 | 539,617 | 343,343 | 78 | 230 |
| jester | NA (hat) | 493 | 529,621 | 412,413 | 92 | NA |

Ten valid approximate visible crown/chin spans across detailed batches: 192,194,206,211,213,223,230,242,243,288 px (median 218, range 192–288). Eye separation observed range extends to ~61–92 px. Not anatomical targets; hair volume, headwear, crop, expression and pose affect results. No comparable anatomical shoulder/torso ratios established.
- 20: warm brown broad flat hair masses, abrupt curved shadows and clustered warm irregular highlights; fine cheek hatching, expression-specific iris symbols.
- 24: pale cool hair, fine colored contours, narrow bright streaks, one eye partly obscured by fringe; angular dark headgear and saturated stripes. Equal eye visibility not required.
- elf: layered pale locks, colored gray-purple contours, sharply tapered upper lash shape and cool layered irises; flower/jewels form compact high-frequency accents.
- jester: contrasting two-tone hair, broad curved red/purple hat folds, repeated motifs, pale layered ruffles, differently colored irises. Hat dimensions are not head dimensions.
- Overview screening supports variation in silhouette due to hair, wings, props and seated equipment. No precise measurements inferred from overview sheets.

### Local pixel measurements (17 additional ROIs)
Prior verified PNG decoder, alpha>=250, RGB4 bins and adjacent-pair max RGB channel delta. Bounds [x0,y0,x1,y1). All coverage 100% except elf flower boundary (96%). Mixed patches explicitly retained and excluded from clean comparisons.
| Sprite | Region | ROI | n | Bins | Top bin (%) | d50 | d90 | edge16% |
|---|---|---|---:|---:|---|---:|---:|---:|
| 20 | hair local | 538,211,554,233 | 352 | 1 | a77 (100) | 0 | 1 | 0 |
| 20 | hair glint | 536,251,558,270 | 418 | 29 | a77 (32.3) | 6 | 30 | 26.7 |
| 20 | cheek | 542,337,552,346 | 90 | 2 | fed (97.8) | 0 | 1 | 0 |
| 24 | hair local | 552,263,565,285 | 286 | 13 | eef (47.6) | 1 | 3 | 4.3 |
| 24 | hair glint | 535,221,559,244 | 552 | 9 | eef (74.8) | 1 | 13 | 7.8 |
| 24 | cheek | 530,356,540,365 | 90 | 2 | fee (57.8) | 1 | 1 | 0 |
| 24 | headgear stripe | 423,220,432,247 | 243 | 28 | 545 (37.9) | 4 | 73 | 25.8 |
| elf | hair local | 563,248,579,270 | 352 | 5 | dde (90.1) | 1 | 2 | 0.6 |
| elf | hair glint | 549,219,573,243 | 576 | 18 | fff (29) | 4 | 28 | 20.2 |
| elf | cheek | 544,375,556,385 | 120 | 1 | fdc (100) | 1 | 1 | 0 |
| elf | flower boundary | 666,251,686,275 | 461 | 156 | bbd (10.8) | 21 | 103 | 58.7 |
| jester | hair local (mixed edge) | 544,335,554,355 | 200 | 42 | fd4 (28) | 10 | 83 | 38.4 |
| jester | hair glint | 542,298,560,324 | 468 | 28 | fc3 (44.7) | 3 | 13 | 8.7 |
| jester | cheek | 548,440,560,450 | 120 | 1 | fdc (100) | 1 | 1 | 0 |
| jester | hat local (mixed folds) | 364,296,380,312 | 256 | 36 | 64a (33.2) | 2 | 53 | 21.9 |
| jester | hat motif | 383,224,411,251 | 756 | 57 | fe6 (50.5) | 2 | 36 | 18.2 |
| jester | ruffle edge | 555,514,581,538 | 624 | 87 | fff (22.3) | 11 | 87 | 43.6 |

Three new local-to-glint hair pairs increase edge16 (20: 0→26.7; 24: 4.3→7.8; elf: 0.6→20.2). Jester nominal local hair patch includes strong edges, so its reverse result is not a clean pair. Four quiet cheek patches all edge16=0, d90=1. Headgear stripe, flower boundary and ruffle edge have much stronger transitions. Total 69 manually selected ROIs; purposive samples, not population statistics.

## V4 consolidated production prompt — registration verified, style evidence-based, generation validation pending

### Shared style (fixed)
Create a clearly adult Japanese 2D anime visual-novel NPC dialogue sprite for a consistent game character collection, not a promotional illustration or scene. Preserve the individual's recognizable adult physique and identity. Use compact stylized proportions, short natural neck, simplified smooth contours and readable body regions. Avoid fashion-illustration elongation, photorealistic anatomy, exaggerated perspective or fixed universal head/body and shoulder ratios.

Use an eye-level, mostly frontal conversational composition with a readable face. Allow natural small head tilts, slight forward lean, asymmetric gestures, held items, seated poses and character-specific posture. Keep the pose legible rather than cinematic. Preserve headwear, hair, props and accessibility equipment as part of the silhouette; never mistake their dimensions for anatomy. Natural waist/hip/upper-thigh crop depends on identity and pose.

Use fine colored line art responsive to local hue and value, selective dark contours at overlaps, and especially strong upper eyelash silhouettes. Construct eyes with dark wine or purple-black upper lashes, sharply tapered outer tips, layered irises with dark upper values and brighter lower colors, compact discrete reflections. Preserve individual eye shape, brow angle, eye visibility, color, expression and makeup. Use compact nose and mouth and fine diagonal cheek hatching where appropriate. Do not force identical eyes or perfectly mirrored features, or copy another character's iris symbols.

Construct hair from overlapping broad coherent locks, layered tapered tips, sparse thin inner strand lines and compact irregular highlight clusters following lock curvature. Use broad stable base-color areas and deliberate shadow masses; place thin bright streaks selectively, rather than covering every lock in noise. Hair volume is independent of face scale.

Build shading from coherent flat or gently transitioning color masses, clear curved cel-like primary shadows and restrained contact shadows at overlaps. Keep quiet interiors clean; use local smooth transitions only where material or form benefits. Avoid global airbrush gradients, random noise, photographic skin textures, excessive contour details and uniform shine.

Distinct material treatments: matte cloth with broad simple fold masses; polished fabric and armor with controlled irregular reflection streaks or separated bands; metal/jewelry with sharp compact edge highlights; decorated textile with confined repeating motifs, embroidery or glitter where appropriate; technical hardware with small localized saturated emission and dark surrounding contrast. Lace/translucency character-specific, not a universal opacity rule. Do not spread accessory-level edge density across plain surfaces.

### Character-specific module (replace per character)
Adult character identity: [name / explicit adult version].
Recognizable facial features, hair silhouette and palette: [details].
Individual adult build and outfit silhouette: [details, no universal body ratio].
Costume layers, materials, colors and identity accessories: [details].
Personality, expression and conversational pose: [details].
Use identity references for recognizable features, hairstyle, costume, colors, accessories and personality, not as a substitute for the shared rendering system. Reconstruct composition, contour, eyes, shading and material treatment in this NPC sprite visual language. Do not import unrelated reference-character accessories or motifs.

### Output registration (fixed, measured)
Exact 1152 x 844 RGBA landscape canvas; transparent background, one character. Entire nonzero-alpha silhouette occupies rows 188–843 inclusive, exactly 656 px tall; rows 0–187 transparent. Center complete silhouette horizontally within 0.5 px of canvas center 575.5, not face. Preserve aspect ratio and individual width. Observed width range 448–800 px; IQR 559.5–654 px, descriptive only. Character-specific anatomical crop. No scene, frame, text, logo, panel, extra figure or opaque background. Clean antialiasing. If generator cannot guarantee exact registration, proportionally scale alpha bbox and place on transparent canvas; verify bounds afterward. Do not alter interior opacity solely to match alpha statistics.

### Validation rubric
1. Hard registration: canvas, alpha bounds, center verified numerically; pass/fail.
2. Identity: recognizable adult identity, hair, outfit, expression, accessories, no feature leakage from other reference characters.
3. Dialogue composition: readable face, coherent conversational pose, no cinematic perspective, identity-specific crop.
4. Face: upper lash shape, layered iris/reflections, compact features, blush and colored contours.
5. Hair/shading: large lock structure, quiet base areas, clustered highlights, controlled transitions, no global texture noise.
6. Materials: compare visually justified matched quiet patches vs glints/folds/trim, not universal ROI thresholds.
7. Do not treat global alpha width or visible hair height as anatomy. Comparable anatomical shoulder/torso widths remain unestablished; full semantic segmentation not completed. Further representative, eligible and suitably clothed references are needed for defensible ratios. Production prompt leaves those dimensions character-dependent.

### Resume point after batch 3
- Geometry/global palette complete 30/30, unchanged; do not repeat.
- Manifest screening 30/30: all 17 previously unscreened entries now screened.
- Detailed semantic/material 15/30 total (11 earlier + 4 face/hair/head-accessory); 69 local ROIs; 10 approximate visible crown/chin pairs. 13 overview-only this batch, including three deferred from detailed use, plus two earlier deferred. Screening is not detailed measurement.
- V4 consolidated prompt drafted above. Registration measured; style evidence-based, not a claim of full anatomy/segmentation or validated generation fidelity.
- Next: generation comparison on distinct clearly adult character identities in separate test outputs; record actual rubric pass/fail and revise only for observed failures. No test images generated in this batch. No original mod or localization files modified.
