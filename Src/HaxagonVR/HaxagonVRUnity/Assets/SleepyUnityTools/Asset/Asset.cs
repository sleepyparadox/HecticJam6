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
using UnityEngine;

namespace Sleepy.UnityTools
{
    public class Asset
    {
        public readonly string AssetPath;
        public bool Loaded { get; private set; }
        public string Name 
        {
            get
            {
                if (!Loaded)
                    throw new Exception("Failed to get name, Asset not loaded! path: " + AssetPath);
                return _obj.name;
            }
        }

        private UnityEngine.Object _obj;

        public Asset(string path)
        {
            AssetPath = path;
        }

        public UnityEngine.Object GetObject(Type type)
        {
            if (Loaded
                && _obj.GetType() != type)
                throw new Exception(AssetPath + " is already loaded as " + _obj.GetType() + "not a " + type);

            if (!Loaded)
            {
                _obj = UnityEngine.Resources.Load(AssetPath, type);
                Loaded = true;
                //Debug.Log("Loaded " + type + " at " + AssetPath);
            }

            //Debug.Log("Returning " + (_obj != null ? _obj.ToString() : "null"));

            return _obj;
        }
    }
}
