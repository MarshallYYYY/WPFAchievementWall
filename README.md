# 关于

* 软件名称：个人成就记录墙
* 作者：MarshallYYYY
  * GitHub 个人主页：https://github.com/MarshallYYYY
  * bilibili 个人主页：https://space.bilibili.com/180399015?spm_id_from=333.1007.0.0
* 联系方式：marshallyyyy@foxmail.com

# 项目技术架构

* 客户端：WPF

* 服务器端：ASP.NET Core WebAPI

## 版本信息

- .NET 版本：.NET 8
- Prism: Prism.DryIoc 9.0.537
- EFCore: Microsoft.EntityFrameworkCore.SqlServer 9.0.10
- MDIX：MaterialDesignThemes 4.9.0
- LiveChars2: LiveChartsCore.SkiaSharpView.WPF 2.0.0-rc2
- Newtonsoft.Json 13.0.4

## Prism

* 数据和命令绑定 DelegateCommand
* 区域管理 RegionManager
* 导航 Navigate
* 会话 Dialog
* 事件聚合器 EventAggregator PubSubEvent

## Material Design

`xmlns:md="http://materialdesigninxaml.net/winfx/xaml/themes"`

* md:DialogHost
  * md:DrawerHost
    * md:DrawerHost.LeftDrawerContent
    * md:ColorZone
    * `md:Snackbar MessageQueue="{Binding Path=MsgQueue}"`
  * md:DialogHost.DialogContent
  * md:Transitioner
    * `md:TransitionerSlide OpeningEffect="{md:TransitionEffect Kind=SlideInFromBottom}"`
* TabControl
  * `md:NavigationRailAssist.ShowSelectionBackground="True"`
* TextBox
  * `md:TextFieldAssist.HasClearButton="True"`
  * `md:HintAssist.Hint="XXX"`
* PasswordBox
  * `md:PasswordBoxAssist.Password="{Binding Path=XXXPassword, UpdateSourceTrigger=PropertyChanged}"`
    * 4.5.0（不含）以上（也就是 ≥ 4.6.0）才支持
* Button
  * `md:PackIcon Kind="XXX"`

## Entity Framework Core

```
Add-Migration AchievementWall
Update-Database
```

