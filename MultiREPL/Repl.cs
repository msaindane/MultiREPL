using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace MultiREPL
{
    class REPL
    {
        private ScriptRuntime dlrRuntime;
        private ScriptEngine dlrEngine;
        private ScriptScope dlrScope;
        private string lang;
        private ExceptionOperations eo;

        /// <summary>
        /// REPL Constructor
        /// </summary>
        public REPL()
        {
            ScriptRuntimeSetup setup = new ScriptRuntimeSetup();
            setup.LanguageSetups.Add(IronRuby.Ruby.CreateRubySetup());
            setup.LanguageSetups.Add(IronPython.Hosting.Python.CreateLanguageSetup(null));
            dlrRuntime = new ScriptRuntime(setup);
            dlrEngine = dlrRuntime.GetEngine("IronRuby");
            dlrScope = dlrEngine.CreateScope();
            eo = dlrEngine.GetService<ExceptionOperations>();
            lang = "rb";
        }

        /// <summary>
        /// Method which executes commands and code
        /// </summary>
        public void Run()
        {
            while (true)
            {
                Console.Write("{0}> ", lang);
                string line = Console.ReadLine();
                if (line == null)
                    break;
                if (line[0] == 35) // Look for the '#' character
                {
                    ExecuteCommand(line.Substring(1).TrimEnd());
                }
                else
                {
                    ExecuteCode(ReadCode(line));
                }
            }
        }

        /// <summary>
        /// Read single and multi-line code
        /// </summary>
        /// <param name="firstLine">code to be executed</param>
        /// <returns>Returns a ScriptSource object</returns>
        public ScriptSource ReadCode(string firstLine)
        {
            StringBuilder code = new StringBuilder("");
            code.AppendLine(firstLine);

            while (true)
            {
                ScriptSource interactiveCode = dlrEngine.CreateScriptSourceFromString(code.ToString(), SourceCodeKind.InteractiveCode);
                switch (interactiveCode.GetCodeProperties())
                {
                    case ScriptCodeParseResult.Complete:
                        return interactiveCode;
                    case ScriptCodeParseResult.Invalid:
                        return interactiveCode;
                    default:
                        {
                            Console.Write("{0}| ", lang);
                            string nextLine = Console.ReadLine();
                            if (nextLine == null || nextLine.Trim().Length == 0)
                            {
                                return interactiveCode;
                            }
                            code.AppendLine(nextLine);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Executes the source code typed in the REPL window
        /// </summary>
        /// <param name="source">source code to be executed</param>
        public void ExecuteCode(ScriptSource source)
        {
            try
            {
                source.Execute(dlrScope);
            }
            catch (Exception ex)
            {
                string error = eo.FormatException(ex);
                Console.WriteLine(error);
            }
        }


        /// <summary>
        /// Executes the #commands passed in the REPL window
        /// </summary>
        /// <param name="command">command to be executed</param>
        public void ExecuteCommand(string command)
        {
            if (command == "help")
            {
                Console.WriteLine("  Available commands:");
                Console.WriteLine("\t#ls\tLists all Languages supported.");
                Console.WriteLine("\t#rb\tChange language to Ruby.");
                Console.WriteLine("\t#py\tChange language to Python.");
                Console.WriteLine("\t#exit\tExits the application.");
                Console.WriteLine("\t#help\tThis cruft");
            }
            else if (command == "exit")
            {
                Environment.Exit(0);
            }
            else if (command == "ls")
            {
                DisplayLanguages();
            }
            else
            {
                if (!SwitchLanguage(command))
                {
                    throw new ArgumentException(String.Format("Unknown language name: '{0}'", command));
                }
            }
        }

        /// <summary>
        /// Displays all the languages supported in this REPL
        /// </summary>
        public void DisplayLanguages()
        {
            IList<LanguageSetup> set = dlrEngine.Runtime.Setup.LanguageSetups;
            foreach (LanguageSetup ls in set)
            {
                Console.WriteLine(ls.DisplayName);
            }
        }

        /// <summary>
        /// Switch the current language
        /// </summary>
        /// <param name="name">A string value 'py' or 'rb'</param>
        /// <returns></returns>
        public bool SwitchLanguage(string name)
        {
            bool has_lang;
            has_lang = dlrEngine.Runtime.TryGetEngine(name, out dlrEngine);
            if (has_lang)
                lang = name;
            return has_lang;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Type #help to know more.");
            REPL rr = new REPL();
            rr.Run();
        }
    }
}
