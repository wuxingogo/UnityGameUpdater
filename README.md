##这是GameUpdater的扩展程序

GameUpdater是Unity3D的一个非常好用的热更新插件.

他的思路是每次登陆游戏检查版本,如果和远程版本不同,则对比版本文件里需要更新的文件,然后逐个从远程服务器上更新bundle文件.

第一次Build Bundle到Resources路径下,然后每一次Build Bundle到AssetBundlePool路径下,Build的Bundle都是经过7Zip压缩和加密,所以Bundle文件会特别小,每次把版本信息记录在一个版本文件里.每次LoadBundle时解密解压缩.

##优势:

如果有10个版本的更新,则可以从当前版本直接更新到最新版本,而不必考虑各个版本的差异

##劣势:
如果版本更新文件较多则与远程服务器IO操作较多.

##BundleExtension

对此我在他的基础上写了BundleExtension,一样都是用7Zip压缩和加密.
但是每次Build的Bundle都是压缩到一个文件,每次从服务器更新时会把文件解包到临时文件夹下.

##优势

每个版本只下载一个文件,减少Client和Server的IO操作

##劣势

如果有10个版本更新,则必须从1版本到10版本逐个更新.对此最好有大版本更新,把最新的Bundle再次打入Resources下则即可.


##Note:

BundleExtension是基于我的插件WuxingogoExtension之上写的,如果觉着不需要WuxingogoExtension其他插件,则需要自己删除类,或者重构:)

WuxingogoExtension:

https://github.com/wuxingogo/WuxingogoExtension



##Note:

Unity5.x buildBundle会自动把依赖分好。prefab尤其重要，最好把模型(.fbx)和动画(.anim)都使用一个bundle。要不然两个相同的prefab会将这些都打入各自的bundle里，bundle巨大。如果打入公共bundle里则必须先加载公共bundle，否则资源丢失。还好Unity把依赖自己识别了，5.X很方便。


Contact: 52111314ly@sina.com