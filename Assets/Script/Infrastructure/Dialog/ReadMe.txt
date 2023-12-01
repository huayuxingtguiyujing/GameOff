Excel对话文件格式说明：
注意：需要严格按顺序输入：

---------------Type栏：不同的符号代表不同的对话类型-------—
1.#是一般对话；
2.&是会需要选择的对话；
3.*是对话分类标识，标识了接下来到结束的一段对话是负载于某个事件节点的
4.%(ArtType)表示该段对话是BGM、画廊控制、镜头控制、UI控制、地图控制、触发商店等改变游戏界面的效果
5.$表示该段对话会触发战斗（content有效、CharacterName、NPCName均无效，触发战斗会直接结束对话）

---------------Id：本对话的Id------------------------------
1.如果本条信息为*，则该Id表示要加载到的EventPlace的节点ID，这个节点ID在在同一场景中是唯一的，需要与Unity中内容对应（有点吃力，之后可能会改）
2.否则，是标识该行对话的Id

---------------Jump：要跳转的对话Id-------------------------
1.0是保留Id，值为0时不跳转；-1是结束Id，对话到此结束
2.如果本条信息为*（对话分类标识），则是该行对话的优先级，优先级越高越先触发

---------------CharacterName：左方人物名称-------------------

---------------NPCName：右方人物名称-------------------------

---------------Content：对话内容-----------------------------

一.Common
1.对话内容中的一些特殊符号会产生特殊功能，例：#Pause@1s 表示暂停1s后再播放后续内容
2.英文逗号和中文逗号必须严格区分！

二.选项类型
1.当本条信息为&时（需要选择的对话），Content为选项内容

2.例子
是的!6/不是!7 ：会出现两个选项，分别是“是的”与“不是”，“!”后方的数字代表选择后跳转的Id
打招呼!6/离开!7 ：同上

三.美术类型
1.当本条信息为%时（美术效果），Content为目的美术效果

2.格式说明
BGM@BGMName:n：播放指定音效，后方数字是音量
MainTheme@MainThemeName:n：播放指定主题曲,后方数字是音量
MainTheme@MainThemeName:0：停止播放主题曲
Gallery@photoName：在HUD展示指定图片
Gallery@None：停止展现图片
Environment@targetEnvironment：环境效果
<--5.28日更新：-->
Map@StartLoad_NorthArea:1：使地图显示指定区域
Map@StartLoad_NorthArea:0：使地图隐藏指定区域
Camera@Blood：使摄像机显示指定的滤镜		//TODO
<--6.17日更新-->
EventPlace@eventPlaceId:0				//级别0为正常,级别1为

3.例子
Environment@Rain:1：环境效果，开始下雨，级别1~n代表雨的程度
Environment@Rain:0：环境效果，停止下雨
BGM@BGMName:n/MainTheme@MainThemeName:n/StopGallery：组合效果

---------------Effect: 对话造成的效果------------------------
1.各种格式说明
Statu@HP:1 ：即生命值回复一点；Statu@HP:-1 即生命值减少一点
Statu@San:1 ：即San值回复一点；Statu@San:-1 即San值减少一点
Statu@Pre:1 ：即压力值增加一点；Statu@Pre:-1 即压力值减少一点
EventBool@eventname:1 ：修改eventname的变量为指定值（0 or 其他）
EventInt@eventname:1 ： 修改eventname的变量值 加1 或 减1
Item@itemname:1 ：向背包中添加物品*1
Item@itemname:-1 ：向背包中减少物品*1
AddCharacter@charactertag ：向玩家队伍中添加某个角色（如果队伍中没有该角色）
LoseCharacter@charactertag ：向玩家队伍中删去某个角色

2.例子
例：Statu@HP:1/Statu@San:1/Statu@Pre:1 ：为队伍所有角色增加一些状态

---------------Option：选项出现条件------------------------------
现在option已经成为保留字段

---------------Condition：触发条件----------------------

一.对话分类标识使用的Condition
1.各种格式说明
Local@1：事件节点的本地变量1为true时（本地存有5个事件变量）
EventBool@eventname：存档中的全局事件变量为true时
EventBool@!eventname：存档中的全局事件变量为false时
EventInt@eventname>num：存档中的全局事件变量大于num时
EventInt@eventname<num：存档中的全局事件变量小于num时
ItemBool@itemname：背包中是否有某物品
ItemInt@itemname>num：背包中某物品数量大于num时
Statu@statuname>num：玩家某状态大于某值时
Team@charactertag：玩家队伍中存在某角色时
Team@!charactertag：玩家队伍中不存在某角色时

2.例子
例：EventBool@isTrigger&EventBool@isLoad：当这两个bool变量满足与条件时触发
例：EventBool@isTrigger|EventBool@isLoad：当这两个bool变量满足或条件时触发
例：Statu@HP>90：玩家生命值大于90时触发
例：Team@ZLF：玩家队伍中存在飞哥时触发
例：Team@!ZLF：玩家队伍中不存在飞哥时触发

二.一般文本类型使用的Condition
不可以使用Local类型控制变量
同上，但是当文本条件不满足时，会立刻跳转到本段文本的Jump值指示的文本

三.选项使用的Condition
1.需要先将Type栏设置为&才会起效
2.Option中用/分割的内容必须与Content中的内容一一对应，分别表示Content中的选项的可选条件
3.当Option为空时，表示所有选项不需要触发条件
4.例：content内容为是的!6/不是!7 ，condition为EventBool@isTrigger/EventBool@notTrigger，意为当isTrigger为true时，可以选第一个选项，当notTrigger为true时，可以选第二个选项

四.其他类型，暂不支持使用Condition触发
