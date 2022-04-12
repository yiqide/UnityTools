using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Framework.Tools;
using Unity.VisualScripting;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    void Start()
    {
        Debug.Log( sda(new int[]{1,2,3},new int[]{4,5,6}));
        return;
        TaskManager.ExecuteTaskAsync(t,5000, (b) =>
        {
            Debug.Log(b);
        });
    }

    private void t()
    {
        for (int i = 0; i < 10; i++)
        {
            Debug.Log(i);
            Thread.Sleep(2000);
           // new GameObject();
        }
    }

    private float sda(int[] a,int[] b )
    {
        int l = a.Length + b.Length;
        bool j;
        if ((float)l%2!=0)
        {
            //奇数
            j = true;
        }
        else
        {
            j = false;
        }
        Debug.Log(j);
        l = l / 2;
        if (j)
        {
            l++;
        }
        int pa = -1;
        int pb = -1;
        int sa = 0;
        int sb = 0;

        float rest = 0;
        Debug.Log(l);
        l++;
        if (j)
        {
            for (int i = 0; i < l; i++)
            {
                if (sa<sb)
                {
                    pa++;
                    sa = a[pa];
                    if (i==l-1)
                    {
                        rest = sa;
                    }
                }
                else
                {
                    pb++;
                    sb = b[pb];
                    if (i==l-1)
                    {
                        rest = sb;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < l+1; i++)
            {
                bool t = sa <= sb;
                int x = pa;
                if (pa<a.Length-1)
                {
                    if (t||pb==b.Length-1)
                    {
                        
                        pa++;
                        Debug.Log(pa);
                        sa = a[pa];
                        if (i==l)
                        {
                            rest += sa;
                            Debug.LogError(sa);
                        }
                        if (i==l-1)
                        {
                            rest += sa;
                            Debug.LogError(sa);
                        }
                    }
                }

                if (pb<b.Length-1)
                {
                    if ( !t || x==a.Length-1)
                    {
                        pb++;
                        Debug.Log(pb);
                        sb = b[pb];
                        if (i==l-1)
                        {
                            rest += sb;
                            Debug.LogError(sb);
                        }

                        if (i==l)
                        {
                            rest += sb;
                            Debug.LogError(sb);
                        }
                    }
                }
                Debug.LogWarning(i);
            }
        }

        if (!j)
        {
            rest = rest / 2;
        }

        return rest;
    }
}