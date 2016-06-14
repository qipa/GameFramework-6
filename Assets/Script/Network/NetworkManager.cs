using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System.IO;

public class LuaNetCallback
{
    public string module;
    public string func;
}

    public class NetworkManager : MonoBehaviour
    {
        private SocketClient socket;
        static Queue<KeyValuePair<int, ByteBuffer>> sEvents = new Queue<KeyValuePair<int, ByteBuffer>>();
        private Dictionary<int, Action<Stream>> m_CSharpCallback = new Dictionary<int, Action<Stream>>();
        private Dictionary<int, LuaNetCallback> m_LuaCallback = new Dictionary<int, LuaNetCallback>();       
 
        public static NetworkManager Instance
        {
            get { return GameManager.Instance.netMgr; }
        }
        SocketClient SocketClient
        {
            get
            {
                if (socket == null)
                    socket = new SocketClient();
                return socket;
            }
        }

        void Awake()
        {
            Init();
        }

        void Init()
        {
            SocketClient.OnRegister();
        }


        public static void AddEvent(int _event, ByteBuffer data)
        {
            sEvents.Enqueue(new KeyValuePair<int, ByteBuffer>(_event, data));
        }

        
        void Update()
        {
            if (sEvents.Count > 0)
            {
                while (sEvents.Count > 0)
                {
                    KeyValuePair<int, ByteBuffer> _event = sEvents.Dequeue();
                    int msgID = _event.Key;
                    ByteBuffer msg = _event.Value;

                    //处理C#中的网络回调
                    if(m_CSharpCallback.ContainsKey(msgID))
                    {
                        m_CSharpCallback[msgID](new MemoryStream(msg.ToBytes()));
                    }
                    //处理Lua中的网络回调
                    else if(m_LuaCallback.ContainsKey(msgID))
                    {
                        LuaNetCallback evt = m_LuaCallback[msgID];
                        //调用lua中的函数    形如： module.func(msg)   
                        Util.CallMethod(evt.module, evt.func, msg);
                    }
                    else
                    {
                        Log.Error("未处理的消息 ： " + msgID);
                    }
                }
            }
        }

        /// <summary>
        /// 发送链接请求
        /// </summary>
        public void SendConnect()
        {
            SocketClient.SendConnect();
        }

        /// <summary>
        /// 发送SOCKET消息
        /// </summary>
        public void SendMessage(ByteBuffer buffer)
        {
            SocketClient.SendMessage(buffer);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        void OnDestroy()
        {
            SocketClient.OnRemove();
            Log.Info("~NetworkManager was destroy");
        }
        

#region  注册网络消息回调  

        //绑定C#网络事件回调，仅在C#中调用
        public void BindCSharpCallback(int id,Action<Stream> action)
        {
            m_CSharpCallback[id] = action;
        }

        //绑定Lua网络事件回调，仅在lua中调用
        public void BindLuaCallback(int id,string module,string func)
        {
            LuaNetCallback evt = new LuaNetCallback();
            evt.module = module;
            evt.func = func;
            m_LuaCallback[id] = evt;
        }
#endregion
    }
