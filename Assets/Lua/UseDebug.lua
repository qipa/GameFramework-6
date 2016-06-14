ZBS = "C:\Users\huanqing.she\Desktop\ZeroBraneStudio"
LuaPath = "E:\godfight\GameFramework\Assets\Lua"

package.path = package.path..";./?.lua;"..ZBS.."lualibs/?/?.lua;"..ZBS.."lualibs/?.lua;"..LuaPath.."?.lua;"  
require("mobdebug").start() 