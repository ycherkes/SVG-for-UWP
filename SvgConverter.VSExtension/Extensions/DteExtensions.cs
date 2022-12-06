using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;

namespace SvgForUWPConverter.Extensions
{
    static class DteExtensions
    {
        public static string GetSolutionPath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var solution = ((DTE2)ServiceProvider.GlobalProvider.GetService(typeof(DTE)))?.Solution;
                if (!string.IsNullOrEmpty(solution?.FullName))
                {
                    return Path.GetDirectoryName(solution.FullName);
                }
            }
            catch (Exception) { }

            return null;
        }
    }
}
