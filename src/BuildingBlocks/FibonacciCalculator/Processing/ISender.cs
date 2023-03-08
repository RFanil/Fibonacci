namespace Fibonacci.Processing {
    public interface ISender {
        Task Send(double number);
    }
}