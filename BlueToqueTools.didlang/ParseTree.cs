using BlueToqueTools.didlang;

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
        Failed_BadDidIdentifier
    }

    public class ParseTree
    {
        // parsing state
        public DidParsingState parsingState = DidParsingState.Uninitialized;

        // DID Command pre-actions
        public bool ifDidDocIndirect = false;
        public bool ifDidAgentDocIndirect = false;
        public bool ifDidObjectIndirect = false;

        // DID Command stmt fields
        public string didIdentifier = String.Empty;
        public string didTopLevelName = String.Empty;
        public string didMethodName = String.Empty;
        public string didIdString = String.Empty;
        public string didDocument = String.Empty;
        public string didAgentUri = String.Empty;
        public string didAgentScred = String.Empty;
        public string didObjectScred = String.Empty;
        public string didAgentAction = "getObjectInterfaces"; // default action when calling an Agent

        // DID Command execution outputs
        public DidDocResolutionState didDocumentResolutionState = DidDocResolutionState.NotAttempted;
        public bool wasDidDocumentFound = false;
        public bool wasDidAgentScredFound = false;
        public bool wasDidAgentActionSuccessful = false; // default: getObjectInterfaces
        public bool wasDidVerified = false;
        public bool wasDidVerifiedTrue = false;
        public DidTrustLevel didTrustLevel = DidTrustLevel.Level0;

        public bool WasSuccessful()
        {
            return parsingState == DidParsingState.Successful;
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

            //var jsonOptions = new JsonSerializerOptions();
            //jsonOptions.IncludeFields = true;
            //jsonOptions.WriteIndented = true;
            //var json = JsonSerializer.Serialize<ParseTree>(this, jsonOptions);
            //Console.WriteLine("e> " + json);
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

            //var jsonOptions = new JsonSerializerOptions();
            //jsonOptions.IncludeFields = true;
            //jsonOptions.WriteIndented = true;
            //var json = JsonSerializer.Serialize<ParseTree>(this, jsonOptions);
            //Console.WriteLine("e> " + json);
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

            //var jsonOptions = new JsonSerializerOptions();
            //jsonOptions.IncludeFields = true;
            //jsonOptions.WriteIndented = true;
            //var json = JsonSerializer.Serialize<ParseTree>(this, jsonOptions);
            //Console.WriteLine("e> " + json);
        }
    }
}