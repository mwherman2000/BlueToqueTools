using BlueToqueTools.didlang;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlueToqueTools.didlang
{
    public class didlangProgram
    {
        static int nlines = 0;
        static int nstmts = 0;

        public static void Main(string[] args)
        {
            bool exiting = false;

            Help.Welcome();
            DidColorMethodSimulator simulator = new DidColorMethodSimulator();

            string? stmt = getStatement();
            while (!String.IsNullOrEmpty(stmt))
            {
                nstmts++;
                Console.WriteLine("<: " + stmt);
                switch (stmt.ToLower())
                {
                    case "h":
                    case "help": { Help.CommandHelp(); break; }
                    case "cls":
                    case "clear": { Help.Welcome(); break; }
                    case "exit":
                    case "bye": { exiting = true; break; }
                    default:
                    {
                            if (stmt.StartsWith('!'))
                            {
                                switch (stmt.Substring(1))
                                {
                                    case "0": { stmt = "did:example:1234"; processStatement(nlines, stmt); break; }
                                    case "q": { stmt = "did:example:1234?format=xml"; processStatement(nlines, stmt); break; }
                                    case "r": { stmt = "did:color:red"; processStatement(nlines, stmt); break; }
                                    case "1": { stmt = "*did:color:red"; processStatement(nlines, stmt); break; }
                                    case "2": { stmt = "**did:color:red"; processStatement(nlines, stmt); break; }
                                    case "3": { stmt = "***did:color:red"; processStatement(nlines, stmt); break; }
                                    case "red": { stmt = "***did:color:red"; processStatement(nlines, stmt); break; }
                                    case "green": { stmt = "***did:color:green"; processStatement(nlines, stmt); break; }
                                    case "blue": { stmt = "***did:color:blue"; processStatement(nlines, stmt); break; }
                                    case "colors": { DidColorMethodSimulator.DumpValues(); break; }
                                    case "a": { stmt = "**(agentTypeA^idA)*did:color:red"; processStatement(nlines,stmt); break; }
                                    case "aa": { stmt = "**(^idAA)*did:color:red"; processStatement(nlines, stmt); break; }
                                    case "aaa": { stmt = "**(agentTypeAAA)*did:color:red"; processStatement(nlines, stmt); break; }
                                    case "b": { stmt = "*(agentTypeB^idB)*did:color:red"; processStatement(nlines, stmt); break; }
                                    case "bb": { stmt = "*(:idBB)*did:color:red"; processStatement(nlines, stmt); break; }
                                    case "bbb": { stmt = "*(agentTypeBBB)*did:color:red"; processStatement(nlines, stmt); break; }
                                    case "c": { stmt = "*(agentInterfaceC^agentMethodC)**did:color:red"; processStatement(nlines, stmt); break; }
                                    case "cc": { stmt = "*(^agentMethodCC)**did:color:red"; processStatement(nlines, stmt); break; }
                                    case "ccc": { stmt = "*(agentInterfaceCCC)**did:color:red"; processStatement(nlines, stmt); break; }
                                    case "d": { stmt = "*(agentInterfaceD^agentMethodD)*(agentTypeD^idD)*did:color:red"; processStatement(nlines, stmt); break; }
                                    case "dd": { stmt = "*(^agentMethodDD)*(^idDD)*did:color:red"; processStatement(nlines, stmt); break; }
                                    case "ddd": { stmt = "*(agentInterfaceDDD)*(agentTypeDDD)*did:color:red"; processStatement(nlines, stmt); break; }
                                    case "e": { stmt = "**(^did:color:red#agentcluster2)*did:color:red"; processStatement(nlines, stmt); break; }
                                    case "h":
                                    case "help": { Help.ShortcutsHelp(); break; }
                                    default:
                                        {
                                            Console.WriteLine("!> unknown command shortcut: " + stmt);
                                            Help.ShortcutsHelp();
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                processStatement(nlines, stmt);
                            }
                        break;
                    }
                }

                if (exiting) break;

                stmt = getStatement();
            }
            Console.WriteLine("Done. " + nlines.ToString() + " lines. " + nstmts.ToString() + " statements. ");
        }

        internal static string? getStatement()
        {
            string? stmt = String.Empty;
            string? line = String.Empty;

            Console.Write("didlang> ");
            line = Console.ReadLine();
            nlines++;
            while (!String.IsNullOrEmpty(line) && line.EndsWith("_"))
            {
                stmt += line.Substring(0, line.Length - 1);
                Console.Write(nlines.ToString() + "> ");
                line = Console.ReadLine();
                nlines++;
            }
            if (String.IsNullOrEmpty(line))
            {
                nlines--;
            }
            else
            {
                stmt += line; 
            }

            return stmt;
        }

        internal static void processStatement(int nlines, string stmt)
        {
            DateTime start = DateTime.Now; 
            Console.WriteLine(nlines.ToString() + "> Start: " + start.ToString());
            ParseTree? parseTree = ParseTree.Parse(stmt);
            if (parseTree.parsingState != DidParsingState.Uninitialized)
            {
                parseTree = ParseTree.Validate(parseTree);
                if (parseTree.WasParsingSuccessful()) parseTree = ParseTree.Execute(parseTree);
            }
            DateTime end = DateTime.Now;
            Console.WriteLine(nlines.ToString() + "> Done: " + end.ToString() + "\t" + (end-start).TotalSeconds.ToString() + " seconds");
            Console.WriteLine();
        }
    }
}
