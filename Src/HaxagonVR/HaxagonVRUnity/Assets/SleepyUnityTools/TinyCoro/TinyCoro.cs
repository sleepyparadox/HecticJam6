///
/// The latest version of SleepyUnityTools can be found at 
/// https://github.com/sleepyparadox/SleepyUnityTools
///
/// The MIT License (MIT)
///
/// Copyright (c) 2015 Don Logan (sleepyparadox)
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:

/// The above copyright notice and this permission notice shall be included in all
/// copies or substantial portions of the Software.

/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
/// SOFTWARE.
///

using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace Sleepy.UnityTools
{
    public partial class TinyCoro
    {
        public bool Alive { get; private set; }
        public event Action<TinyCoro, TinyCoroFinishReason> OnFinished;

        public virtual void Kill()
        {
            if (!Alive)
                return;
            if (OnFinished != null)
                OnFinished(this, TinyCoroFinishReason.Killed);
            Alive = false;
        }

        protected TinyCoro(Func<IEnumerator> operation)
        {
            Alive = true;
            _operation = operation;
        }

        private void Step()
        {
            if (_waitingOn != null)
            {
                if (!_waitingOn())
                    return;
                // Finished waiting
                _waitingOn = null;
            }

            if (_ienumerable == null)
            {
                // Execute first block of code
                _ienumerable = _operation();
            }
            else if (_ienumerable.MoveNext())
            {
                // Executed another block of code
                if (_ienumerable.Current != null)
                {
                    if (_ienumerable.Current is Func<bool>)
                    {
                        _waitingOn = (Func<bool>)_ienumerable.Current;
                    }
                    else
                    {
                        throw new Exception(this + " expected to yield null or a Func<bool> instead a " + _ienumerable.Current + " of type " + _ienumerable.Current.GetType() + " was returned");
                    }
                }
            }
            else
            {
                // Executed final block of code
                Alive = false;
                if (OnFinished != null)
                    OnFinished(this, TinyCoroFinishReason.Finished);
            }
        }

        private Func<IEnumerator> _operation;
        private IEnumerator _ienumerable;
        private Func<bool> _waitingOn;
    }

    public enum TinyCoroFinishReason
    {
        Finished,
        Killed,
    }
}
