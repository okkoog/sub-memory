철혈 경량화 초상화 개조 프로젝트 (Iron Blood Sheer Project)
======================================================

철혈 인형의 기존 초상화를 교체하는 이미지 전용 모드입니다.
게임 방식과 수치, 저장 데이터는 변경하지 않습니다.

설치
----
모드 폴더 전체를 게임 Mods 디렉터리에 넣고 게임 내에서 활성화하세요.

이미지 교체
-----------
PNG 파일은 Textures/Items가 아니라 Art/ 폴더에 넣어야 합니다.
파일명은 원본 초상화 키와 정확히 같아야 합니다. 예:

  npc_agent.png
  npc_destroyer.png
  npc_birch.png
  npc_architect.png
  npc_scarecrow.png
  npc_executioner.png
  npc_metrology.png
  npc_judge.png
  npc_beak.png
  npc_ouroboros.png
  npc_hunter.png
  npc_intruder.png
  npc_alchemist.png
  npc_ironblood.png

스킨 초상화도 교체할 수 있습니다.

  npc_birch_skin1.png
  npc_birch_skin2.png
  npc_destroyer_skin1.png
  npc_agent_skin1.png

이미지 규격은 투명 배경의 PNG이며 크기 제한은 없습니다.
텍스처 너비에 맞춰 PPU가 자동 조정되므로 화면에서 차지하는 영역은 변하지 않습니다.
따라서 576×422, 1152×844, 1728×1266 모두 사용할 수 있으며
해상도가 높을수록 더 선명하게 표시됩니다.

Art/ 폴더의 이미지는 게임 실행 중 실제로 필요해질 때만 디코딩됩니다.
메모리에는 한 번에 최대 6장만 보관합니다.
이 기능 때문에 모드에 files 권한이 필요합니다.

현재 버전
---------
1.0.0: 기본 구조와 임시 초상화 추가.
       새 이미지를 위 파일명으로 덮어쓰면 교체됩니다.
       Art/ 폴더에서 필요할 때 이미지를 불러오도록 변경했습니다.
       기존 Textures/Items 사전 불러오기 방식은 사용하지 않습니다.
