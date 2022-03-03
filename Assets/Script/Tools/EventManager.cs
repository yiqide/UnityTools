using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Setting;
using Framework.SingleMone;
using UnityEngine;

namespace Framework.Tools
{
    [SingleScript(false)]
    public class EventManager : SingleBase<EventManager>
    {
        //key-注册者的类型  value-这种类型的注册者注册的方法表
        private Dictionary<EReceiveType, List<Pakg>> receiveActions = new Dictionary<EReceiveType, List<Pakg>>();

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <param name="selfType"></param>
        /// <param name="sendType">你想接受的那些发送至者发送的消息</param>
        /// <param name="receiveAction">要注册的方法</param>
        public void RegisterEvent(EReceiveType selfType, ESendType sendType, Action<object[]> receiveAction)
        {
            if (!receiveActions.ContainsKey(selfType))
            {
                receiveActions.Add(selfType, new List<Pakg>());
            }

            receiveActions[selfType].Add(new Pakg(sendType, receiveAction));
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="selfType"></param>
        /// <param name="receiveType">接受者</param>
        /// <param name="objects"></param>
        public void SendEvent(ESendType selfType, EReceiveType receiveType, params object[] objects)
        {
            if (receiveType == EReceiveType.EAll)
            {
                foreach (var item in receiveActions.Values)
                {
                    foreach (var value in item)
                    {
                        if (value.sendType == ESendType.EAll || value.sendType == selfType)
                        {
                            value.Action(objects);
                        }
                    }
                }

                return;
            }

            if (receiveActions.ContainsKey(receiveType))
            {
                foreach (var item in receiveActions[receiveType])
                {
                    if (item.sendType == ESendType.EAll || item.sendType == selfType)
                    {
                        item.Action(objects);
                    }
                }
            }
        }

        private class Pakg
        {
            public ESendType sendType;
            public Action<object[]> Action;

            public Pakg(ESendType sendType, Action<object[]> Action)
            {
                this.sendType = sendType;
                this.Action = Action;
            }
        }
    }
}