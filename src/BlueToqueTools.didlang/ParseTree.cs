using BlueToqueTools.didlang;
using System.Text.Json;

namespace BlueToqueTools.didlang
{
    public enum DidTrustLevel
    {
        Level0,
        Level1,
        Level2,
        Level3,
        Level4
    }

    public enum DidDocResolutionState
    {
        NotAttempted,
        Found,
        Returned,
        Verified,
        Disposed
    }

    public enum DidParsingState
    {
        Uninitialized,
        Starting,
        Started,
        Successful,
        Failed,
        Failed_BadDidIdentifier,
        Failed_BadDidAgentServiceEndpointCoercion,
        Failed_BadDidAgentInterfaceMethodCoercion,
        Failed_BadQueryString
    }

    public class ParseTree
    {
        private const string SELECTOR_SEPARATOR = "^";

        // parsing state
        public DidParsingState parsingState = DidParsingState.Uninitialized;
        public string command = string.Empty;  

        // DID Command pre-actions
        public bool ifDidDocIndirect = false;
        public bool ifDidAgentIndirect = false;
        public bool ifDidObjectIndirect = false;
        public bool ifDidAgentServiceEndpointCoerced = false;
        public bool ifDidAgentInterfaceMethodCoerced = false;

        // DID Command stmt fields
        public string didIdentifier = String.Empty;
        public string didTopLevelName = String.Empty;
        public string didMethodName = String.Empty;
        public string didIdString = String.Empty;
        public string didQueryString = String.Empty;

        // DID Document
        public string didDocument = String.Empty;

        // DID Agent Service Endpoint
        public string didAgentServiceEndpointType = "BlueToque.Agent";   
        public string didAgentServiceEndpointId =   "#default";   
        public string didAgentServiceEndpointUri = String.Empty;
        public string didAgentScred = String.Empty;

        // DID Agent Interface (and Interface Method)
        public string didAgentInterface  = "BlueToque.ObjectAccessor";
        public string didAgentInterfaceMethod = "getObjectScred";      // default interface method when calling an Agent to return an object
        public string didAgentInterfaceMethodQueryString = String.Empty;   
        public string didObjectScred = String.Empty;

        // DID Command execution outputs
        public DidDocResolutionState didDocumentResolutionState = DidDocResolutionState.NotAttempted;
        public bool wasDidDocumentFound = false;
        public bool wasDidAgentScredFound = false;
        public bool wasDidAgentActionSuccessful = false; // default: getObjectInterfaces
        public bool wasDidVerified = false;
        public bool wasDidVerifiedTrue = false;
        public DidTrustLevel didTrustLevel = DidTrustLevel.Level0;

        public bool WasParsingSuccessful()
        {
            return parsingState == DidParsingState.Successful;
        }

        public static ParseTree? Parse(string stmt)
        {
            if (String.IsNullOrEmpty(stmt)) throw new ArgumentNullException(nameof(stmt));

            ParseTree? parseTree = new();

            ConsoleColor cbgc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("p> " + stmt);
            Console.ForegroundColor = cbgc;

            parseTree.parsingState = DidParsingState.Starting;
            parseTree.command = stmt;

            string[] stmtParts = stmt.Split('?');
            stmt = stmtParts[0];
            if (stmtParts.Length == 2) parseTree.didQueryString = stmtParts[1];
            if (stmtParts.Length > 2)
            {
                parseTree.parsingState = DidParsingState.Failed_BadQueryString;
            }
            else
            {
                parseTree = ParseRecursive(parseTree, stmt);
            }

            return parseTree;
        }

