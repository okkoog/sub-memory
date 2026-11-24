# All Girls / More Girls — NPC sprite art system V3
Checkpoint: 2026-11-24 (KST)

## Scope and state
Goal: recreate a consistent game NPC dialogue-sprite art system for new adult characters, without uploading dozens of reference images into chat. Source repository is the sub-memory. Work on generic NPC dialogue sprites only; exclude circular portraits, event CG, props, icons and unrelated variants. Only clearly adult, non-explicit references are in scope. Do not modify original mods or localization files.

Prior session reported visually examining 30 sprites (11 initial + 19 additional). This is inherited analysis, not a new independent visual verification:
- All Girls (19): 1, 2, 10, 16, 20, 24, 56, 100, 101, 104, 110, 120, 125, 139, 143, 150, 164, 175, 183.png.
- More Girls (11): knight, knight_royal, green_cloak, elf, machine_girl, dark_elf, angel, ghost, jester, idol, ballet.png.
- Source directories: probably-stolen/mods/3838352006/Textures/Items and probably-stolen/mods/3838820208/Textures/Items.
- Current tree check: 201 and 189 PNG files respectively (390 combined; NOT 390 NPCs). Tree SHAs: 92f68105698728af4bcf74820471ccaf25d94b4b, 740160bc06c346fddf98cf67f2b8473473d41eb4.
- Current session attempted PNG byte access via GitHub fetch and fetch_blob; both failed on non-UTF-8 binary. Direct raw URL from runtime failed DNS, and web open failed DisabledError. No new pixel measurements obtained. Do not mark quantitative verification complete.

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

## Next step: quantitative verification (NOT DONE)
1. Obtain binary PNG data from repository in a runtime that can read it without displaying each image in chat. Verify source revision, actual file type, dimensions, alpha channel, and candidate scope. If access fails, report failure instead of inventing data.
2. Reuse the 30-name manifest. For each eligible adult sprite, calculate alpha bbox at thresholds alpha>=1, >=128 and >=250. Use inclusive bounds or clearly state exclusive bounds. Record canvas dimensions, bbox coordinates, margins, visible height/width, center offset, bottom contact, and alpha coverage. Separate headwear/accessory outliers. Use mod-wise medians, ranges and IQR; verify Y=188 hypothesis and bottom alignment. Do not assume all PNGs have alpha.
3. Measure semantic landmarks (head width/height, face width, shoulder width, torso width) only where reliably annotated from images. Alpha bbox is not equivalent to anatomical landmarks. Record pose, crop, occlusion and confidence. Do not infer anatomy from the bounding box alone.
4. Color: exclude transparent pixels; distinguish overall palette from manually marked skin/hair/fabric regions; compare color quantization, edge contrast, shade transitions and highlight frequency. Pixel counts alone do not establish artistic technique.
5. Revise V3 only after measured evidence; label measured, visually inferred, and provisional statements separately. Save compact TSV and markdown results in this analysis area, no copies of PNGs, no bulk image attachments. Update checkpoint after completed batches; resume from first incomplete stage.
6. Test prompt on distinct adult character identities and compare registration, facial technique, shading, silhouette and identity fidelity. Keep generated results separate from source measurements.
