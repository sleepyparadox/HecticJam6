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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sleepy.UnityTools
{
    public partial class TinyCoro
    {
        /// <summary>
        /// Adds an new TinyCoro to the pool after the currently exceutiny one (or last if none are running) 
        /// </summary>
        /// <param name="threadedOperation"></param>
        /// <returns></returns>
        public static TinyCoro SpawnNext(Func<IEnumerator> operation)
        {
            var coro = new TinyCoro(operation);
            var index = _allCoros.Count;
            if (_currentCoro != null)
                index = _allCoros.IndexOf(_currentCoro) + 1;
            _allCoros.Insert(index, coro);
            return coro;
        }

        public static void StepAllCoros()
        {
            for (int i = 0; i < _allCoros.Count; )
            {
                //Step normal
                _currentCoro = _allCoros[i];
                if (_currentCoro.Alive)
                    _currentCoro.Step();
                if (!_currentCoro.Alive)
                {
                    _allCoros.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
            _currentCoro = null;
        }


        /// <summary>
        /// Human readable version of "yield return of Func<bool>"
        /// Any half decent compiler should optimize this :P
        /// </summary>
        /// <param name="conditionMet"></param>
        /// <returns></returns>
        public static Func<bool> WaitUntil(Func<bool> conditionMet)
        {
            return conditionMet;
        }

        public static Func<bool> Wait(float seconds)
        {
            var destinationTime = Time.time + seconds;
            return () => Time.time >= destinationTime;
        }

        public static IEnumerable<TinyCoro> AllCoroutines { get { return _allCoros; } }

        private static List<TinyCoro> _allCoros = new List<TinyCoro>();
        private static TinyCoro _currentCoro;
    }
}
