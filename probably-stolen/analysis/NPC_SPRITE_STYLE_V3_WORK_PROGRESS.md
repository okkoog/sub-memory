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

Interpretation: 17/21 ROIs have d50 <=3, consistent with broad locally stable areas and discrete edges. All Girls 1 purple collar (93.9% dominant bin; d90=1) contrasts with metal flute (17.1% dominant bin; d90=76) and royal gold trim (d90=40). Knight's hair ROI samples high-contrast highlight edges, while 1's hair ROI is comparatively flat. The hood ROI is explicitly a decorated boundary; it is NOT evidence that matte green cloth itself has the highest edge contrast. The cheek ROIs mix flat skin, blush, lash/hair or neighboring contours depending on framing; not directly comparable as pure skin regions. Sample selection is manual, not randomized. These data support region-dependent rendering and restrained local gradients, not a universal number of shades or an All Girls vs More Girls statistical conclusion.

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
