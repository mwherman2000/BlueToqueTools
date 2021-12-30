using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueToqueTools.didlang
{
    public class DidColorMethodSimulator
    {
        public const string name = "did:color DID Method Simulator (142 colors)";
        public const string version = "0.1.0.0";

        public static Dictionary<string, System.Drawing.Color> colors = new Dictionary<string, System.Drawing.Color>();
        internal static bool isInitialized = false;

        public DidColorMethodSimulator()
        {
            Initialize();
        }

        internal static void Initialize()
        {
            foreach (var colorCode in Enum.GetValues(typeof(System.Drawing.KnownColor)))
            {
#pragma warning disable CS8604 // Possible null reference argument.
                var color = System.Drawing.Color.FromKnownColor((System.Drawing.KnownColor)colorCode);
                if (!color.IsSystemColor)
                {
                    string colorName = colorCode.ToString();
                    colors[colorName.ToLower()] = color;
                }
#pragma warning restore CS8604 // Possible null reference argument.
            }
            isInitialized = true;
        }

        static public void DumpValues()
        {
            if (!isInitialized) Initialize();

            foreach (var colorCode in colors.Keys)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                var color = colors[colorCode.ToString().ToLower()];
                Console.WriteLine("did:color:{0}:\t{1}\t{2}\t{3}\t{4}", 
                    colorCode.ToString().PadRight(20), 
                    color.Name.PadRight(20), color.R, color.G, color.B);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            Console.WriteLine("{0} colors", colors.Count);
        }

        public static ParseTree VerifyDid(ParseTree parseTree)
        {
            bool isVerified = false;

            if (colors.Keys.Contains(parseTree.didIdString)) isVerified = true;

            parseTree.wasDidVerified = true;
            parseTree.wasDidVerifiedTrue = isVerified;

            return parseTree;
        }

        static System.Reflection.Assembly assembly = typeof(didlangProgram).Assembly;
        public  static string GetDidDocument(ParseTree parseTree)
        {
            string didDoc = String.Empty;

            if (!isInitialized) Initialize();

            //var streams = assembly.GetManifestResourceNames();
            var didDocTemplateStream = assembly.GetManifestResourceStream("BlueToqueTools.didlang.DidColorDidDocTemplate.json");
            byte[] res = new byte[didDocTemplateStream.Length];
            int nBytes = didDocTemplateStream.Read(res);
            string didDocTemplate = Encoding.UTF8.GetString(res);

            didDoc = didDocTemplate;
            didDoc = didDoc.Replace("%Color%", colors[parseTree.didIdString].Name).Replace("%color%", parseTree.didIdString);

            return didDoc;
        }

        public static string GetAgentScred(ParseTree parseTree, string didDocument)
        {
            string agentScred = String.Empty;

            if (!isInitialized) Initialize();

            var agentScredTemplateStream = assembly.GetManifestResourceStream("BlueToqueTools.didlang.DidColorAgentScredTemplate.json");
            byte[] res = new byte[agentScredTemplateStream.Length];
            int nBytes = agentScredTemplateStream.Read(res);
            string agentScredTemplate = Encoding.UTF8.GetString(res);

            agentScred = agentScredTemplate;

            return agentScred;
        }

        public static string GetObjectScred(ParseTree parseTree, string agentDocument)
        {
            string objectScred = String.Empty;

            if (!isInitialized) Initialize();

            var objectScredTemplateStream = assembly.GetManifestResourceStream("BlueToqueTools.didlang.DidColorObjectScredTemplate.json");
            byte[] res = new byte[objectScredTemplateStream.Length];
            int nBytes = objectScredTemplateStream.Read(res);
            string objectScredTemplate = Encoding.UTF8.GetString(res);

            objectScred = objectScredTemplate;
            objectScred = objectScred.Replace("%Color%", colors[parseTree.didIdString].Name).Replace("%color%", parseTree.didIdString);
            objectScred = objectScred.Replace("%A%", colors[parseTree.didIdString].A.ToString()).Replace("%R%", colors[parseTree.didIdString].R.ToString());
            objectScred = objectScred.Replace("%G%", colors[parseTree.didIdString].G.ToString()).Replace("%B%", colors[parseTree.didIdString].B.ToString());

            return objectScred;
        }
    }
}
