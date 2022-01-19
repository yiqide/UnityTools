using System;
using System.Collections;
using System.Collections.Generic;

public class CoroutineTools : SingleMonoBase<CoroutineTools>
{

    public override void Init()
    {
        base.Init();
        StartCoroutine(ActionWithUnityMainThread());
        StartCoroutine(ActionUnityMainThread());
    }

    private List<MyAction> _myActions = new List<MyAction>();
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
    public void AddAction(MyAction action)
    {
        _myActions.Add(action);
    }

    IEnumerator ActionUnityMainThread()
    {
        bool state = false;
        while (true)
        {
            int count = _myActions.Count;
            for (int i = 0; i < count; i++)
            {
                state = _myActions[i].Invoke();
                if (state)
                {
                    _myActions.Remove(_myActions[i]);
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

    /// <summary>
    /// 当state返回true时代表该任务已经结束了
    /// </summary>
    public delegate bool MyAction();

}