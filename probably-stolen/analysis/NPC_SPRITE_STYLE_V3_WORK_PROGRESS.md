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
