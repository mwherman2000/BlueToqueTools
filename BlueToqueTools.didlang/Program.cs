using BlueToqueTools.didlang;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlueToqueTools.didlang
{
    public class didlangProgram
    {
        static DidColorMethodSimulator didcolorMethodSimulator = null;
        static System.Reflection.Assembly assembly = typeof(didlangProgram).Assembly;

        static int nlines = 0;
        static int nstmts = 0;

        public static void Main(string[] args)
        {
            bool exiting = false;

            Welcome();
            string? stmt = getStatement();
            while (!String.IsNullOrEmpty(stmt))
            {
                nstmts++;
                Console.WriteLine("<: " + stmt);
                switch (stmt.ToLower())
                {
                    case "h":
                    case "help": { Help(); break; }
                    case "cls":
                    case "clear": { Welcome(); break; }
                    case "exit":
                    case "bye": { exiting = true; break; }
                    default:
                    {
                        ParseTree? parseTree = parse(stmt);
                        if (parseTree.parsingState != DidParsingState.Uninitialized)
                        {
                            validate(parseTree);
                            execute(parseTree);
                        }

                        Console.WriteLine("== " + DateTime.Now.ToString());
                        Console.WriteLine();
                        break;
                    }
                }

                if (exiting) break;

                stmt = getStatement();
            }
            Console.WriteLine("Done. " + nlines.ToString() + " lines. " + nstmts.ToString() + " statements. ");
        }

        private static void Welcome()
        {
            Console.Clear();
            Console.WriteLine("didlang Language Command Line Interpreter for DID Identifiers, DID Documents, DID Agents, and DID Objects");
            Console.WriteLine("== Version: " + assembly.FullName.ToString());
            Console.WriteLine("== Loading: " + DidColorMethodSimulator.name + " version " + DidColorMethodSimulator.version);
            didcolorMethodSimulator = new DidColorMethodSimulator();
            Console.WriteLine("== " + DateTime.Now.ToString());
            Console.WriteLine();
        }

        internal static string? getStatement()
        {
            string? stmt = String.Empty;
            string? line = String.Empty;

            Console.Write(nlines.ToString() + "> ");
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

        internal static ParseTree? parse(string stmt)
        {
            if (String.IsNullOrEmpty(stmt)) throw new ArgumentNullException(nameof(stmt));

            ParseTree? parseTree = new ParseTree();

            if (stmt.StartsWith('!'))
            {
                switch (stmt.Substring(1))
                {
                    case "0": { stmt = "did:example:1234"; break; }
                    case "r": { stmt = "did:color:red"; break; }
                    case "1": { stmt = "*did:color:red"; break; }
                    case "2": { stmt = "**did:color:red"; break; }
                    case "3": { stmt = "***did:color:red"; break; }
                    case "red": { stmt = "***did:color:red"; break; }
                    case "green": { stmt = "***did:color:green"; break; }
                    case "blue": { stmt = "***did:color:blue"; break; }
                    case "colors": { DidColorMethodSimulator.DumpValues(); return new ParseTree(); }
                    case "h":
                    case "help": { ShortcutsHelp(); return new ParseTree(); }
                    default:
                        {
                            Console.WriteLine("!> unknown command shortcut: " + stmt);
                            ShortcutsHelp();
                            return new ParseTree();
                        }
                }

                ConsoleColor cbgc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("!> " + stmt);
                Console.ForegroundColor = cbgc;
            }

            parseTree.parsingState = DidParsingState.Starting;
            parseTree = parseRecursive(parseTree, stmt);

            return parseTree;
        }

        internal static void ShortcutsHelp()
        {
            Console.WriteLine("!0      = did:example:1234");
            Console.WriteLine("!r      = did:color:red");
            Console.WriteLine("!1      = *did:color:red");
            Console.WriteLine("!2      = **did:color:red");
            Console.WriteLine("!3      = ***did:color:red");
            Console.WriteLine("!red    = ***did:example:red");
            Console.WriteLine("!green  = ***did:example:green");
            Console.WriteLine("!blue   = ***did:example:blue");
            Console.WriteLine("!colors = display a list of the registered did:color DID Objects");
            Console.WriteLine("!help   = redisplay this list of command shortcuts");
            Console.WriteLine("help    = display a list of top-level commands");
        }

        internal static void Help()
        {
            Console.WriteLine("Enter <did> to verify a DID Identifier (\"no indirection\").");
            Console.WriteLine("Enter *<did> to return the DID Document associated with a DID Identifier (\"single indirection\").");
            Console.WriteLine("Enter **<did> to return the Agent Scred (VC) associated with a DID Identifier (\"double indirection\").");
            Console.WriteLine("Enter ***<did> to return the Object Scred (VC) associated with a DID Identifier (\"triple indirection\").");
            Console.WriteLine("Enter help to redisplay this list of commands.");
            Console.WriteLine("Enter !help to see a list of command shortcuts.");
        }

        internal static ParseTree? parseRecursive(ParseTree? parseTree, string stmt)
        {
            if (parseTree == null) throw new ArgumentNullException(nameof(parseTree));  
            if (String.IsNullOrEmpty(stmt)) throw new ArgumentNullException(nameof(stmt));

            parseTree.parsingState = DidParsingState.Started;

            if (stmt.StartsWith("***")) // triple-indirection - return DID Object via Agent via DID Document
            {
                parseTree.ifDidDocIndirect = true;
                parseTree.ifDidAgentDocIndirect = true;
                parseTree.ifDidObjectIndirect = true;
                parseRecursive(parseTree, stmt.Substring(2));
            }
            if (stmt.StartsWith("**")) // double-indirection - return Agent Document via DOD Document
            {
                parseTree.ifDidDocIndirect = true;
                parseTree.ifDidAgentDocIndirect = true;
                parseRecursive(parseTree, stmt.Substring(2));
            }
            else if (stmt.StartsWith("*")) // single-indirection - return DID Document
            {
                parseTree.ifDidDocIndirect = true;
                parseRecursive(parseTree, stmt.Substring(1));
            }
            else if (stmt.StartsWith("did:")) // no indirection - verify DID and Security Level
            {
                parseTree.didIdentifier = stmt;
                string[] didIdentifierParts = parseTree.didIdentifier.Split(':');
                if (didIdentifierParts.Length != 3)
                {
                    parseTree.parsingState = DidParsingState.Failed_BadDidIdentifier;
                }
                else
                {
                    parseTree.didTopLevelName = didIdentifierParts[0];
                    parseTree.didMethodName = didIdentifierParts[1];  
                    parseTree.didIdString = didIdentifierParts[2];
                    parseTree.parsingState = DidParsingState.Successful;
                }
            }
            else
            {
                parseTree.parsingState = DidParsingState.Failed;
            }

            return parseTree;
        }

        internal static ParseTree? validate(ParseTree? parseTree)
        {
            if (parseTree == null)
            {
                Console.WriteLine("parseTree: null");
                return new ParseTree();
            }

            if (!parseTree.WasSuccessful())
            {
                Console.WriteLine("v> parsing not successful");
                return new ParseTree();
            }

            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.IncludeFields = true;
            jsonOptions.WriteIndented = true;
            var json = JsonSerializer.Serialize<ParseTree>(parseTree, jsonOptions);
            Console.WriteLine("v> " + json);

            parseTree = DidColorMethodSimulator.VerifyDid(parseTree);
            if (parseTree.wasDidVerifiedTrue)
            {
                Console.WriteLine("v> " + parseTree.didIdentifier + " verified True");
            }
            else
            {
                Console.WriteLine("v> " + parseTree.didIdentifier + " verified False");
            }

            return parseTree;
        }

        internal static ParseTree? execute(ParseTree? parseTree)
        {
            string didDocument = String.Empty;
            string agentScred = String.Empty;
            string objectScred = String.Empty;   

            if (parseTree == null) throw new ArgumentNullException(nameof(parseTree));

            if (!parseTree.WasSuccessful())
            {
                Console.WriteLine("e> parsing not successful");
                return parseTree;
            }

            parseTree = DidColorMethodSimulator.VerifyDid(parseTree);
            if (!parseTree.wasDidVerifiedTrue)
            {
                Console.WriteLine("e> DID did not verify True");
                return parseTree;
            }

            if (parseTree.ifDidDocIndirect && parseTree.ifDidAgentDocIndirect && parseTree.ifDidObjectIndirect) // return DID Object SCred
            {
                didDocument = DidColorMethodSimulator.GetDidDocument(parseTree);
                agentScred = DidColorMethodSimulator.GetAgentScred(parseTree, didDocument);
                objectScred = DidColorMethodSimulator.GetObjectScred(parseTree, agentScred);
                parseTree.didDocument = didDocument;
                parseTree.didAgentScred = agentScred;
                parseTree.didObjectScred = objectScred;
                parseTree.DumpDidDocument();
                parseTree.DumpAgentScred();
                parseTree.DumpObjectScred();
            }
            else if (parseTree.ifDidDocIndirect && parseTree.ifDidAgentDocIndirect && !parseTree.ifDidObjectIndirect) // return Agent Scred
            {
                didDocument = DidColorMethodSimulator.GetDidDocument(parseTree);
                agentScred = DidColorMethodSimulator.GetAgentScred(parseTree, didDocument);
                parseTree.didDocument = didDocument;
                parseTree.didAgentScred = agentScred;
                parseTree.DumpDidDocument();
                parseTree.DumpAgentScred();
            }
            else if (parseTree.ifDidDocIndirect && !parseTree.ifDidAgentDocIndirect && !parseTree.ifDidObjectIndirect) // return DID Document
            {
                didDocument = DidColorMethodSimulator.GetDidDocument(parseTree);
                parseTree.didDocument = didDocument;
                parseTree.DumpDidDocument();
            }
            else if (!parseTree.ifDidDocIndirect && !parseTree.ifDidAgentDocIndirect && !parseTree.ifDidObjectIndirect) // return DID verification test
            {
                parseTree = DidColorMethodSimulator.VerifyDid(parseTree); // redundant
            }

            return parseTree;
        }
    }
}
