using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Dom.Css;
using AngleSharp.Extensions;
using AngleSharp.Parser.Css;
using AngleSharp.Parser.Xml;
using SvgConverterCore.VieModels;


namespace SvgConverterCore.Utils
{
    public class SvgConverter
    {
        public static async Task Convert(SvgUpdatingViewModelBase model, Action<string> addFileToProject)
        {
            if(!Directory.Exists(model.InputFolder))
                throw new FileNotFoundException($"Directory {model.InputFolder} not found!");

            if (!Directory.Exists(model.OutputFolder))
            {
                Directory.CreateDirectory(model.OutputFolder);
            }

            var svgFiles = new DirectoryInfo(model.InputFolder).GetFiles("*.svg");

            if(!svgFiles.Any())
                throw new FileNotFoundException("SVG files not found!");

            foreach (var file in svgFiles)
            {
                await CreateInlinedSvg(model.RemoveInlinedStyles, file.FullName, model.OutputFolder, addFileToProject, model.OverwriteExistingFiles);
            }
        }

        public static async Task ConvertFile(string filePath)
        {
            var fileDirectory = Path.GetDirectoryName(filePath);

            await CreateInlinedSvg(false, filePath, fileDirectory, s => { }, true);
        }

        private static async Task CreateInlinedSvg(bool removeStyleElements, string fileName, string patchedDirectory, Action<string> addFileToProject, bool overwrite)
        {
            var xmlSource = File.ReadAllText(fileName);

            var document = new XmlParser().Parse(xmlSource);
            var cssParser = new CssParser();
            var cssSourceNodes = document.QuerySelectorAll("style");

            RemoveStyles(removeStyleElements, cssSourceNodes);

            var cssBlocks = cssSourceNodes.Select(x => x.InnerHtml);
            var stylesToInline = cssBlocks.Select(x => cssParser.ParseStylesheet(x)).ToArray();

            var elementsWithStyles = GetElementsWithStyles(stylesToInline, document);

            ApplyStyles(elementsWithStyles);

            var html = document.ToHtml();

            var patchedFileName = Path.Combine(patchedDirectory, Path.GetFileName(fileName));

            await WriteTextAsync(patchedFileName, html, overwrite);

            addFileToProject(patchedFileName);
        }

        private static async Task WriteTextAsync(string filePath, string text, bool overwrite)
        {
            var encodedText = Encoding.ASCII.GetBytes(text);

            using (var sourceStream = new FileStream(filePath, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                await sourceStream.FlushAsync();
            }
        }

        private static void ApplyStyles(Dictionary<IElement, List<ICssStyleRule>> elementsWithStyles)
        {
            foreach (var elementsWithStyle in elementsWithStyles)
            {
                var mergedStyles = string.Join("; ", elementsWithStyle.Value.Select(x => Regex.Match(x.SourceCode.Text, @"\{(.*);*\}").Groups[1].Value.Replace(":", ": ")));
                elementsWithStyle.Key.SetAttribute("style", mergedStyles);
            }
        }

        private static void RemoveStyles(bool removeStyleElements, IEnumerable<IElement> cssSourceNodes)
        {
            if (!removeStyleElements) return;

            foreach (var cssSourceNode in cssSourceNodes.ToArray())
            {
                cssSourceNode.Remove();
            }
        }

        private static Dictionary<IElement, List<ICssStyleRule>> GetElementsWithStyles(IEnumerable<ICssStyleSheet> stylesToInline, IParentNode document)
        {
            var elementsWithStyles = new Dictionary<IElement, List<ICssStyleRule>>();

            foreach (var style in stylesToInline.SelectMany(x => x.Children).OfType<ICssStyleRule>())
            {
                var elementsForSelector = document.QuerySelectorAll(style.Selector.Text);

                foreach (var el in elementsForSelector)
                {
                    var existing = elementsWithStyles.ContainsKey(el) ? elementsWithStyles[el] : new List<ICssStyleRule>();
                    existing.Add(style);
                    elementsWithStyles[el] = existing;
                }
            }

            return elementsWithStyles;
        }
    }
}
