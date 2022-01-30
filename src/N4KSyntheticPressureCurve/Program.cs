using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4KSyntheticPressureCurve
{
    internal class Program
    {
        const int MILLI = 1000;
        const int D1MAX = 10 * MILLI;
        const int D2MAX = 30 * MILLI;
        const int D3MAX = 30 * MILLI;
        const int D4MAX = 30 * MILLI;
        const int D5MAX = 5 * MILLI;

        const int IPMIN = 1;
        const int IPMAX = 10;

        static bool autoPeak = false;

        static void Main(string[] args)
        {
            int d1, d2, d3, d4, d5;
            int m, t0, t1, t2, t3, t4, t5;
            int ip, id1, id2, id3, id4, id5;
            int im, it0, it1, it2, it3, it4, it5;
            int totalCoverage, leftCoverage, sustainCoverge, rightCoverage;

            ip = IPMAX * 3 / 4;
            d1 = 5 * MILLI + SomeNoise();
            d2 = 10 * MILLI + SomeNoise();
            d3 = 30 * MILLI + SomeNoise();
            d4 = 10 * MILLI + SomeNoise();
            d5 = D5MAX + SomeNoise();

            char ch = '-';
            while (ch != 'q')
            {
                switch(ch)
                {
                    case '=': { autoPeak = !autoPeak; break; }
                    case '^': case 'V': { ip += 1; autoPeak = false;  if (ip > IPMAX) { ip = IPMAX; Console.Beep(); } break; }
                    case 'v': { ip -= 1; autoPeak = false;  if (ip < IPMIN) { ip = IPMIN;      Console.Beep(); } break; }
                    case '1': { ip = 1; autoPeak = false; break; }
                    case '2': { ip = 2; autoPeak = false; break; }
                    case '3': { ip = 3; autoPeak = false; break; }
                    case '4': { ip = 4; autoPeak = false; break; }
                    case '5': { ip = 5; autoPeak = false; break; }
                    case '6': { ip = 6; autoPeak = false; break; }
                    case '7': { ip = 7; autoPeak = false; break; }
                    case '8': { ip = 8; autoPeak = false; break; }
                    case '9': { ip = 9; autoPeak = false; break; }
                    case '0': { ip = 10; autoPeak = false; break; }
                    case 'a': { d1 += SomeNoise(); if (d1 > D1MAX) { d1 = D1MAX - SomeNoise(); Console.Beep(); } break; }
                    case 'A': { d1 -= SomeNoise(); if (d1 < MILLI) { d1 = MILLI + SomeNoise(); Console.Beep(); } break; }
                    case 'p': { d2 += SomeNoise(); if (d2 > D2MAX) { d2 = D2MAX - SomeNoise(); Console.Beep(); } break; }
                    case 'P': { d2 -= SomeNoise(); if (d2 < MILLI) { d2 = MILLI + SomeNoise(); Console.Beep(); } break; }
                    case 's': { d3 += SomeNoise(); if (d3 > D3MAX) { d3 = D3MAX - SomeNoise(); Console.Beep(); } break; }
                    case 'S': { d3 -= SomeNoise(); if (d3 < MILLI) { d3 = MILLI + SomeNoise(); Console.Beep(); } break; }
                    case 'r': { d4 += SomeNoise(); if (d4 > D4MAX) { d4 = D4MAX - SomeNoise(); Console.Beep(); } break; }
                    case 'R': { d4 -= SomeNoise(); if (d4 < MILLI) { d4 = MILLI + SomeNoise(); Console.Beep(); } break; }
                    default: { break; }

                }

                if (autoPeak) ip = (int)Math.Round((double)IPMAX * (double)(d2 + d3 + d4) / (double)(D2MAX + D3MAX + D4MAX));

                t0 = 0;
                t1 = t0 + d1;
                t2 = t1 + d2;
                t3 = t2 + d3;
                t4 = t3 + d4;

                d5 = (d2 + d3 + d4) / 5;
                t5 = t4 + d5;

                m = t2 + (4 * d3 - 2 * d2 + 2 * d4) / 8;

                // Normalize values for plotting the curve
                it0 = (int)Math.Round((double)t0 / MILLI);
                it1 = (int)Math.Round((double)t1 / MILLI);
                it2 = (int)Math.Round((double)t2 / MILLI);
                it3 = (int)Math.Round((double)t3 / MILLI);
                it4 = (int)Math.Round((double)t4 / MILLI);
                it5 = (int)Math.Round((double)t5 / MILLI);
                id1 = it1 - it0;
                id2 = it2 - it1;
                id3 = it3 - it2;    
                id4 = it4 - it3;    
                id5 = it5 - it4;

                im = it2 + (4 * id3 - 2 * id2 + 2 * id4) / 8;

                Console.Clear();
                Console.WriteLine("BlueToqueTools N4K Synthentic Pressure Curve for a Kiss version 0.1");
                Console.WriteLine("Time unit: 0.1 millisecond (0.0001 second)");
                Console.WriteLine();
                Console.WriteLine("t0=" + t0.ToString().PadRight(10) + "\tAPPROACH (approach " + d1.ToString() + ")\t" + it0.ToString());
                Console.WriteLine("t1=" + t1.ToString().PadRight(10) + "\tPRESS    (press    " + d2.ToString() + ")\t" + it1.ToString() + " " + id1.ToString());
                Console.WriteLine("t2=" + t2.ToString().PadRight(10) + "\tSUSTAIN  (sustain  " + d3.ToString() + ")\t" + it2.ToString() + " " + id2.ToString());
                Console.WriteLine("t3=" + t3.ToString().PadRight(10) + "\tRELEASE  (release  " + d4.ToString() + ")\t" + it3.ToString() + " " + id3.ToString());
                Console.WriteLine("t4=" + t4.ToString().PadRight(10) + "\tRECOVERY (recovery " + d5.ToString() + ")\t" + it4.ToString() + " " + id4.ToString());
                Console.WriteLine("t5=" + t5.ToString().PadRight(10) + "\tFINISH                 "             + " \t" + it5.ToString() + " " + id5.ToString());
                Console.WriteLine();
                Console.WriteLine("m=" + m.ToString().PadRight(10) +  "\tMEDIAN   |" + "\t" + im.ToString());
                Console.WriteLine("p=" + ip.ToString().PadRight(10) + "\tPEAK     *" + "\tautoPeak " + autoPeak.ToString());

                //Console.WriteLine();
                int[] leftPressureCurve = GetScaledLeftCurve(ip, id2);
                int[] rightPressureCurve = GetScaledRightCurve(ip, id4);

                leftCoverage = 0; foreach (int pressure in leftPressureCurve) leftCoverage += pressure;
                sustainCoverge = id3 * ip;
                rightCoverage = 0; foreach (int pressure in rightPressureCurve) rightCoverage += pressure;
                totalCoverage = id1 + leftCoverage + sustainCoverge + rightCoverage + id5;

                // Clear chart
                char[][] chart = new char[10][];
                for (int i = 0; i < chart.Length; i++) chart[i] = new char[150];
                foreach (var row in chart)
                {
                    for (int i = 0; i < row.Length; i++) row[i] = '.'; // Background
                }

                // Plot synthetic pressure curve
                for (int i = it0+1; i <= it1; i++) chart[1 - 1][i - 1] = '_'; // APPROACH

                for (int i = 1; i <= id2; i++)
                {
                    int y = leftPressureCurve[i - 1];
                    chart[y - 1][it1 + i - 1] = '/'; // PRESS
                }

                for (int t = it2+1; t <= it3; t++) chart[ip - 1][t - 1] = '='; // SUSTAIN

                foreach (var row in chart)
                {
                    row[im - 1] = '|'; // PEAK
                }
                chart[ip-1][im - 1] = '*';  // TOP

                for (int i = 1; i <= id4; i++)
                {
                    int y = rightPressureCurve[i - 1];
                    chart[y - 1][it3 + i - 1] = '\\'; // RELEASE
                }

                for (int t = it4 + 1; t <= it5; t++) chart[1 - 1][t - 1] = '_'; // RECOVERY

                // Output left axis and synthetic pressure curve
                Console.WriteLine();
                for (int y = 10; y > 0; y--)
                {
                    Console.Write((y % 10).ToString());
                    for (int x = 0; x < it5; x++) Console.Write(chart[y-1][x]);
                    Console.WriteLine();
                }

                // Output timeline
                Console.WriteLine();
                Console.Write(">");
                for (int t = 1; t <= id1; t++) Console.Write("a");
                for (int t = 1; t <= id2; t++) Console.Write("p");
                for (int t = 1; t <= id3; t++) Console.Write("s");
                for (int t = 1; t <= id4; t++) Console.Write("r");
                for (int t = 1; t <= id5; t++) Console.Write("v");
                Console.WriteLine();

                // Output bottom axis
                for (int x = 0; x <= it5; x++)
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
            return r.Next(MILLI);
        }

        static int[] GetScaledLeftCurve(int peak, int duration)
        {
            const double PI = 3.14159;

            int[] result = new int[duration];

            //Console.WriteLine("peak, duration: " + peak.ToString() + " " + duration.ToString());

            double a = 0; 
            for (int i = 0; i < duration; i++)
            {
                double y = System.Math.Sin(a);
                int iy = (int)System.Math.Round(y * (peak-1)) + 1;
                result[i] = iy;
                //Console.WriteLine(i.ToString() + "\t" + a.ToString() + "\t" + y.ToString() + "\t" + result[i].ToString());
                a += (PI / 2) / duration;
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
