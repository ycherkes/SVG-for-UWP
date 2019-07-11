using System;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace SvgForUWPConverter.Extensions
{
    static class DteExtensions
    {
        public static string GetSolutionPath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var solution = ((DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE))).Solution;
                if (!string.IsNullOrEmpty(solution.FullName))
                {
                    return Path.GetDirectoryName(solution.FullName);
                }
            }
            catch (Exception) { }

            return null;
        }
    }
}
