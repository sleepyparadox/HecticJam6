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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sleepy.UnityTools
{
    public static class TinyThread
    {
        public static TinyCoro SpawnAfter(Action threadedOperation)
        {
            return TinyCoro.SpawnNext(() => PreformThreadedOperation(threadedOperation));
        }

        private static IEnumerator PreformThreadedOperation(Action threadedOperation)
        {
            var thread = new Thread(new ThreadStart(threadedOperation));

            thread.Start();

            yield return TinyCoro.WaitUntil(() => !thread.IsAlive);
        }
    }

    public static class TinyThread<TResult>
    {
        public static TinyCoro SpawnAfter(Func<TResult> threadedOperation, Action<TResult> onSuccess)
        {
            return TinyCoro.SpawnNext(() => PreformThreadedOperation(threadedOperation, onSuccess));
        }

        private static IEnumerator PreformThreadedOperation(Func<TResult> threadedOperation, Action<TResult> onSuccess)
        {
            object threadResult = null;

            var thread = new Thread(new ThreadStart(() =>
            {
                threadResult = threadedOperation();
            }));

            thread.Start();

            yield return TinyCoro.WaitUntil(() => !thread.IsAlive);

            onSuccess((TResult)threadResult);
        }
    }
}
