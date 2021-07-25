using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Crudify.Utils
{
    internal class Options
    {
        [Option('a', "area", Required = true, HelpText = "Area Name.")]
        public string Area { get; set; }

        [Option('m', "model", Required = true, HelpText = "Model Name.")]
        public string Model { get; set; }

        [Usage(ApplicationAlias = "crud")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>
                {
                    new Example("create crud by specifying area and model name", new Options { Area = "TestArea", Model = "TestModel" })
                };
            }
        }
    }
}