using UnityEngine;
using System.Collections;
using LuaInterface;


    public class LuaManager : MonoBehaviour
    {
        public static LuaManager Instance
        {
            get { return GameManager.Instance.luaMgr; }
        }
        private LuaState lua;
        private LuaLoader loader;
        private LuaLooper loop = null;

        // Use this for initialization
        void Awake()
        {
            loader = new LuaLoader();
            lua = new LuaState();
            this.OpenLibs();
            lua.LuaSetTop(0);

            LuaBinder.Bind(lua);
            LuaCoroutine.Register(lua, this);
        }

        public void InitStart()
        {
            InitLuaPath();
            InitLuaBundle();
            this.lua.Start();    //启动LUAVM
            this.StartMain();
            this.StartLooper();
        }

        void StartLooper()
        {
            loop = gameObject.AddComponent<LuaLooper>();
            loop.luaState = lua;
        }

        void StartMain()
        {
            lua.DoFile("Main.lua");

            LuaFunction main = lua.GetFunction("Main");
            main.Call();
            main.Dispose();
            main = null;
        }

        /// <summary>
        /// 初始化加载第三方库
        /// </summary>
        void OpenLibs()
        {
            lua.OpenLibs(LuaDLL.luaopen_pb);
            lua.OpenLibs(LuaDLL.luaopen_lpeg);
            lua.OpenLibs(LuaDLL.luaopen_cjson);
            lua.OpenLibs(LuaDLL.luaopen_cjson_safe);
            lua.OpenLibs(LuaDLL.luaopen_bit);
            lua.OpenLibs(LuaDLL.luaopen_socket_core);
        }

        /// <summary>
        /// 初始化Lua代码加载路径
        /// </summary>
        void InitLuaPath()
        {
            if (GameDefine.UseBundle)
            {
                lua.AddSearchPath(Util.DataPath + "lua");    //这个是Assetbundle模式时的路径
            }
            else
            {
                //开发阶段在这个目录
                lua.AddSearchPath(Application.dataPath + "/Lua");
            }
        }

        /// <summary>
        /// 初始化LuaBundle
        /// </summary>
        void InitLuaBundle()
        {
            if (loader.beZip)
            {
                loader.AddBundle("lua/lua.unity3d");
                loader.AddBundle("lua/lua_cjson.unity3d");
                loader.AddBundle("lua/lua_controller.unity3d");
                loader.AddBundle("lua/lua_lpeg.unity3d");
                loader.AddBundle("lua/lua_misc.unity3d");
                loader.AddBundle("lua/lua_protobuf.unity3d");
                loader.AddBundle("lua/lua_socket.unity3d");
                loader.AddBundle("lua/lua_system.unity3d");
                loader.AddBundle("lua/lua_system_reflection.unity3d");
                loader.AddBundle("lua/lua_unityengine.unity3d");
                loader.AddBundle("lua/lua_view.unity3d");

            }
        }

        public object[] DoFile(string filename)
        {
            return lua.DoFile(filename);
        }

        // Update is called once per frame
        public object[] CallFunction(string funcName, params object[] args)
        {
            LuaFunction func = lua.GetFunction(funcName);
            if (func != null)
            {
                return func.Call(args);
            }
            return null;
        }

        public void LuaGC()
        {
            lua.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
        }

        public void Close()
        {
            loop.Destroy();
            loop = null;

            lua.Dispose();
            lua = null;
            loader = null;
        }
    }
