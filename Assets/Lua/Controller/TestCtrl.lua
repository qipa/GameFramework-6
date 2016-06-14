--region *.lua
--Date
--此文件由[BabeLua]插件自动生成

TestCtrl = {}
local this = TestCtrl
local luaBehaviour
local gameObject
local transform

function TestCtrl.New()
    Log.Info("TestCtrl.New")
    return this
end

function TestCtrl.Awake()
    UIMgr:CreatePanel("TestPanel",this.OnCreate)
end

function TestCtrl.OnCreate(obj)
    gameObject = obj
    
    luaBehaviour = gameObject:GetComponent("LuaBehaviour")

    if luaBehaviour == nil then
        Log.Error("界面不存在LuaBehaviour脚本，请检查")
        return
    end  

    this.BindEvents()
end

function TestCtrl.BindEvents()
    luaBehaviour:AddClick(TestPanel.btn,this.OnBtn)
end

local num = 0

function TestCtrl.OnBtn()
     num = num + 1
     this.SetText(""..num)
end

function TestCtrl.SetText(str)
    TestPanel.text:GetComponent("Text").text = str
end

--endregion
