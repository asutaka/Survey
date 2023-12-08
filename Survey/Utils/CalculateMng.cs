using System;
using static TicTacTec.TA.Library.Core;

namespace Survey.Utils
{
    public static class CalculateMng
    {
        public static double[] ADX(double[] arrHigh, double[] arrLow, double[] arrClose, int period, int count)
        {
            var output = new double[1000];
            try
            {
                Adx(0, count - 1, arrHigh, arrLow, arrClose, period, out var outBegIdx, out var outNBElement, output);
                //return output[count - period];
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex, $"CalculateMng.ADX|EXCEPTION| {ex.Message}");
            }
            return output;
        }
        public static double[] MA(double[] arrInput, MAType type, int period, int count)
        {
            var output = new double[1000];
            try
            {
                MovingAverage(0, count - 1, arrInput, period, type, out var outBegIdx, out var outNBElement, output);
                //return output[count - period];
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex, $"CalculateMng.MA|EXCEPTION| {ex.Message}");
            }
            return output;
        }
        public static (double[], double[], double[]) BB(double[] arrInput, MAType type, int period, int count)
        {
            var outputU = new double[1000];
            var outputM = new double[1000];
            var outputL = new double[1000];
            try
            {
                Bbands(0, count - 1, arrInput, period, 2, 2, type, out var outBegIdx, out var outNBElement, outputU, outputM, outputL);
                //var div = count - period;
                //return (outputU[div], outputM[div], outputL[div]);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex, $"CalculateMng.MA|EXCEPTION| {ex.Message}");
            }
            return (outputU, outputM, outputL);
        }
        public static double[] MACD(double[] arrInput, int fast, int slow, int signal, int count)
        {
            var output = new double[1000];
            try
            {
                Macd(0, count - 1, arrInput, fast, slow, signal, out var outBegIdx, out var outNbElement, new double[1000], new double[1000], output);
                //return output[count - (slow + signal - 1)];
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex, $"CalculateMng.MACD|EXCEPTION| {ex.Message}");
            }
            return output;
        }
        public static double[] RSI(double[] arrInput, int period, int count)
        {
            var output = new double[1000];
            try
            {
                Rsi(0, count - 1, arrInput, period, out var outBegIdx, out var outNBElement, output);
                //return output[count - 1 - period];
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex, $"CalculateMng.RSI|EXCEPTION| {ex.Message}");
            }
            return output;
        }
    }
}
