铁血清凉化改造计划
==================

纯立绘替换模组，覆盖铁血人形的原版立绘。
不改玩法、不改数值、不改存档。

安装
----
把整个文件夹放进 Mods 目录，游戏内启用即可。

替换方式
--------
图片放在 Art/ 目录（不是 Textures/Items）。文件名必须和原版立绘 key 一致，例如：

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

也可以覆盖皮肤立绘：

  npc_birch_skin1.png
  npc_birch_skin2.png
  npc_destroyer_skin1.png
  npc_agent_skin1.png

图片规格：任意尺寸 PNG，透明背景。贴图会按宽度自动换算 PPU，屏幕上的占位不变；
所以出 576x422、1152x844 还是 1728x1266 都行，数字越大越清晰。

Art/ 里的图是运行时按需读取的：只有真的要用到某张立绘时才解码，
内存里同时最多保留 6 张。这也是本 mod 需要 files 能力的原因。

当前版本
--------
1.0.0：框架与占位立绘。把新图按上面的文件名覆盖即可生效。
       立绘改为 Art/ 按需加载（不再走 Textures/Items 预热）。
