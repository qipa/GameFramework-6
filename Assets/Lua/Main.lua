require "Define"
require "CtrlManager"


--主入口函数。从这里开始lua逻辑
function Main()					
	 print("hello sb")

    -- ResisterNetEvent(1,"Main","Main")
    -- TestSend()

    CtrlManager.InitCtrlList()
    CtrlManager.InitViewPanels()

    
    local ctrl = CtrlManager.GetCtrl(CtrlNames.Test)
    if ctrl ~= nil then
        ctrl.Awake()
    end
    
end

--场景切换通知
function OnLevelWasLoaded(level)
	Time.timeSinceLevelLoad = 0   
end

--供测试发送网络数据
function TestSend()
    local buffer = ByteBuffer.New()
    buffer:WriteShort(1)
    SendMessage(buffer)
end

-- 注册lua网络回调事件的总接口，处理服务器下发的消息
function ResisterNetEvent(id,Module,func)
    NetMgr:BindLuaCallback(id,Module,func)
end

--  发送网络消息的总接口   参数ByteBuffer
function SendMessage(args)
    NetMgr:SendMessage(args)
end