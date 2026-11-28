# Anime costume illustration — style reference (2026-11-28)

## Provenance
The user approved the visual quality of the recent standalone anime fashion illustrations. This document records a reusable reconstruction of the visible style and the conversation's design requirements, not the exact internal image-generation prompt. The generation metadata supplied no original prompt text (empty prompt field). No deterministic reproduction is guaranteed.


## 실제 생성 요청에서 확인되는 표현 (원문 발췌)

아래는 대화에 남은 생성 요청의 영문 표현을 그대로 발췌한 기록이다. 새로 작성한 프롬프트가 아니다. 전체 요청이나 최종 모델 입력을 복원한 것으로 간주하지 않는다.

### 아르누보 요정
- "A polished, highly detailed anime-style full-body portrait"
- "ornate Art Nouveau floral-fairy couture ensemble"
- "soft pale gray-white gradient"
- "Gold filigree, emerald accents, lilies, vines, jeweled accessories"
- "crisp detail, glossy highlights, soft lighting, and a pale neutral backdrop"

### 하라주쿠 펑크
- "Produce a highly detailed, full-body anime illustration on a clean, pale gradient background, matching the sample’s sharp, glossy polish"
- "neon Harajuku punk/techno streetwear"
- "Emphasize crisp linework, saturated color, reflective patent textures, mesh, plaid weave, metal glints, and dense accessory layering in a clean studio-style composition with no other props or characters."

### 오리엔탈 구미호
- "A polished full-body anime-style illustration"
- "adult kitsune-inspired fashion character"
- "rendered with jewel-like shine, ornate embroidery, and a red, white, and gold palette"

### 최근 긍정 평가를 받은 천체·시계풍 의상
- "A highly detailed, polished anime-style full-body portrait"
- "futuristic-baroque “astral clockwork priestess” ensemble"
- "crisp linework, glossy highlights, and celestial time-themed detailing."

## 기록 구분
위 발췌문은 확인 가능한 실제 요청 표현이며, 아래 재구성 프롬프트와 구분한다. 최종 생성 메타데이터의 prompt 필드는 비어 있으므로 전체 최종 입력은 검증할 수 없다.

## Reusable style prompt (English, reconstructed; secondary reference)
```text
Create a highly detailed, polished full-body anime fashion illustration of one clearly adult woman (age 30), presenting an original couture costume based on [NEW COSTUME CONCEPT]. Use crisp linework, refined shading, controlled glossy highlights, rich material differentiation, intricate embroidery, delicate metalwork, jewels and carefully coordinated accessories. Give the garment a distinctive silhouette, coherent construction, and a consistent visual motif from headwear to footwear. Use a simple pale neutral studio background and show the entire outfit, including shoes, in one standalone image. Keep the costume the primary focus, with a confident natural pose and a consistent high-quality character rendering. No text, numbers, collage or extra characters.
```

## 작업 기준
- 의상 콘셉트를 매번 새롭게 선정. 특정 의상의 색상 변경이나 반복 변형에 고정하지 않는다.
- 의상의 재단, 실루엣, 소재, 장식 구조를 독립적으로 설계한다.
- 캐릭터 표현은 최근 만족한 결과물 수준으로 유지한다. 캐릭터 자체의 개선보다 의상 디자인에 집중한다.
- 세부 장식의 정교함과 소재의 광택, 색상 통일성을 유지한다.
- 한 이미지에 의상 하나, 여러 장 요청 시 최소 3장의 독립 이미지. 콜라주 제외.
- 새 콘셉트의 실제 반영 여부와 이전 결과물 대비 완성도를 확인한다.

## 승인된 스타일의 참고 사례
- 아르누보 요정: 식물 모티프, 백합, 에메랄드, 금속 세공.
- 하라주쿠 펑크: 형광 핑크와 라임, 체크 패턴, 지퍼, 금속 장식.
- 오리엔탈 구미호: 적백색 직물, 전통 매듭과 자수, 금장.
- 최근 생성 결과의 시계·천체·크리스털·바로크 장식 계열도 사용자가 품질을 긍정적으로 평가했다. 단, 사전 지정 콘셉트와 실제 결과의 불일치가 있었으므로 개별 의상 재현 성공으로 기록하지 않는다.

이 문서는 화풍 및 일반적인 패션 디자인 기준을 저장한 것이며, 생성 가능 범위를 보장하지 않는다.
