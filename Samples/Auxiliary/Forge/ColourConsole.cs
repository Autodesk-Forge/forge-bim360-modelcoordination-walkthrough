/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Forge
{
    public static class ColourConsole
    {
        public static void WriteSuccess(string message)
        {
            var current = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Success");
            }
            finally
            {
                Console.ForegroundColor = current;
            }

            Console.WriteLine($" - {message}");
        }

        public static void WriteInfo(string message)
        {
            var current = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("Info   ");
            }
            finally
            {
                Console.ForegroundColor = current;
            }

            Console.WriteLine($" - {message}");
        }

        public static void WriteWarning(string message)
        {
            var current = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Info   ");
            }
            finally
            {
                Console.ForegroundColor = current;
            }

            Console.WriteLine($" - {message}");
        }

        public static void WriteFail(string message)
        {
            var current = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Fail   ");
            }
            finally
            {
                Console.ForegroundColor = current;
            }

            Console.WriteLine($" - {message}");
        }
    }
}
