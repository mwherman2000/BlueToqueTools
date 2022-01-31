using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4KSyntheticPressureCurve
{
    internal class Program
    {
        static bool autoPeak = false;

        static void Main(string[] args)
        {
            int d1ms = 0, d2ms = 0, d3ms = 0, d4ms = 0, d5ms = 0;
            int medianms, t0ms, t1ms, t2ms, t3ms, t4ms, t5ms;
            int peak = 0, d1s = 0, d2s = 0, d3s = 0, d4s = 0, d5s = 0;
            int medians, t0s, t1s, t2s, t3s, t4s, t5s;

            char ch = 'I';
            while (ch != 'q')
            {
                switch(ch)
                {
                    case 'I':
                        {
                            peak = N4KSyntheticPressureCurve.PEAKMAX * 3 / 4;
                            d1ms = 5 * N4KSyntheticPressureCurve.MILLI + SomeNoise();
                            d2ms = 10 * N4KSyntheticPressureCurve.MILLI + SomeNoise();
                            d3ms = 30 * N4KSyntheticPressureCurve.MILLI + SomeNoise();
                            d4ms = 10 * N4KSyntheticPressureCurve.MILLI + SomeNoise();
                            d5ms = N4KSyntheticPressureCurve.D5MSMAX + SomeNoise();
                            break;
                        }
                    case '=': { autoPeak = !autoPeak; break; }
                    case '^': case 'V': 
                        { 
                            peak += 1; autoPeak = false;  
                            if (peak > N4KSyntheticPressureCurve.PEAKMAX) { peak = N4KSyntheticPressureCurve.PEAKMAX; Console.Beep(); }
                            break; 
                        }
                    case 'v': 
                        { peak -= 1; autoPeak = false;  
                            if (peak < N4KSyntheticPressureCurve.PEAKMIN) { peak = N4KSyntheticPressureCurve.PEAKMIN; Console.Beep(); } 
                            break; 
                        }
                    case '1': { peak = 1; autoPeak = false; break; }
                    case '2': { peak = 2; autoPeak = false; break; }
                    case '3': { peak = 3; autoPeak = false; break; }
                    case '4': { peak = 4; autoPeak = false; break; }
                    case '5': { peak = 5; autoPeak = false; break; }
                    case '6': { peak = 6; autoPeak = false; break; }
                    case '7': { peak = 7; autoPeak = false; break; }
                    case '8': { peak = 8; autoPeak = false; break; }
                    case '9': { peak = 9; autoPeak = false; break; }
                    case '0': { peak = 10; autoPeak = false; break; }
                    case 'a': 
                        { 
                            d1ms += SomeNoise(); 
                            if (d1ms > N4KSyntheticPressureCurve.D1MSMAX) { d1ms = N4KSyntheticPressureCurve.D1MSMAX - SomeNoise(); Console.Beep(); } 
                            break; 
                        }
                    case 'A': { d1ms -= SomeNoise(); if (d1ms < N4KSyntheticPressureCurve.MILLI) { d1ms = N4KSyntheticPressureCurve.MILLI + SomeNoise(); Console.Beep(); } break; }
                    case 'p': 
                        { 
                            d2ms += SomeNoise(); 
                            if (d2ms > N4KSyntheticPressureCurve.D2MSMAX) { d2ms = N4KSyntheticPressureCurve.D2MSMAX - SomeNoise(); Console.Beep(); } 
                            break; 
                        }
                    case 'P': { d2ms -= SomeNoise(); if (d2ms < N4KSyntheticPressureCurve.MILLI) { d2ms = N4KSyntheticPressureCurve.MILLI + SomeNoise(); Console.Beep(); } break; }
                    case 's': 
                        { 
                            d3ms += SomeNoise(); 
                            if (d3ms > N4KSyntheticPressureCurve.D3MSMAX) { d3ms = N4KSyntheticPressureCurve.D3MSMAX - SomeNoise(); Console.Beep(); }
                            break; 
                        }
                    case 'S': { d3ms -= SomeNoise(); if (d3ms < N4KSyntheticPressureCurve.MILLI) { d3ms = N4KSyntheticPressureCurve.MILLI + SomeNoise(); Console.Beep(); } break; }
                    case 'r': 
                        { 
                            d4ms += SomeNoise(); 
                            if (d4ms > N4KSyntheticPressureCurve.D4MSMAX) { d4ms = N4KSyntheticPressureCurve.D4MSMAX - SomeNoise(); Console.Beep(); }
                            break; 
                        }
                    case 'R': { d4ms -= SomeNoise(); if (d4ms < N4KSyntheticPressureCurve.MILLI) { d4ms = N4KSyntheticPressureCurve.MILLI + SomeNoise(); Console.Beep(); } break; }
                    default: { break; }

                }

                d5ms = (d2ms + d3ms + d4ms) / 5;

                t0ms = 0;
                t1ms = t0ms + d1ms;
                t2ms = t1ms + d2ms;
                t3ms = t2ms + d3ms;
                t4ms = t3ms + d4ms;
                t5ms = t4ms + d5ms;

                if (autoPeak) peak = -1;

                var synthCurve = new N4KSyntheticPressureCurve(d1ms, d2ms, d3ms, d4ms, peak);
                //var synthCurve = new N4KSyntheticPressureCurve(d1ms, d2ms + d3ms + d4ms, peak);

                int[] approachCurve = synthCurve.GetD1ApproachCurve();
                int[] pressCurve = synthCurve.GetD2PressCurve();
                int[] sustainCurve = synthCurve.GetD3SustainCurve();
                int[] releaseCurve = synthCurve.GetD4ReleaseCurve();
                int[] recoveryCurve = synthCurve.GetD5RecoveryCurve();

                // Recalculate durations (seconds)
                d1s = approachCurve.Length;
                d2s = pressCurve.Length;
                d3s = sustainCurve.Length;
                d4s = releaseCurve.Length;
                d5s = recoveryCurve.Length;

                // Recalculate time line values (seconds)
                t0s = 0;
                t1s = t0s + d1s;
                t2s = t1s + d2s;
                t3s = t2s + d3s;
                t4s = t3s + d4s;
                t5s = t4s + d5s;

                int totalCoverage = synthCurve.Coverage();
                medians = synthCurve.MedianS();
                medianms = synthCurve.MedianMs();
                peak = synthCurve.Peak();

                // Clear chart
                char[][] chart = new char[10][];
                for (int i = 0; i < chart.Length; i++) chart[i] = new char[150];
                foreach (var row in chart)
                {
                    for (int i = 0; i < row.Length; i++) row[i] = '.'; // Background
                }

                // Plot synthetic pressure curve
                for (int i = 0; i < approachCurve.Length; i++) chart[approachCurve[i]][t0s + i] = '_'; // APPROACH
                for (int i = 0; i < pressCurve.Length; i++) chart[pressCurve[i]][t1s + i] = '/'; // PRESS
                for (int i = 0; i < sustainCurve.Length; i++) chart[sustainCurve[i]][t2s + i] = '='; // SUSTAIN
                for (int i = 0; i < releaseCurve.Length; i++) chart[releaseCurve[i]][t3s + i] = '\\'; // RELEASE
                for (int i = 0; i < recoveryCurve.Length; i++) chart[recoveryCurve[i]][t4s + i] = '_'; // RECOVERY

                foreach (var row in chart)
                {
                    row[medians] = '|'; // PEAK
                }
                chart[peak][medians] = '*';  // TOP

                // Output chart
                Console.Clear();
                Console.WriteLine("BlueToqueTools N4K Synthentic Pressure Curve for a Kiss version 0.2");
                Console.WriteLine("Time unit: 0.1 millisecond (0.0001 second)");
                Console.WriteLine();
                Console.WriteLine("t0=" + t0ms.ToString().PadRight(10) + "\tAPPROACH (approach " + d1ms.ToString() + ")\t" + t0s.ToString() + " " + d1s.ToString());
                Console.WriteLine("t1=" + t1ms.ToString().PadRight(10) + "\tPRESS    (press    " + d2ms.ToString() + ")\t" + t1s.ToString() + " " + d2s.ToString());
                Console.WriteLine("t2=" + t2ms.ToString().PadRight(10) + "\tSUSTAIN  (sustain  " + d3ms.ToString() + ")\t" + t2s.ToString() + " " + d3s.ToString());
                Console.WriteLine("t3=" + t3ms.ToString().PadRight(10) + "\tRELEASE  (release  " + d4ms.ToString() + ")\t" + t3s.ToString() + " " + d4s.ToString());
                Console.WriteLine("t4=" + t4ms.ToString().PadRight(10) + "\tRECOVERY (recovery " + d5ms.ToString() + ")\t" + t4s.ToString() + " " + d5s.ToString());
                Console.WriteLine("t5=" + t5ms.ToString().PadRight(10) + "\tFINISH                 " + " \t" + t5s.ToString());
                Console.WriteLine();
                Console.WriteLine("m=" + medianms.ToString().PadRight(10) + "\tMEDIAN   |" + "\t" + medians.ToString());
                Console.WriteLine("p=" + peak.ToString().PadRight(10) + "\tPEAK     *" + "\tautoPeak " + autoPeak.ToString());

                // Output left axis and synthetic pressure curve
                Console.WriteLine();
                for (int y = 10; y > 0; y--)
                {
                    Console.Write((y % 10).ToString());
                    for (int x = 0; x < t5s; x++) Console.Write(chart[y-1][x]);
                    Console.WriteLine();
                }

                // Output timeline
                Console.WriteLine();
                Console.Write(">");
                for (int t = 1; t <= d1s; t++) Console.Write("a");
                for (int t = 1; t <= d2s; t++) Console.Write("p");
                for (int t = 1; t <= d3s; t++) Console.Write("s");
                for (int t = 1; t <= d4s; t++) Console.Write("r");
                for (int t = 1; t <= d5s; t++) Console.Write("v");
                Console.WriteLine();

                // Output bottom axis
                for (int x = 0; x <= t5s; x++)
                {
                    if ((x % 10) == 0)
                    {
                        Console.Write("*");
                    }
                    else
                    {
                        Console.Write((x % 10).ToString());
                    }
                }
                Console.WriteLine();

                Console.WriteLine();
                Console.WriteLine("coverage=" + totalCoverage + " " + "".PadRight(totalCoverage/10, '*'));

                // Calculate and output DID Identifiers
                string didNfe = "did:bluetoquenfe:" + Guid.NewGuid().ToString();
                string didDeed = "did:bluetoquedeed:" + Guid.NewGuid().ToString();
                string didObject = "did:object:" + Guid.NewGuid().ToString();
                Console.WriteLine();
                Console.WriteLine("DID Identifiers generated for this kiss synthetic pressure curve - a kiss being a non-fungible activity (NFA):");
                Console.WriteLine(didDeed);
                Console.WriteLine("-> " + didNfe);
                Console.WriteLine("   -> " + didObject);

                ch = Console.ReadKey(true).KeyChar;
            }
        }

        static Random r = new Random((int)DateTime.Now.Ticks);
        private static int SomeNoise()
        {
            return r.Next(N4KSyntheticPressureCurve.MILLI);
        }

        static int[] GetScaledLeftCurve(int peak, int duration)
        {
            int[] result = new int[duration];

            //Console.WriteLine("peak, duration: " + peak.ToString() + " " + duration.ToString());

            double a = 0; 
            for (int i = 0; i < duration; i++)
            {
                double y = System.Math.Sin(a);
                int iy = (int)System.Math.Round(y * (peak-1)) + 1;
                result[i] = iy;
                //Console.WriteLine(i.ToString() + "\t" + a.ToString() + "\t" + y.ToString() + "\t" + result[i].ToString());
                a += (Math.PI / 2) / duration;
            }

            return result; 
        }

        static int[] GetScaledRightCurve(int peak, int duration)
        {
            //Console.WriteLine("peak, duration: " + peak.ToString() + " " + duration.ToString());

            int[] result = GetScaledLeftCurve(peak, duration);

            for (int i = 0; i < duration/2; i++)
            {
                int temp = result[i]; 
                result[i] = result[duration - 1 - i];
                result[duration - 1 - i] = temp;
            }

            //for (int i = 0; i < duration; i++)
            //{
            //    Console.WriteLine(i.ToString() + "\t" + result[i].ToString());
            //}

            return result;
        }
    }
}
