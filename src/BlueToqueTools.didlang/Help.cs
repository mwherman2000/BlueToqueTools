using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueToqueTools.didlang
{
    public class Help
    {
        static DidColorMethodSimulator didcolorMethodSimulator = null;
        static System.Reflection.Assembly assembly = typeof(didlangProgram).Assembly;

        public static void Welcome()
        {
            Console.Clear();
            Console.WriteLine("didlang Language Command Line Interpreter for DID Identifiers, DID Documents, DID Agents, and DID Objects");
            Console.WriteLine("== Version: " + assembly.FullName.ToString());
            Console.WriteLine("== " + DateTime.Now.ToString());
            Console.WriteLine();
        }

        public static void CommandHelp()
        {
            Console.WriteLine("Enter help to redisplay this list of commands.");
            Console.WriteLine("Enter !help to see a list of command shortcuts.");
            Console.WriteLine("Enter <did> to verify a DID Identifier (\"no indirection\").");
            Console.WriteLine("Enter *<did> to return the DID Document associated with a DID Identifier (\"single indirection\").");
            Console.WriteLine("Enter **<did> to return the Agent Scred (VC) associated with a DID Identifier (\"double indirection\").");
            Console.WriteLine("Enter ***<did> to return the Object Scred (VC) associated with a DID Identifier (\"triple indirection\").");
            Console.WriteLine("Enter !help to display a list of advanced coercion commands and shortcuts.");

            //var streams = assembly.GetManifestResourceNames();
            var helpStream = assembly.GetManifestResourceStream("BlueToqueTools.didlang.DidLangCoercionHelp.txt");
            byte[] res = new byte[helpStream.Length];
            int nBytes = helpStream.Read(res);
            string helpText = Encoding.UTF8.GetString(res);
            Console.WriteLine(helpText);    
        }

        public static void ShortcutsHelp()
        {
            Console.WriteLine("!help   = redisplay this list of command shortcuts");
            Console.WriteLine("help    = display a list of top-level commands");
            Console.WriteLine("!0      = did:example:1234");
            Console.WriteLine("!r      = did:color:red");
            Console.WriteLine("!1      = *did:color:red");
            Console.WriteLine("!2      = **did:color:red");
            Console.WriteLine("!3      = ***did:color:red");
            Console.WriteLine("!red    = ***did:example:red");
            Console.WriteLine("!green  = ***did:example:green");
            Console.WriteLine("!blue   = ***did:example:blue");
            Console.WriteLine("!colors = display a list of the registered did:color DID Objects");
            Console.WriteLine("!a      = **(agentTypeA:idA)*did:color:red               (\"single agent coercion, triple indirection\")");
            Console.WriteLine("!b      = *(agentTypeB:idB)*did:color:red                (\"single agent coercion, double indirection\")");
            Console.WriteLine("!c      = *(agentInterfaceC:agentMethodC)**did:color:red (\"single method coercion, triple indirection\")");
            Console.WriteLine("!d      = *(agentInterfaceD:agentMethodD)*(agentTypeD:idD)*did:color:red (\"double coercion, triple indirection\")");
            Console.WriteLine("!e      = **(^did:color:red#agentcluster2)*did:color:red (Agent round-robin cluster test)");
        }
    }
}
