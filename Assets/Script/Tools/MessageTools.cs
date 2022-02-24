using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESendMessage
{
    All
}

public enum EReceiveMessage
{
    All
}

public class MessageTools : SingleBase<MessageTools>
{
    public override void Awake()
    {
        base.Awake();
    }

    public void sds()
    {
        Debug.Log("???");
    }

    public void Register(IReceiveMessage receiveMessage)
    {
        
    }
    private void SendMessage(ISendMessage sendMessage)
    {
        
    }
}

public interface ISendMessage
{
    public ESendMessage ESendMessage { get; set; }
    public void SendMessage();
}

public interface IReceiveMessage
{
    public EReceiveMessage EReceiveMessage { get; set; }
    public void ReceiveMessage(EReceiveMessage selfType);
}
