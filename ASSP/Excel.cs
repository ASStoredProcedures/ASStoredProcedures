/*============================================================================
  File:    Excel.cs

  Summary: Mimics the functionality of the Excel library which can be called
           from an MDX query. If the compiled assembly is registered at the
           database level with the name "ExcelMDX" then the Microsoft Excel
           library will not be called.

  Date:    July 12, 2006

  ----------------------------------------------------------------------------
  This file is part of the Analysis Services Stored Procedure Project.
  http://www.codeplex.com/ASStoredProcedures
  
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/

using System;
using Microsoft.AnalysisServices.AdomdServer;

namespace ASStoredProcs
{

    public class Excel
    {
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4000)]
        //double Count([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4002)]
        //bool IsNA([In, MarshalAs(UnmanagedType.Struct)] object Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4003)]
        //bool IsError([In, MarshalAs(UnmanagedType.Struct)] object Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4004)]
        //double Sum([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4005)]
        //double Average([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4006)]
        //double Min([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4007)]
        //double Max([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x400b)]
        //double Npv([In] double Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x400c)]
        //double StDev([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x400d)]
        //string Dollar([In] double Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x400e)]
        //string Fixed([In] double Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4013)]
        //double Pi();
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4016)]
        //double Ln([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4017)]
        //double Log10([In] double Arg1);

        //double Round([In] double Arg1, [In] double Arg2);
        public static double Round(double Arg1, double Arg2)
        {
            return Math.Round(Arg1, (int)Arg2, MidpointRounding.AwayFromZero); //don't use banker's rounding
        }

        //string Rept([In, MarshalAs(UnmanagedType.BStr)] string Arg1, [In] double Arg2);
        public static string Rept(string Arg1, double Arg2)
        {
            return Microsoft.VisualBasic.Strings.StrDup((int)Arg2, Arg1);
        }
        
        //bool And([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        public static bool And(bool Arg1)
        {
            return Arg1;
        }
        public static bool And(bool Arg1, bool Arg2)
        {
            return Arg1 && Arg2;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3)
        {
            return Arg1 && Arg2 && Arg3;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4)
        {
            return Arg1 && Arg2 && Arg3 && Arg4;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17 && Arg18;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17 && Arg18 && Arg19;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17 && Arg18 && Arg19 && Arg20;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17 && Arg18 && Arg19 && Arg20 && Arg21;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17 && Arg18 && Arg19 && Arg20 && Arg21 && Arg22;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17 && Arg18 && Arg19 && Arg20 && Arg21 && Arg22 && Arg23;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17 && Arg18 && Arg19 && Arg20 && Arg21 && Arg22 && Arg23 && Arg24;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24, bool Arg25)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17 && Arg18 && Arg19 && Arg20 && Arg21 && Arg22 && Arg23 && Arg24 && Arg25;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24, bool Arg25, bool Arg26)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17 && Arg18 && Arg19 && Arg20 && Arg21 && Arg22 && Arg23 && Arg24 && Arg25 && Arg26;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24, bool Arg25, bool Arg26, bool Arg27)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17 && Arg18 && Arg19 && Arg20 && Arg21 && Arg22 && Arg23 && Arg24 && Arg25 && Arg26 && Arg27;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24, bool Arg25, bool Arg26, bool Arg27, bool Arg28)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17 && Arg18 && Arg19 && Arg20 && Arg21 && Arg22 && Arg23 && Arg24 && Arg25 && Arg26 && Arg27 && Arg28;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24, bool Arg25, bool Arg26, bool Arg27, bool Arg28, bool Arg29)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17 && Arg18 && Arg19 && Arg20 && Arg21 && Arg22 && Arg23 && Arg24 && Arg25 && Arg26 && Arg27 && Arg28 && Arg29;
        }
        public static bool And(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24, bool Arg25, bool Arg26, bool Arg27, bool Arg28, bool Arg29, bool Arg30)
        {
            return Arg1 && Arg2 && Arg3 && Arg4 && Arg5 && Arg6 && Arg7 && Arg8 && Arg9 && Arg10 && Arg11 && Arg12 && Arg13 && Arg14 && Arg15 && Arg16 && Arg17 && Arg18 && Arg19 && Arg20 && Arg21 && Arg22 && Arg23 && Arg24 && Arg25 && Arg26 && Arg27 && Arg28 && Arg29 && Arg30;
        }
        public static bool And(Set s)
        {
            foreach (Tuple t in s.Tuples)
            {
                if (!new Expression("[Measures].CurrentMember").Calculate(t).ToBool())
                {
                    return false;
                }
            }
            return true;
        }

        //bool Or([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        public static bool Or(bool Arg1)
        {
            return Arg1;
        }
        public static bool Or(bool Arg1, bool Arg2)
        {
            return Arg1 || Arg2;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3)
        {
            return Arg1 || Arg2 || Arg3;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4)
        {
            return Arg1 || Arg2 || Arg3 || Arg4;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17 || Arg18;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17 || Arg18 || Arg19;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17 || Arg18 || Arg19 || Arg20;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17 || Arg18 || Arg19 || Arg20 || Arg21;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17 || Arg18 || Arg19 || Arg20 || Arg21 || Arg22;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17 || Arg18 || Arg19 || Arg20 || Arg21 || Arg22 || Arg23;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17 || Arg18 || Arg19 || Arg20 || Arg21 || Arg22 || Arg23 || Arg24;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24, bool Arg25)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17 || Arg18 || Arg19 || Arg20 || Arg21 || Arg22 || Arg23 || Arg24 || Arg25;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24, bool Arg25, bool Arg26)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17 || Arg18 || Arg19 || Arg20 || Arg21 || Arg22 || Arg23 || Arg24 || Arg25 || Arg26;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24, bool Arg25, bool Arg26, bool Arg27)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17 || Arg18 || Arg19 || Arg20 || Arg21 || Arg22 || Arg23 || Arg24 || Arg25 || Arg26 || Arg27;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24, bool Arg25, bool Arg26, bool Arg27, bool Arg28)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17 || Arg18 || Arg19 || Arg20 || Arg21 || Arg22 || Arg23 || Arg24 || Arg25 || Arg26 || Arg27 || Arg28;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24, bool Arg25, bool Arg26, bool Arg27, bool Arg28, bool Arg29)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17 || Arg18 || Arg19 || Arg20 || Arg21 || Arg22 || Arg23 || Arg24 || Arg25 || Arg26 || Arg27 || Arg28 || Arg29;
        }
        public static bool Or(bool Arg1, bool Arg2, bool Arg3, bool Arg4, bool Arg5, bool Arg6, bool Arg7, bool Arg8, bool Arg9, bool Arg10, bool Arg11, bool Arg12, bool Arg13, bool Arg14, bool Arg15, bool Arg16, bool Arg17, bool Arg18, bool Arg19, bool Arg20, bool Arg21, bool Arg22, bool Arg23, bool Arg24, bool Arg25, bool Arg26, bool Arg27, bool Arg28, bool Arg29, bool Arg30)
        {
            return Arg1 || Arg2 || Arg3 || Arg4 || Arg5 || Arg6 || Arg7 || Arg8 || Arg9 || Arg10 || Arg11 || Arg12 || Arg13 || Arg14 || Arg15 || Arg16 || Arg17 || Arg18 || Arg19 || Arg20 || Arg21 || Arg22 || Arg23 || Arg24 || Arg25 || Arg26 || Arg27 || Arg28 || Arg29 || Arg30;
        }
        public static bool Or(Set s)
        {
            foreach (Tuple t in s.Tuples)
            {
                if (new Expression("[Measures].CurrentMember").Calculate(t).ToBool())
                {
                    return true;
                }
            }
            return false;
        }
        
        //double Var([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        public static double Var(double Arg1)
        {
            return Var(new double[] { Arg1 });
        }
        public static double Var(double Arg1, double Arg2)
        {
            return Var(new double[] { Arg1, Arg2 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3)
        {
            return Var(new double[] { Arg1, Arg2, Arg3 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24, double Arg25)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24, Arg25 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24, double Arg25, double Arg26)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24, Arg25, Arg26 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24, double Arg25, double Arg26, double Arg27)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24, Arg25, Arg26, Arg27 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24, double Arg25, double Arg26, double Arg27, double Arg28)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24, Arg25, Arg26, Arg27, Arg28 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24, double Arg25, double Arg26, double Arg27, double Arg28, double Arg29)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24, Arg25, Arg26, Arg27, Arg28, Arg29 });
        }
        public static double Var(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24, double Arg25, double Arg26, double Arg27, double Arg28, double Arg29, double Arg30)
        {
            return Var(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24, Arg25, Arg26, Arg27, Arg28, Arg29, Arg30 });
        }
        //no need for a "public static double Var(Set s)" signature because there's already a Var built-in MDX function
        public static double Var(double[] ar)
        {
            double sum = 0;
            double sum2 = 0;
            int count = 0;
            foreach (double temp in ar)
            {
                if (temp.CompareTo(double.NaN) != 0)
                {
                    sum += temp;
                    sum2 += (temp * temp);
                    count++;
                }
            }

            double result;
            if (count > 1)
            {
                result = (count * sum2 - sum * sum) / (count * (count - 1));
            }
            else
            {
                throw new Exception("Two or more parameters are required!");
            }
            return result;
        }

        //double VarP([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        public static double VarP(double Arg1)
        {
            return VarP(new double[] { Arg1 });
        }
        public static double VarP(double Arg1, double Arg2)
        {
            return VarP(new double[] { Arg1, Arg2 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24, double Arg25)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24, Arg25 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24, double Arg25, double Arg26)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24, Arg25, Arg26 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24, double Arg25, double Arg26, double Arg27)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24, Arg25, Arg26, Arg27 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24, double Arg25, double Arg26, double Arg27, double Arg28)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24, Arg25, Arg26, Arg27, Arg28 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24, double Arg25, double Arg26, double Arg27, double Arg28, double Arg29)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24, Arg25, Arg26, Arg27, Arg28, Arg29 });
        }
        public static double VarP(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, double Arg6, double Arg7, double Arg8, double Arg9, double Arg10, double Arg11, double Arg12, double Arg13, double Arg14, double Arg15, double Arg16, double Arg17, double Arg18, double Arg19, double Arg20, double Arg21, double Arg22, double Arg23, double Arg24, double Arg25, double Arg26, double Arg27, double Arg28, double Arg29, double Arg30)
        {
            return VarP(new double[] { Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, Arg12, Arg13, Arg14, Arg15, Arg16, Arg17, Arg18, Arg19, Arg20, Arg21, Arg22, Arg23, Arg24, Arg25, Arg26, Arg27, Arg28, Arg29, Arg30 });
        }
        //no need for a "public static double VarP(Set s)" signature because there's already a VarP built-in MDX function
        public static double VarP(double[] ar)
        {
            double sum = 0;
            double sum2 = 0;
            int count = 0;
            foreach (double temp in ar)
            {
                if (temp.CompareTo(double.NaN) != 0)
                {
                    sum += temp;
                    sum2 += (temp * temp);
                    count++;
                }
            }

            double result;
            if (count > 0)
            {
                result = ((count * sum2 - sum * sum) / (count * count));
            }
            else
            {
                throw new Exception("One or more parameter is required!");
            }
            return result;
        }
        
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4030)]
        //string Text([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.BStr)] string Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4038)]
        //double Pv([In] double Arg1, [In] double Arg2, [In] double Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4039)]
        //double Fv([In] double Arg1, [In] double Arg2, [In] double Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x403a)]
        //double NPer([In] double Arg1, [In] double Arg2, [In] double Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x403b)]
        //double Pmt([In] double Arg1, [In] double Arg2, [In] double Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x403c)]
        //double Rate([In] double Arg1, [In] double Arg2, [In] double Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x403d)]
        //double MIrr([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In] double Arg2, [In] double Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x403e)]
        //double Irr([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4040)]
        //double Match([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4046)]
        //double Weekday([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4052)]
        //double Search([In, MarshalAs(UnmanagedType.BStr)] string Arg1, [In, MarshalAs(UnmanagedType.BStr)] string Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4053)]
        //object Transpose([In, MarshalAs(UnmanagedType.Struct)] object Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4061)]
        //double Atan2([In] double Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4062)]
        //double Asin([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4063)]
        //double Acos([In] double Arg1);
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4064)]
        //object Choose([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x406d)]
        //double Log([In] double Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4072)]
        //string Proper([In, MarshalAs(UnmanagedType.BStr)] string Arg1);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4076)]
        //string Trim([In, MarshalAs(UnmanagedType.BStr)] string Arg1);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4077)]
        //string Replace([In, MarshalAs(UnmanagedType.BStr)] string Arg1, [In] double Arg2, [In] double Arg3, [In, MarshalAs(UnmanagedType.BStr)] string Arg4);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4078)]
        //string Substitute([In, MarshalAs(UnmanagedType.BStr)] string Arg1, [In, MarshalAs(UnmanagedType.BStr)] string Arg2, [In, MarshalAs(UnmanagedType.BStr)] string Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x407c)]
        //double Find([In, MarshalAs(UnmanagedType.BStr)] string Arg1, [In, MarshalAs(UnmanagedType.BStr)] string Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x407e)]
        //bool IsErr([In, MarshalAs(UnmanagedType.Struct)] object Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x407f)]
        //bool IsText([In, MarshalAs(UnmanagedType.Struct)] object Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4080)]
        //bool IsNumber([In, MarshalAs(UnmanagedType.Struct)] object Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x408e)]
        //double Sln([In] double Arg1, [In] double Arg2, [In] double Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x408f)]
        //double Syd([In] double Arg1, [In] double Arg2, [In] double Arg3, [In] double Arg4);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4090)]
        //double Ddb([In] double Arg1, [In] double Arg2, [In] double Arg3, [In] double Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40a2)]
        //string Clean([In, MarshalAs(UnmanagedType.BStr)] string Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40a7)]
        //double Ipmt([In] double Arg1, [In] double Arg2, [In] double Arg3, [In] double Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40a8)]
        //double Ppmt([In] double Arg1, [In] double Arg2, [In] double Arg3, [In] double Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40a9)]
        //double CountA([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40b7)]
        //double Product([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40b8)]
        //double Fact([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40be)]
        //bool IsNonText([In, MarshalAs(UnmanagedType.Struct)] object Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40c1)]
        //double StDevP([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40c6)]
        //bool IsLogical([In, MarshalAs(UnmanagedType.Struct)] object Arg1);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40cc)]
        //string USDollar([In] double Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40cd)]
        //double FindB([In, MarshalAs(UnmanagedType.BStr)] string Arg1, [In, MarshalAs(UnmanagedType.BStr)] string Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40ce)]
        //double SearchB([In, MarshalAs(UnmanagedType.BStr)] string Arg1, [In, MarshalAs(UnmanagedType.BStr)] string Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40cf)]
        //string ReplaceB([In, MarshalAs(UnmanagedType.BStr)] string Arg1, [In] double Arg2, [In] double Arg3, [In, MarshalAs(UnmanagedType.BStr)] string Arg4);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40d4)]
        //double RoundUp([In] double Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40d5)]
        //double RoundDown([In] double Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40dc)]
        //double Days360([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40de)]
        //double Vdb([In] double Arg1, [In] double Arg2, [In] double Arg3, [In] double Arg4, [In] double Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40e3)]
        //double Median([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40e4)]
        //double SumProduct([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40e5)]
        //double Sinh([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40e6)]
        //double Cosh([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40e7)]
        //double Tanh([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40e8)]
        //double Asinh([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40e9)]
        //double Acosh([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40ea)]
        //double Atanh([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40f7)]
        //double Db([In] double Arg1, [In] double Arg2, [In] double Arg3, [In] double Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x410d)]
        //double AveDev([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x410e)]
        //double BetaDist([In] double Arg1, [In] double Arg2, [In] double Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x410f)]
        //double GammaLn([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4110)]
        //double BetaInv([In] double Arg1, [In] double Arg2, [In] double Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4111)]
        //double BinomDist([In] double Arg1, [In] double Arg2, [In] double Arg3, [In] bool Arg4);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4112)]
        //double ChiDist([In] double Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4113)]
        //double ChiInv([In] double Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4114)]
        //double Combin([In] double Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4115)]
        //double Confidence([In] double Arg1, [In] double Arg2, [In] double Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4116)]
        //double CritBinom([In] double Arg1, [In] double Arg2, [In] double Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4117)]
        //double Even([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4118)]
        //double ExponDist([In] double Arg1, [In] double Arg2, [In] bool Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4119)]
        //double FDist([In] double Arg1, [In] double Arg2, [In] double Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x411a)]
        //double FInv([In] double Arg1, [In] double Arg2, [In] double Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x411b)]
        //double Fisher([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x411c)]
        //double FisherInv([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x411d)]
        //double Floor([In] double Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x411e)]
        //double GammaDist([In] double Arg1, [In] double Arg2, [In] double Arg3, [In] bool Arg4);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x411f)]
        //double GammaInv([In] double Arg1, [In] double Arg2, [In] double Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4120)]
        //double Ceiling([In] double Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4121)]
        //double HypGeomDist([In] double Arg1, [In] double Arg2, [In] double Arg3, [In] double Arg4);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4122)]
        //double LogNormDist([In] double Arg1, [In] double Arg2, [In] double Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4123)]
        //double LogInv([In] double Arg1, [In] double Arg2, [In] double Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4124)]
        //double NegBinomDist([In] double Arg1, [In] double Arg2, [In] double Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4125)]
        //double NormDist([In] double Arg1, [In] double Arg2, [In] double Arg3, [In] bool Arg4);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4126)]
        //double NormSDist([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4127)]
        //double NormInv([In] double Arg1, [In] double Arg2, [In] double Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4128)]
        //double NormSInv([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4129)]
        //double Standardize([In] double Arg1, [In] double Arg2, [In] double Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x412a)]
        //double Odd([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x412b)]
        //double Permut([In] double Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x412c)]
        //double Poisson([In] double Arg1, [In] double Arg2, [In] bool Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x412d)]
        //double TDist([In] double Arg1, [In] double Arg2, [In] double Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x412e)]
        //double Weibull([In] double Arg1, [In] double Arg2, [In] double Arg3, [In] bool Arg4);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x412f)]
        //double SumXMY2([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4130)]
        //double SumX2MY2([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4131)]
        //double SumX2PY2([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4132)]
        //double ChiTest([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4133)]
        //double Correl([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4134)]
        //double Covar([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4135)]
        //double Forecast([In] double Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4136)]
        //double FTest([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4137)]
        //double Intercept([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4138)]
        //double Pearson([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4139)]
        //double RSq([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x413a)]
        //double StEyx([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x413b)]
        //double Slope([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x413c)]
        //double TTest([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In] double Arg3, [In] double Arg4);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x413d)]
        //double Prob([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In] double Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x413e)]
        //double DevSq([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x413f)]
        //double GeoMean([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4140)]
        //double HarMean([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4141)]
        //double SumSq([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4142)]
        //double Kurt([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4143)]
        //double Skew([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4144)]
        //double ZTest([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In] double Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4145)]
        //double Large([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4146)]
        //double Small([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4147)]
        //double Quartile([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4148)]
        //double Percentile([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4149)]
        //double PercentRank([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In] double Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x414a)]
        //double Mode([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x414b)]
        //double TrimMean([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x414c)]
        //double TInv([In] double Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4151)]
        //double Power([In] double Arg1, [In] double Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4156)]
        //double Radians([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4157)]
        //double Degrees([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x415e)]
        //double Ispmt([In] double Arg1, [In] double Arg2, [In] double Arg3, [In] double Arg4);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4162)]
        //string Roman([In] double Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40d6)]
        //string Asc([In, MarshalAs(UnmanagedType.BStr)] string Arg1);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40d7)]
        //string Dbcs([In, MarshalAs(UnmanagedType.BStr)] string Arg1);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4170)]
        //string BahtText([In] double Arg1);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4171), TypeLibFunc((short)0x40)]
        //string ThaiDayOfWeek([In] double Arg1);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4172), TypeLibFunc((short)0x40)]
        //string ThaiDigit([In, MarshalAs(UnmanagedType.BStr)] string Arg1);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc((short)0x40), DispId(0x4173)]
        //string ThaiMonthOfYear([In] double Arg1);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4174), TypeLibFunc((short)0x40)]
        //string ThaiNumSound([In] double Arg1);
        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4175), TypeLibFunc((short)0x40)]
        //string ThaiNumString([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc((short)0x40), DispId(0x4176)]
        //double ThaiStringLength([In, MarshalAs(UnmanagedType.BStr)] string Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc((short)0x40), DispId(0x4177)]
        //bool IsThaiDigit([In, MarshalAs(UnmanagedType.BStr)] string Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc((short)0x40), DispId(0x4178)]
        //double RoundBahtDown([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4179), TypeLibFunc((short)0x40)]
        //double RoundBahtUp([In] double Arg1);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x417a), TypeLibFunc((short)0x40)]
        //double ThaiYear([In] double Arg1);
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x417b)]
        //object RTD([In, MarshalAs(UnmanagedType.Struct)] object progID, [In, MarshalAs(UnmanagedType.Struct)] object server, [In, MarshalAs(UnmanagedType.Struct)] object topic1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object topic28);







        #region Not Supported
        //[DispId(0x94)]
        //Application Application { [return: MarshalAs(UnmanagedType.Interface)] [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x94)] get; }
        //[DispId(0x95)]
        //XlCreator Creator { [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x95)] get; }
        //[DispId(150)]
        //object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(150)] get; }
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc((short)0x440), DispId(0xa9)]
        //object _WSFunction([In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);

        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4158)]
        //double Subtotal([In] double Arg1, [In, MarshalAs(UnmanagedType.Interface)] Range Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg5, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg6, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg7, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg8, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg9, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg10, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg11, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg12, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg13, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg14, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg15, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg16, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg17, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg18, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg19, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg20, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg21, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg22, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg23, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg24, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg25, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg26, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg27, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg28, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg29, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg30);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4159)]
        //double SumIf([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x415a)]
        //double CountIf([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x415b)]
        //double CountBlank([In, MarshalAs(UnmanagedType.Interface)] Range Arg1);

        //[return: MarshalAs(UnmanagedType.BStr)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4168)]
        //string Phonetic([In, MarshalAs(UnmanagedType.Interface)] Range Arg1);

        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40eb)]
        //object DGet([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3);

        //Frequency function not supported according to docs
        //Greg: We could implement Frequency as:
        // int Frequency(Set x, double y) and it would return how many times y matched the evaluated value of each tuple in x
        //but would that be any better than Count(Filter({set},[Measures].[x]=99))?
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40fc)]
        //object Frequency([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);

        //not supported according to docs
        //Greg: I don't understand the math behind this function, but we can certainly implement it as:
        // double MDeterm(Set x)
        //same goes for other matrix functions
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40a3)]
        //double MDeterm([In, MarshalAs(UnmanagedType.Struct)] object Arg1);
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40a4)]
        //object MInverse([In, MarshalAs(UnmanagedType.Struct)] object Arg1);
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40a5)]
        //object MMult([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2);

        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40bd)]
        //double DProduct([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3);

        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40c3)]
        //double DStDevP([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40c4)]
        //double DVarP([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3);

        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40c7)]
        //double DCountA([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3);

        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x40d8)]
        //double Rank([In] double Arg1, [In, MarshalAs(UnmanagedType.Interface)] Range Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3);

        //not supported according to docs
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4065)]
        //object HLookup([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4);
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4066)]
        //object VLookup([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4);

        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4031)]
        //object LinEst([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4);
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4032)]
        //object Trend([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4);
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4033)]
        //object LogEst([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4);

        //not supported according to docs
        //we might be able to implement this
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4034)]
        //object Growth([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4);

        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4028)]
        //double DCount([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x4029)]
        //double DSum([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x402a)]
        //double DAverage([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x402b)]
        //double DMin([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x402c)]
        //double DMax([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x402d)]
        //double DStDev([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3);

        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x402f)]
        //double DVar([In, MarshalAs(UnmanagedType.Interface)] Range Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, MarshalAs(UnmanagedType.Struct)] object Arg3);

        //not supported according to docs
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x401c)]
        //object Lookup([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In, MarshalAs(UnmanagedType.Struct)] object Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3);
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x401d)]
        //object Index([In, MarshalAs(UnmanagedType.Struct)] object Arg1, [In] double Arg2, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg3, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Arg4);

        #endregion
    }

}