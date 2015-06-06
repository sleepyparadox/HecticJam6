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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sleepy.UnityTools
{
    public class UnityEventAction<T0>
    {
        private event Action<T0> _action;
        public void Fire(T0 val)
        {
            _action(val);
        }

        /// <summary>
        /// Returns event if there wasn't one before
        /// </summary>
        /// <param name="unityAction"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static UnityEventAction<T0> operator +(UnityEventAction<T0> unityAction, Action<T0> action)
        {
            if (unityAction == null)
                unityAction = new UnityEventAction<T0>();
            unityAction._action += action;
            return unityAction;
        }

        /// <summary>
        /// Returns null if nothing is subscribed
        /// </summary>
        /// <param name="unityAction"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static UnityEventAction<T0> operator -(UnityEventAction<T0> unityAction, Action<T0> action)
        {
            if (unityAction == null)
                return null;

            unityAction._action -= action;

            if (unityAction._action == null)
                return null;
            
            return unityAction;
        }
    }

    public class UnityEventAction<T0, T1>
    {
        private event Action<T0, T1> _action;
        public void Fire(T0 val0, T1 val1)
        {
            _action(val0, val1);
        }

        /// <summary>
        /// Returns event if there wasn't one before
        /// </summary>
        /// <param name="unityAction"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static UnityEventAction<T0, T1> operator +(UnityEventAction<T0, T1> unityAction, Action<T0, T1> action)
        {
            if (unityAction == null)
                unityAction = new UnityEventAction<T0, T1>();
            unityAction._action += action;
            return unityAction;
        }

        /// <summary>
        /// Returns null if nothing is subscribed
        /// </summary>
        /// <param name="unityAction"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static UnityEventAction<T0, T1> operator -(UnityEventAction<T0, T1> unityAction, Action<T0, T1> action)
        {
            if (unityAction == null)
                return null;

            unityAction._action -= action;

            if (unityAction._action == null)
                return null;

            return unityAction;
        }
    }
}
