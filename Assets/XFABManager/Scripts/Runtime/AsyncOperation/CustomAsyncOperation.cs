﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XFABManager
{
    public class CustomAsyncOperation<T> : CustomYieldInstruction where T : CustomAsyncOperation<T>
    {
        #region 字段 
       
        public event Action<T> completed;
        //public event Action<int> completed;

        private bool _isCompleted;

        #endregion

        #region 属性 

        public bool isDone { get { return isCompleted; } }


        protected bool isCompleted {
            get {
                return _isCompleted;
            }

            set {
                _isCompleted = value;
                if (_isCompleted) {
                    completed?.Invoke(this as T);
                    OnCompleted();
                }
            }
        }

        public float progress { get; protected set; }

        public string error { get; protected set; }

        #endregion

        public override bool keepWaiting
        {
            get
            {
                return !isDone;
            }
        }

        protected virtual void OnCompleted() { 

        }

        protected void Completed(string error) {
            this.error = error;
            Completed();
        }

        protected void Completed() {
            progress = 1;

            if ( isCompleted ) return;
            isCompleted = true;
        }

    }

}
