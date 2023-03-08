namespace Fibonacci;
public class FibonacciCalculator {

    /* 
     * The sequence starts with 2 because the preceding numbers are 0,1,1. 
     * The service only sends the number - no further information, and it's impossible to know whether the 1 comes after 0 or 1.
     */
    private int firstNumberFromFibonacciSequence = 2;

    /// <summary>
    /// returns the first number from the Fibonacci sequence
    /// </summary>
    /// <returns></returns>
    public int GetTheFirstNumberFromTheFibonacciSequence() {
        return firstNumberFromFibonacciSequence;
    }
    /// <summary>
    /// returns the next number from the Fibonacci sequence
    /// </summary>
    /// <param name="number">n[i]</param>
    /// <returns>n(i+1)</returns>
    public double GetNextNumberFromFibonacciSequence(double number) {
        var nextNumber = GetNumberPrecedingNumberNInFibonacciSequence(number) + number;
        return nextNumber;
    }

    /// <summary>
    /// returns the number preceding the current number N in the Fibonacci sequence
    /// </summary>
    /// <param name="N">current number from Fibonacci sequence</param>
    /// <returns></returns>
    private int GetNumberPrecedingNumberNInFibonacciSequence(double N) {
        double a = N / ((1 + Math.Sqrt(5)) / 2.0);
        return (int)Math.Round(a);
    }
}