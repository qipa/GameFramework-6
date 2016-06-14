--region *.lua
--Date
--此文件由[BabeLua]插件自动生成

require "Controller/TestCtrl"    --加载TestCtrl.lua

--所有界面控制器的名字集合
CtrlNames = 
{
    Test = "TestCtrl"
}

--所有界面的名字集合
PanelNames = 
{
    "TestPanel"
}



CtrlManager = {};
local this = CtrlManager;
local CtrlList = {};    --所有界面控制器的列表

--初始化所有界面控制器
function CtrlManager.InitCtrlList()
    Log.Info("CtrlManager.InitCtrlList")

    CtrlList[CtrlNames.Test] = TestCtrl.New()
end

--加载所有界面的 View lua文件
function CtrlManager.InitViewPanels()
    for i = 1,#PanelNames do
        require("View/"..PanelNames[i])
    end
end

function CtrlManager.AddCtrl(ctrlName,ctrlObj)
    CtrlList[ctrlName] = ctrlObj
end

function CtrlManager.GetCtrl(ctrlName)
    return CtrlList[ctrlName]
end

function CtrlManager.RemoveCtrl(ctrlName)
    CtrlList[ctrlName] = nil
end
--endregion
