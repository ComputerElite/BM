﻿using System;
using System.Text.RegularExpressions;

namespace ComputerUtils.RegxTemplates
{
    public class RegexTemplates
    {
        public static bool IsIP(String input)
        {
            return Regex.IsMatch(input, "((2(5[0-5]|[0-4][0-9])|1?[0-9]?[0-9])\\.){3}(2(5[0-5]|[0-4][0-9])|1?[0-9]?[0-9])");
        }

        public static String GetIP(String input)
        {
            Match found = Regex.Match(input, "((2(5[0-5]|[0-4][0-9])|1?[0-9]?[0-9])\\.){3}(2(5[0-5]|[0-4][0-9])|1?[0-9]?[0-9])");
            if (!found.Success) return "";
            return found.Value;
        }

        public static String ReplaceUserName(String input, String replacement)
        {
            return Regex.Replace(input, @"([A-Z]{1}\:\\[Uu]sers\\)([^\\]*\\)(.*)", "$1$3");
        }
    }
}