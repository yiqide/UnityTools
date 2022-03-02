using System;
using System.Collections;
using System.Collections.Generic;
using Framework.SingleMone;

namespace Framework.Tools
{
    public class CoroutineTools : SingleMonoBase<CoroutineTools>
    {
        protected override void Awake()
        {
            base.Awake();
            StartCoroutine(ActionWithUnityMainThread());
            StartCoroutine(ActionUnityMainThread());
        }

        private List<Func<bool>> _funcs = new List<Func<bool>>();
        private Queue<Action> _actions = new Queue<Action>();

        /// <summary>
        /// action这个方法将会在unity的主线程上调用
        /// </summary>
        public void StartActionWithUnityMainThread(Action action)
        {
            lock (_actions)
            {
                _actions.Enqueue(action);
            }
        }

        /// <summary>
        /// 添加到里面到方法会每一帧执行一次，返回true后将不会在执行
        /// </summary>
        /// <param name="action"></param>
        public void AddAction(Func<bool> action)
        {
            _funcs.Add(action);
        }

        /// <summary>
        /// 当state返回true时代表该任务已经结束了
        /// </summary>
        IEnumerator ActionUnityMainThread()
        {
            bool state = false;
            while (true)
            {
                int count = _funcs.Count;
                for (int i = 0; i < count; i++)
                {
                    state = _funcs[i].Invoke();
                    if (state)
                    {
                        _funcs.Remove(_funcs[i]);
                        i--;
                        count -= 1;
                    }

                    yield return null;
                }

                yield return null;
            }
        }

        IEnumerator ActionWithUnityMainThread()
        {
            while (true)
            {
                if (_actions.Count != 0)
                {
                    lock (_actions)
                    {
                        var action = _actions.Dequeue();
                        action.Invoke();
                    }
                }

                yield return null;
            }
        }
    }
}