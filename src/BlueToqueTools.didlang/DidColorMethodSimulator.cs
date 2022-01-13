using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueToqueTools.didlang
{
    public class DidColorMethodSimulator
    {
        public const string Name = "did:color DID Method Simulator";
        public const string Version = "0.3.0.1229";

        static readonly System.Reflection.Assembly assembly = typeof(didlangProgram).Assembly;

        private static Dictionary<string, System.Drawing.Color> colors = new();
        private static bool isColorsInitialized = false;

        private static int currentRoundRobinCounter = -1; // not clustered
        private const int roundRobinSize = 4; // TODO hardcoded to match DidColorDidDocTemplate.json template
        private static int NextRoundRobinIndex() {
            currentRoundRobinCounter = (++currentRoundRobinCounter) % roundRobinSize;
            return currentRoundRobinCounter;
        }

        public DidColorMethodSimulator()
        {
            Console.WriteLine("Loading     : " + Name + " version " + Version);
            InitializeColors();
        }

        internal static void InitializeColors()
        {
            Console.WriteLine("Initializing: " + Name + " version " + Version);
            Console.WriteLine();

            colors.Clear(); 
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
            isColorsInitialized = true;
        }

        static public void DumpValues()
        {
            if (!isColorsInitialized) InitializeColors();

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

        public static ParseTree VerifyDidIdentifier(ParseTree parseTree)
        {
            bool isVerified = false;

            if (!isColorsInitialized) InitializeColors();

            if (colors.ContainsKey(parseTree.didIdString)) isVerified = true;

            parseTree.wasDidVerified = true;
            parseTree.wasDidVerifiedTrue = isVerified;

            return parseTree;
        }

        public  static string GetDidDocument(ParseTree parseTree)
        {
            string didDoc = String.Empty;

            if (!isColorsInitialized) InitializeColors();

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

            if (!isColorsInitialized) InitializeColors();

            var agentScredTemplateStream = assembly.GetManifestResourceStream("BlueToqueTools.didlang.DidColorAgentScredTemplate.json");
            byte[] res = new byte[agentScredTemplateStream.Length];
            int nBytes = agentScredTemplateStream.Read(res);
            string agentScredTemplate = Encoding.UTF8.GetString(res);

            if (parseTree.didAgentServiceEndpointId == "did:color:red#agentcluster2") // Double HACK
            {
                NextRoundRobinIndex();
            }
            else
            {
                currentRoundRobinCounter = -1;
            }
            agentScred = agentScredTemplate.Replace("%RoundRobinIndex%", currentRoundRobinCounter.ToString());

            return agentScred;
        }

        public static string GetObjectScred(ParseTree parseTree, string agentDocument)
        {
            string objectScred = String.Empty;

            if (!isColorsInitialized) InitializeColors();

            var objectScredTemplateStream = assembly.GetManifestResourceStream("BlueToqueTools.didlang.DidColorObjectScredTemplate.json");
            byte[] res = new byte[objectScredTemplateStream.Length];
            int nBytes = objectScredTemplateStream.Read(res);
            string objectScredTemplate = Encoding.UTF8.GetString(res);

            objectScred = objectScredTemplate;
            objectScred = objectScred.Replace("%Color%", colors[parseTree.didIdString].Name).Replace("%color%", parseTree.didIdString);
            objectScred = objectScred.Replace("%A%", colors[parseTree.didIdString].A.ToString()).Replace("%R%", colors[parseTree.didIdString].R.ToString());
            objectScred = objectScred.Replace("%G%", colors[parseTree.didIdString].G.ToString()).Replace("%B%", colors[parseTree.didIdString].B.ToString());
            objectScred = objectScred.Replace("%RoundRobinIndex%", currentRoundRobinCounter.ToString());

            return objectScred;
        }
    }
}
