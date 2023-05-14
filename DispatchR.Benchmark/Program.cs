using BenchmarkDotNet.Running;
using DispatchR.Benchmark;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<Benchmarky>();
    }
}