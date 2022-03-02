using System;
using Framework.Setting;
using UnityEngine;

namespace Framework.Tools
{
    /// <summary>
    /// 扩展方法类
    /// </summary>
    public static class ExtensionFunction
    {
        /// <summary>
        /// 将一张Texture2D转换成Sprite 推荐在编辑器中使用
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public static Texture2D GetTexture2D(this Sprite sprite)
        {
            Texture2D targetTex = new Texture2D((int) sprite.rect.width, (int) sprite.rect.height);
            Color[] pixels = sprite.texture.GetPixels(
                (int) sprite.rect.x,
                (int) sprite.rect.y,
                (int) sprite.rect.width,
                (int) sprite.rect.height);
            targetTex.SetPixels(pixels);
            targetTex.Apply();
            return targetTex;
        }

        /// <summary>
        /// 获取sprite的Texture2D，并且这张图片是只读的，只能在运行时使用
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public static Texture2D GetOnlyReadTexture2D(this Sprite sprite)
        {
            //获取图片
            RenderTexture tmp = RenderTexture.GetTemporary(
                sprite.texture.width,
                sprite.texture.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear
            );
            Graphics.Blit(sprite.texture, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;
            Texture2D myTexture2D = new Texture2D(sprite.texture.width, sprite.texture.height);
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);
            return myTexture2D;
        }

        /// <summary>
        /// 将texture2D转换成Sprite
        /// </summary>
        /// <param name="texture2D"></param>
        /// <returns></returns>
        public static Sprite GetSprite(this Texture2D texture2D)
        {
            return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
        }

        /// <summary>
        /// 计算a相对与b的夹角 结果是 0 - 360之间
        /// 该方法只支持水平面里的计算（二维坐标）
        /// </summary>
        /// <param name="a">向量</param>
        /// <param name="b">向量</param>
        /// <returns>返回夹角的度数0-360</returns>
        public static float GetIncludedAngle(this Vector2 a, Vector2 b)
        {
            //计算夹角
            var s = a * b;
            float res = (float) Math.Acos((s.x + s.y) / a.magnitude * b.magnitude);
            res = res * 180 / 3.1415f /*(float) Math.PI*/;
            //计算方向
            //var a1= (Vector3) a;
            //var b1 = (Vector3) b;
            //a1 * b1
            //var v3= new Vector3(a1.y * b1.z - a1.z * b1.y,a1.z*b1.x-a1.x*b1.z,a1.x*b1.y-a1.y*b1.x);
            if (a.x * b.y - a.y * b.x > 0)
            {
                res = 360 - res;
                //b在 a的上方
            }

            return res;
        }

        #region 事件系统 --MessageManager

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="self">表明你的身份</param>
        /// <param name="receiveType">接受者</param>
        /// <param name="objects">发送的参数</param>
        public static void SendEvent(this ISendEvent self, EReceiveType receiveType, params object[] objects)
        {
            if (self.SendType == ESendType.EAll)
            {
                Debug.LogWarning("发送者的类型无效：all");
                return;
            }

            EventManager.Instance.SendEvent(self.SendType, receiveType, objects);
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="self">表明你的身份</param>
        /// <param name="sendType">你想接受的那些发送至者发送的消息</param>
        /// <param name="receiveAction">注册的方法</param>
        public static void RegisterEvent(this IReceiveEvent self, ESendType sendType, Action<object[]> receiveAction)
        {
            if (self.ReceiveType == EReceiveType.EAll)
            {
                Debug.LogWarning("接受者的类型无效：all");
                return;
            }

            EventManager.Instance.RegisterEvent(self.ReceiveType, sendType, receiveAction);
        }

        #endregion
    }
}