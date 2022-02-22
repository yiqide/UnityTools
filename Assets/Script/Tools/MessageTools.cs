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
    public void sds()
    {
        Debug.Log("???");
    }
}

public interface ISendMessage
{
    public void SendMessage();
}

public interface IReceiveMessage
{
    public void ReceiveMessage();
}