        internal static ParseTree? ParseRecursive(ParseTree? parseTree, string stmt)
        {
            if (parseTree == null) throw new ArgumentNullException(nameof(parseTree));

            if (String.IsNullOrEmpty(stmt)) return parseTree; // throw new ArgumentNullException(nameof(stmt));

            parseTree.parsingState = DidParsingState.Started;

            if (stmt.StartsWith("***")) // minimum: triple-indirection - return DID Object via Agent via DID Document - by implication, no coercion
            {
                // e.g. ***did:color:red
                parseTree.ifDidDocIndirect = true;
                parseTree.ifDidAgentIndirect = true;
                parseTree.ifDidObjectIndirect = true;
                parseTree = ParseRecursive(parseTree, stmt[3..]);
            }
            else if (stmt.StartsWith("*")) // minimum: single-indirection - return DID Document -OR- single or double coercion
            {
                // e.g. *did:color:red -OR-
                //                          !b X *(serviceEndpointType#serviceEndpointId)did  (NOT VALID - cannot coerce a DID Document)
                //                          !b = *(serviceEndpointType#serviceEndpointId)*did (single agent coercion/double indirection)
                //                          !c = *(agentInterface#agentMethod)**did           (single method coercion/triple indirection)
                //                          !d = *(agentInterface#agentMethod)*(serviceEndpointType#serviceEndpointId)*did (double coercion/triple indirection)
                string[] stmtParts = stmt.Split('*');
                switch (stmtParts.Length)
                {
                    case 1:
                        {
                            parseTree = ParseRecursive(parseTree, stmtParts[0]);
                            break;
                        }
                    case 2: // single indirection
                        {
                            parseTree.ifDidDocIndirect = true;
                            parseTree = ParseRecursive(parseTree, stmtParts[1]);
                            break;
                        }
                    case 3: // double indirection - !b
                        {
                            parseTree.ifDidDocIndirect = true;
                            parseTree.ifDidAgentIndirect = true;
                            parseTree = ParseAgentServiceEndpointCoercion(parseTree, stmtParts[1]);
                            parseTree = ParseRecursive(parseTree, stmtParts[2]);
                            break;
                        }
                    case 4: // triple indirection - !c or !d
                        {
                            parseTree.ifDidDocIndirect = true;
                            parseTree.ifDidAgentIndirect = true;
                            parseTree.ifDidObjectIndirect = true;
                            parseTree = ParseAgentServiceEndpointCoercion(parseTree, stmtParts[2]);
                            parseTree = ParseAgentInterfaceMethodCoercion(parseTree, stmtParts[1]);
                            parseTree = ParseRecursive(parseTree, stmtParts[3]);
                            break;
                        }
                    case 0:
                    default:
                        {
                            parseTree.parsingState = DidParsingState.Failed;
                            break;
                        }
                }
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

        private static ParseTree ParseAgentInterfaceMethodCoercion(ParseTree parseTree, string agentInterfaceMethodCoercion)
        {
            if (String.IsNullOrEmpty(agentInterfaceMethodCoercion)) return parseTree;

            parseTree.ifDidAgentInterfaceMethodCoerced = true;    
            Console.WriteLine("=> agentInterfaceMethodCoercion: " + agentInterfaceMethodCoercion);
            switch (agentInterfaceMethodCoercion[..1])
            {
                case "(":
                    {
                        string coercion = agentInterfaceMethodCoercion[1..];
                        int locCloseParen = coercion.IndexOf(')');
                        if (locCloseParen == -1) { parseTree.parsingState = DidParsingState.Failed_BadDidAgentInterfaceMethodCoercion; break; }
                        coercion = coercion[..locCloseParen];
                        string[] coercionParts = coercion.Split(SELECTOR_SEPARATOR);
                        switch (coercionParts.Length)
                        {
                            case 1:
                                {
                                    if (!String.IsNullOrEmpty(coercionParts[0])) parseTree.didAgentInterface = coercionParts[0];
                                    break;
                                }
                            case 2:
                                {
                                    if (!String.IsNullOrEmpty(coercionParts[0])) parseTree.didAgentInterface = coercionParts[0];
                                    if (!String.IsNullOrEmpty(coercionParts[1])) parseTree.didAgentInterfaceMethod = coercionParts[1];
                                    break;
                                }
                            case 0:
                            default:
                                {
                                    parseTree.parsingState = DidParsingState.Failed_BadDidAgentInterfaceMethodCoercion;
                                    break;
                                }
                        }
                        break;
                    }
                default: {
                        parseTree.parsingState = DidParsingState.Failed_BadDidAgentInterfaceMethodCoercion;
                        break; 
                    }
            }

            return parseTree;
        }

        private static ParseTree ParseAgentServiceEndpointCoercion(ParseTree parseTree, string agentServiceEndpointCoercion)
        {
            if (String.IsNullOrEmpty(agentServiceEndpointCoercion)) return parseTree;

            parseTree.ifDidAgentServiceEndpointCoerced = true;
            Console.WriteLine("=> agentServiceEndpointCoercion: " + agentServiceEndpointCoercion);
            switch (agentServiceEndpointCoercion[..1])
            {
                case "(":
                    {
                        string coercion = agentServiceEndpointCoercion[1..];
                        int locCloseParen = coercion.IndexOf(')');
                        if (locCloseParen == -1) { parseTree.parsingState = DidParsingState.Failed_BadDidAgentServiceEndpointCoercion; break; }
                        coercion = coercion[..locCloseParen];
                        string[] coercionParts = coercion.Split(SELECTOR_SEPARATOR);
                        switch (coercionParts.Length)
                        {
                            case 1:
                                {
                                    if (!String.IsNullOrEmpty(coercionParts[0])) parseTree.didAgentServiceEndpointType = coercionParts[0];
                                    parseTree.ifDidAgentServiceEndpointCoerced = true;
                                    break;
                                }
                            case 2:
                                {
                                    if (!String.IsNullOrEmpty(coercionParts[0])) parseTree.didAgentServiceEndpointType = coercionParts[0];
                                    if (!String.IsNullOrEmpty(coercionParts[1])) parseTree.didAgentServiceEndpointId = coercionParts[1];
                                    parseTree.ifDidAgentServiceEndpointCoerced = true;
                                    break;
                                }
                            case 0:
                            default:
                                {
                                    parseTree.parsingState = DidParsingState.Failed_BadDidAgentServiceEndpointCoercion;
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        parseTree.parsingState = DidParsingState.Failed_BadDidAgentServiceEndpointCoercion;
                        break;
                    }
            }

            return parseTree;
        }

        public static ParseTree? Validate(ParseTree? parseTree)
        {
            if (parseTree == null)
            {
                Console.WriteLine("parseTree: null");
                return new ParseTree();
            }

            parseTree.DumpParseTree("v");

            if (!parseTree.WasParsingSuccessful())
            {
                Console.WriteLine("v> parsing not successful");
                return new ParseTree();
            }

            parseTree = DidColorMethodSimulator.VerifyDidIdentifier(parseTree);
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

        public static ParseTree? Execute(ParseTree? parseTree)
        {
            string didDocument = String.Empty;
            string agentScred = String.Empty;
            string objectScred = String.Empty;

            if (parseTree == null) throw new ArgumentNullException(nameof(parseTree));

            if (!parseTree.WasParsingSuccessful())
            {
                Console.WriteLine("e> parsing not successful");
                return parseTree;
            }

            parseTree = DidColorMethodSimulator.VerifyDidIdentifier(parseTree);
            if (!parseTree.wasDidVerifiedTrue)
            {
                Console.WriteLine("e> DID did not verify True");
                return parseTree;
            }

            if (parseTree.ifDidDocIndirect && parseTree.ifDidAgentIndirect && parseTree.ifDidObjectIndirect) // return DID Object SCred
            {
                didDocument = DidColorMethodSimulator.GetDidDocument(parseTree);
                agentScred = DidColorMethodSimulator.GetAgentScred(parseTree, didDocument);
                objectScred = DidColorMethodSimulator.GetObjectScred(parseTree, agentScred);
                parseTree.didDocument = didDocument;
                parseTree.didAgentScred = agentScred;
                parseTree.didObjectScred = objectScred;
                //parseTree.DumpParseTree("e");
                parseTree.DumpDidDocument();
                parseTree.DumpAgentScred();
                parseTree.DumpObjectScred();
            }
            else if (parseTree.ifDidDocIndirect && parseTree.ifDidAgentIndirect && !parseTree.ifDidObjectIndirect) // return Agent Scred
            {
                didDocument = DidColorMethodSimulator.GetDidDocument(parseTree);
                agentScred = DidColorMethodSimulator.GetAgentScred(parseTree, didDocument);
                parseTree.didDocument = didDocument;
                parseTree.didAgentScred = agentScred;
                //parseTree.DumpParseTree("e");
                parseTree.DumpDidDocument();
                parseTree.DumpAgentScred();
            }
            else if (parseTree.ifDidDocIndirect && !parseTree.ifDidAgentIndirect && !parseTree.ifDidObjectIndirect) // return DID Document
            {
                didDocument = DidColorMethodSimulator.GetDidDocument(parseTree);
                parseTree.didDocument = didDocument;
                //parseTree.DumpParseTree("e");
                parseTree.DumpDidDocument();
            }
            else if (!parseTree.ifDidDocIndirect && !parseTree.ifDidAgentIndirect && !parseTree.ifDidObjectIndirect) // return DID verification test
            {
                //parseTree.DumpParseTree("e");
                parseTree = DidColorMethodSimulator.VerifyDidIdentifier(parseTree); // redundant
            }

            return parseTree;
        }

        public void DumpParseTree(string s)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                IncludeFields = true,
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize<ParseTree>(this, jsonOptions);
            Console.WriteLine(s + "> Parse Tree: " + json);
        }

        public void DumpDidDocument()
        {
            if (!String.IsNullOrEmpty(this.didDocument))
            {
                ConsoleColor cbgc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("d> DID Document: " + this.didDocument);
                Console.ForegroundColor = cbgc;
            }
        }

        public void DumpAgentScred()
        {
            if (!String.IsNullOrEmpty(this.didAgentScred))
            {
                ConsoleColor cbgc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("d> Agent Scred:  " + this.didAgentScred);
                Console.ForegroundColor = cbgc;
            }
        }

        public void DumpObjectScred()
        {
            if (!String.IsNullOrEmpty(this.didObjectScred))
            {
                ConsoleColor cbgc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("d> Object Scred: " + this.didObjectScred);
                Console.ForegroundColor = cbgc;
            }
        }
    }
}