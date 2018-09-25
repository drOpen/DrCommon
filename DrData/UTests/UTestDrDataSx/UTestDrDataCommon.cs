/*
  UTestDrDataCommon.cs --  Common of Unit Tests for 'DrDataSx' general purpose Data abstraction layer 1.0.1, January 5, 2014
 
  Copyright (c) 2013-2014 Kudryashov Andrey aka Dr
 
  This software is provided 'as-is', without any express or implied
  warranty. In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

      1. The origin of this software must not be misrepresented; you must not
      claim that you wrote the original software. If you use this software
      in a product, an acknowledgment in the product documentation would be
      appreciated but is not required.

      2. Altered source versions must be plainly marked as such, and must not be
      misrepresented as being the original software.

      3. This notice may not be removed or altered from any source distribution.

      Kudryashov Andrey <kudryashov.andrey at gmail.com>

 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Text;

namespace UTestDrDataSe
{
    //[TestClass]
    static class UTestDrDataCommon
    {

        public const string ElementNameStrong1 = @"qweio234i0123658903946'""=-=12!~({)*($)*@#4-)|+_+_)\\=[]';,./?><:-/+\<DDValue>\\//";

        public static string GetTestMethodName()
        {
            // for when it runs via Visual Studio locally
            var stackTrace = new StackTrace();
            foreach (var stackFrame in stackTrace.GetFrames())
            {
                MethodBase methodBase = stackFrame.GetMethod();
                Object[] attributes = methodBase.GetCustomAttributes(typeof(TestMethodAttribute), false);
                if (attributes.Length >= 1)
                {
                    return methodBase.Name;
                }
            }
            return "Not called from a test method";
        }

        public static void WriteMemmoryStreamToXmlFile(MemoryStream stream)
        {
            WriteMemmoryStreamToFile(stream, UTestDrDataCommon.GetTestMethodName() + ".xml");
        }
        public static void WriteMemmoryStreamToBinFile(MemoryStream stream)
        {
            WriteMemmoryStreamToFile(stream, UTestDrDataCommon.GetTestMethodName() + ".bin");
        }
        public static void WriteMemmoryStreamToFile(MemoryStream stream, string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                stream.WriteTo(file);
            }
        }

        #region GetMemoryStreamFromFile
        public static MemoryStream GetMemoryStreamFromFile()
        {
            return GetMemoryStreamFromFile(".\\XML\\" +  GetTestMethodName() + ".xml");
        }

        public static MemoryStream GetMemoryStreamFromFile(string file)
        {
            var ms = new MemoryStream();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                ms.Write(bytes, 0, (int)fs.Length);
            }
            ms.Position = 0;
            return ms;
        }
        #endregion GetMemoryStreamFromFile

        public static MemoryStream GetMemoryStreamFromString(string s)
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(s);
            writer.Flush();
            ms.Position = 0;
            return ms;
        }
    }
}
