using CommandLine;
using Crudify.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColorfulConsole = Colorful.Console;

namespace Crudify
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            _ = Parser.Default.ParseArguments<Options>(args).WithParsed(option =>
              {
                  string currentDir = Directory.GetCurrentDirectory();

                  string errorMsg = CheckRequiredFolders(currentDir, option.Area, option.Model);

                  if (errorMsg != null)
                  {
                      Console.ForegroundColor = ConsoleColor.Red;
                      Console.WriteLine(errorMsg);
                      Console.ResetColor();
                      return;
                  }

                  if (!AskForConfirmation(option.Area, option.Model))
                      return;

                  Console.Write("Generating files...");

                  string templateBaseDir = $"{AppContext.BaseDirectory}\\Templates";
                  string destinationBaseDir = $"{currentDir}\\Areas\\{option.Area}";
                  var templateParams = GetTemplateParams(currentDir, option.Area, option.Model);

                  GenerateFile($"{templateBaseDir}\\Model.txt", $"{destinationBaseDir}\\Models\\{option.Model}.cs", templateParams);

                  GenerateFile($"{templateBaseDir}\\ApiController.txt", $"{destinationBaseDir}\\{templateParams["@ApiController@"]}\\{option.Model}Controller.cs", templateParams);

                  GenerateFile($"{templateBaseDir}\\Controller.txt", $"{destinationBaseDir}\\Controllers\\{option.Model}Controller.cs", templateParams);

                  ShowSuccessMsg(currentDir, option.Area, option.Model);
              });
        }

        private static string GetApiControllerDirName(string currentDir, string area)
        {
            string apiController = Array.Find(Directory.GetDirectories($"{currentDir}\\Areas\\{area}"), dir =>
            {
                dir = Path.GetFileName(dir).ToLower();
                return dir.Contains("api");
            });

            return Path.GetFileName(apiController);
        }

        private static string CheckRequiredFolders(string currentDir, string area, string model)
        {
            if (!Directory.Exists($"{currentDir}\\Areas"))
                return "'Areas' folder is missing in this directory. Make sure you're in the root directory of project.";

            if (!Directory.Exists($"{currentDir}\\Areas\\{area}"))
                return $"'{area}' area does not exist. Create an area called '{area}' from your IDE.";

            var areaDirs = Directory.GetDirectories($"{currentDir}\\Areas\\{area}").Where(dir =>
            {
                dir = Path.GetFileName(dir).ToLower();

                return dir == "controllers" || dir == "apicontroller" ||
                       dir == "apicontrollers" || dir == "models";
            });

            if (areaDirs.Count() < 3)
                return $"Required folder(s) missing in '{area}' area. Make sure following folders are present in '{area}' area, Controllers, ApiControllers and Models.";

            if (File.Exists($"{currentDir}\\Areas\\{area}\\Models\\{model}.cs"))
                return $"There's already a file named '{model}.cs'. '/{area}/Models/{model}.cs'";

            if (File.Exists($"{currentDir}\\Areas\\{area}\\{GetApiControllerDirName(currentDir, area)}\\{model}Controller.cs"))
                return $"There's already a file named '{model}Controller.cs'. '/{area}/{GetApiControllerDirName(currentDir, area)}/{model}Controller.cs'";

            if (File.Exists($"{currentDir}\\Areas\\{area}\\Controllers\\{model}Controller.cs"))
                return $"There's already a file named '{model}Controller.cs' in '/{area}/Controllers/{model}Controller.cs'";

            return null;
        }

        private static bool AskForConfirmation(string area, string model)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Area : {area}\nModel: {model}");

            bool confirm = false;

            while (true)
            {
                Console.Write("[?] Confirm area and model name [y/n]: (y) ");
                string response = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(response) || string.Equals(response.Trim(), "y", StringComparison.OrdinalIgnoreCase))
                {
                    confirm = true;
                    break;
                }
                else if (string.Equals(response.Trim(), "n", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            Console.ResetColor();
            return confirm;
        }

        private static Dictionary<string, string> GetTemplateParams(string currentDir, string area, string model)
        {
            return new Dictionary<string, string>
            {
                { "@App@", Path.GetFileName(currentDir) },
                { "@Area@", area },
                { "@ApiController@", GetApiControllerDirName(currentDir, area) },
                { "@Model@", model },
            };
        }

        private static void GenerateFile(string templatePath, string destinationPath, Dictionary<string, string> templateParams)
        {
            string templateContent = File.ReadAllText(templatePath);

            foreach (var param in templateParams)
            {
                templateContent = templateContent.Replace(param.Key, param.Value);
            }

            File.WriteAllText(destinationPath, templateContent);
        }

        private static void ShowSuccessMsg(string currentDir, string area, string model)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("\n3 files generated:");
            Console.WriteLine($"/{area}/Controllers/{model}Controller.cs");
            Console.WriteLine($"/{area}/{GetApiControllerDirName(currentDir, area)}/{model}Controller.cs");
            Console.WriteLine($"/{area}/Models/{model}.cs");

            Console.ForegroundColor = ConsoleColor.Blue;
            ColorfulConsole.WriteAscii("CRUDIFY");

            Console.ResetColor();
        }
    }
}