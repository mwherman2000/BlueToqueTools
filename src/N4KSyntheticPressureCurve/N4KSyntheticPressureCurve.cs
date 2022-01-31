using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4KSyntheticPressureCurve
{
    public class N4KSyntheticPressureCurve
    {
        public const int MILLI = 1000;
        public const int D1MSMAX = 10 * MILLI;
        public const int D2MSMAX = 30 * MILLI;
        public const int D3MSMAX = 30 * MILLI;
        public const int D4MSMAX = 30 * MILLI;
        public const int D5MSMAX = 5 * MILLI;

        public const int PEAKMIN = 0;
        public const int PEAKMAX = 9;

        private int d1ms, d2ms, d3ms, d4ms, d5ms; // milliseconds
        private int t0ms, t1ms, t2ms, t3ms, t4ms, t5ms; // milliseconds
        private int d1s, d2s, d3s, d4s, d5s; // seconds
        private int t0s, t1s, t2s, t3s, t4s, t5s; // seconds - t5s = total width of the curve
        private int peak; // peak height of the curve
        private int medianms, medians; // weigthed median/center of curve in milliseconds and seconds

        public N4KSyntheticPressureCurve(int d1msactual, int d234msactual, int peakactual)
        {
            int d2msestimate = (int)Math.Round((double)d234msactual * 3 / 8);
            int d3msestimate = (int)Math.Round((double)d234msactual * 4 / 8);
            int d4msestimate = (int)Math.Round((double)d234msactual * 1 / 8);

            Initialize(d1msactual, d2msestimate, d3msestimate, d4msestimate, peakactual);
        }

        public N4KSyntheticPressureCurve(int d1msactual, int d2msactual, int d3msactual, int d4msactual, int peakactual)
        {
            Initialize(d1msactual, d2msactual, d3msactual, d4msactual, peakactual);
        }

        private void Initialize(int d1msactual, int d2msactual, int d3msactual, int d4msactual, int peakactual)
        { 
            d1ms = d1msactual;
            d2ms = d2msactual;
            d3ms = d3msactual;
            d4ms = d4msactual;
            d5ms = (d2ms + d3ms + d4ms) / 5;
            peak = peakactual;
            if (peak == -1) peak = (int)Math.Round((double)PEAKMAX * (double)(d2ms + d3ms + d4ms) / (double)(D2MSMAX + D3MSMAX + D4MSMAX));

            d1s = (int)Math.Round((double)d1ms / MILLI);
            d2s = (int)Math.Round((double)d2ms / MILLI);
            d3s = (int)Math.Round((double)d3ms / MILLI);
            d4s = (int)Math.Round((double)d4ms / MILLI);
            d5s = (d2s + d3s + d4s) / 5;

            t0s = 0;
            t1s = t0s + d1s;
            t2s = t1s + d2s;
            t3s = t2s + d3s;
            t4s = t3s + d4s;
            t5s = t4s = d5s;
            medians = t2s + (4 * d3s - 2 * d2s + 2 * d4s) / 8; 

            t0ms = 0;
            t1ms = t0ms + d1ms;
            t2ms = t1ms + d2ms;
            t3ms = t2ms + d3ms;
            t4ms = t3ms + d4ms;
            t5ms = t4ms + d5ms;
            medianms = t2ms + (4 * d3ms - 2 * d2ms + 2 * d4ms) / 8;
        }

        public int TotalWidth() { return t5s; }
        public int TotalDuration() { return t5ms; }
        public int Peak() { return peak; }
        public int MedianS() { return medians; }
        public int MedianMs() { return medianms; }

        public int Coverage()
        {
            int pressCoverage = 0; foreach (int pressure in GetD2PressCurve()) pressCoverage += pressure;
            int sustainCoverage = 0; foreach (int pressure in GetD3SustainCurve()) sustainCoverage += pressure;
            int releaseCoverage = 0; foreach (int pressure in GetD4ReleaseCurve()) releaseCoverage += pressure;
            int totalCoverage = pressCoverage + sustainCoverage + releaseCoverage;

            return totalCoverage;
        }

        public int[] GetD1ApproachCurve()
        {
            int[] curve = new int[d1s];
            for (int i = 0; i < curve.Length; i++) curve[i] = 0;
            return curve;
        }

        public int[] GetD2PressCurve()
        {
            int[] curve = GetScaledLeftCurve(peak, d2s);
            return curve;
        }

        public int[] GetD3SustainCurve()
        {
            int[] curve = new int[d3s];
            for (int i = 0; i < curve.Length; i++) curve[i] = peak;
            return curve;
        }

        public int[] GetD4ReleaseCurve()
        {
            int[] curve = GetScaledLeftCurve(peak, d4s);

            for (int i = 0; i < d4s / 2; i++)
            {
                int temp = curve[i];
                curve[i] = curve[d4s - 1 - i];
                curve[d4s - 1 - i] = temp;
            }

            return curve;
        }

        public int[] GetD5RecoveryCurve()
        {
            int[] curve = new int[d5s];
            for (int i = 0; i < curve.Length; i++) curve[i] = 0;
            return curve;
        }

        private int[] GetScaledLeftCurve(int peak, int duration)
        {
            int[] result = new int[duration];

            //Console.WriteLine("peak, duration: " + peak.ToString() + " " + duration.ToString());

            double a = 0;
            for (int i = 0; i < duration; i++)
            {
                double y = System.Math.Sin(a);
                int iy = (int)System.Math.Round(y * peak);
                result[i] = iy;
                //Console.WriteLine(i.ToString() + "\t" + a.ToString() + "\t" + y.ToString() + "\t" + curve[i].ToString());
                a += (Math.PI / 2) / duration;
            }

            return result;
        }
    }
}
