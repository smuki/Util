using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Security.Principal;

namespace Volte.Utils
{
    public class Util {

        const string ZFILE_NAME = "CoreUtil";

        public static string ReplaceWith(string original, string pattern, string replacement)
        {
            int count;
            int position0;
            int position1;

            count               = position0 = position1 = 0;
            string upperString  = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc             = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
            char [] chars       = new char[original.Length + Math.Max(0, inc)];

            while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1) {
                for (int i = position0 ; i < position1 ; ++i) {
                    chars[count++] = original[i];
                }

                for (int i = 0 ; i < replacement.Length ; ++i) {
                    chars[count++] = replacement[i];
                }

                position0 = position1 + pattern.Length;
            }

            if (position0 == 0) {
                return original;
            }

            for (int i = position0 ; i < original.Length ; ++i) {
                chars[count++] = original[i];
            }

            return new string(chars, 0, count);
        }

        public static string DataTypeChar(string datatype)
        {
            string typechar = "c";

            switch (datatype.Trim().ToLower()) {
            case "boolean":
                typechar = "l";
                break;

            case "datetime":
                typechar = "d";
                break;

            case "float":
            case "decimal":
                typechar = "n";
                break;

            case "int":
                typechar = "i";
                break;

            case "image":
                typechar = "b";
                break;

            case "ntext":
            case "picture":
            case "nvarchar":
                typechar = "c";
                break;

            case "caption":
            case "label":
                typechar = "x";
                break;
            }

            return typechar;
        }

        public static void CreateDir(string sPathName)
        {
            try {
                if (!Directory.Exists(sPathName)) {
                    Directory.CreateDirectory(sPathName);
                }
            } catch (Exception ex) {
            }
        }

        public static void WriteContents(string sFilePath, string sContents, bool bAppend = true)
        {
            FileStream file = null;
            StreamWriter sw = null;

            try {

                string _DirName = System.IO.Path.GetDirectoryName(sFilePath);

                if (_DirName != "") {
                    if (!Directory.Exists(_DirName)) {
                        Directory.CreateDirectory(_DirName);
                    }
                }

                if (bAppend == false) {
                    if (File.Exists(sFilePath)) {
                        File.Delete(sFilePath);
                    }
                }

                // Specify file, instructions, and privelegdes
                if (bAppend) {
                    if (File.Exists(sFilePath)) {
                        file = new FileStream(sFilePath, FileMode.Append , FileAccess.Write);
                    } else {
                        file = new FileStream(sFilePath, FileMode.Create, FileAccess.Write);
                    }
                } else {
                    file = new FileStream(sFilePath, FileMode.Create, FileAccess.Write);
                }

                UTF8Encoding _UTF8Encoding = new UTF8Encoding(false, true);

                sw = new StreamWriter(file, _UTF8Encoding);

                // Write a string to the file
                sw.Write(sContents);
            } catch (Exception ex) {
                throw new Exception("WriteFileContents() failed with error: " + ex.Message);
            } finally {
                // Close StreamWriter
                if (sw != null) {
                    sw.Close();
                }

                // Close file
                if (file != null) {
                    file.Close();
                }
            }
        }
    }
}
