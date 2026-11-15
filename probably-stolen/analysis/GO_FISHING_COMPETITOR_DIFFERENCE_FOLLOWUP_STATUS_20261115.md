# Go Fishing 2.5.2 — 경쟁 번역 기존 기록 재점검 (2026-11-15)

## 자료와 검토 범위
- 우리 CSV: `probably-stolen/번역 모드/Localization/pskb__3829110153.csv`, 3,448개 키.
- 과거 비교 원본: 사용자가 제공했던 **Probably Translated v1.0.19**의 `workshop-overlays/3829110153/Localization/ko.csv` (당시 보고 3,426키). **이번 세션에는 원본 경쟁 번역 CSV가 남아 있지 않아 3,426키를 새로 전량 대조하지 못했다.**
- 대신 GitHub에 보관된 `probably-stolen/analysis/GO_FISHING_REFERENCE_ONLY_COMPARISON_FIXES_20261115.csv`에서 **14개 교정 사례**를 현재 중국어/영어/한국어 소스와 직접 다시 비교했다. 이 범위 밖의 경쟁 번역과의 차이는 검증했다고 주장하지 않는다.

## 이번 실제 변경
- `chemical.manual.page.12.section.1.title`: `원액 10 mL당 안정제 투입량` → `원액 10 mL당 조합별 투입량`.
- 중국어 원문 `每 10 mL 原液的单组用量`, 영어 `One pair per 10 mL of source solution`. 단일 보조재 조합에 대한 사용량이므로 안정제 전용 투입량으로 오역된 회귀를 복구했다.
- 나머지 13건은 현재 개선된 문구/사용자 확정 용어 유지(물 및 병입, '{0} 과다' 등), 변경하지 않았다.

## 검증
- 실제 한국어 K 키 3,448개 유지, 수정은 위 1개 키뿐, 그 외 기존 번역값/메타데이터 변경 0.
- CSV 기록 14개 행의 비교 근거·현재 판정은 `probably-stolen/analysis/GO_FISHING_COMPETITOR_DIFFERENCE_FOLLOWUP_20261115.csv`에 보관.
- **아직 달성하지 못한 작업:** 경쟁 번역 CSV 전체 3,426개 키와의 차이 추출. 원본 CSV를 확보한 이후에만 전수 비교 수치를 산출할 수 있음.
- 원본 모드, Steam 배포, DLL 미수정.
