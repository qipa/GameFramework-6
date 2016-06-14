--region *.lua
--Date
--此文件由[BabeLua]插件自动生成

--全局的lua变量  ，因此这个文件加载后(即require或dofile之后) ，这个变量可以在任意lua文件中调用
TestPanel = {}

local transform;
local gameObject;

local this = TestPanel;


--这个函数会在LuaBehaviour中调用
function TestPanel.Awake(obj)
    gameObject = obj;
    transform = obj.transform;

    this.InitPanel()
end

--初始化工作，，比如如找到某个控件
function TestPanel.InitPanel()
   this.text = transform:FindChild("Text").gameObject;
   this.btn = transform:FindChild("Button").gameObject;

   
end

--endregion
