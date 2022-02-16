using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using ManyConsole;

namespace Asv.Tools.Shell.Commands
{
    public class BenchmarkFFTCommand : ConsoleCommand
    {
        public BenchmarkFFTCommand()
        {
            IsCommand("benchmark-fft", "Benchmark test");
        }

        public override int Run(string[] remainingArguments)
        {
            BenchmarkRunner.Run<FFT>();
            return 0;
        }
    }


    [SimpleJob(RuntimeMoniker.CoreRt60)]
    [RPlotExporter]
    [MemoryDiagnoser]
    public class FFT
    {

        private double[] _data = new double[19200];
        private alglib.complex[] _dataComplex = new alglib.complex[19200];

        public FFT()
        {
           
        }

        [GlobalSetup]
        public void Setup()
        {
            
           
            

        }

        [Benchmark]
        public void AlgLibReal()
        {
            for (var j = 0; j < _data.Length; j++)
            {
                _data[j] = Math.Sin(j * 90 * Math.PI / 180);
                _dataComplex[j] = new alglib.complex(Math.Sin(j * 90 * Math.PI / 180), 0);
            }
            alglib.fftr1d(_data,out var complex);

        }

        [Benchmark]
        public void AlgLibComplex()
        {
            for (var j = 0; j < _data.Length; j++)
            {
                _data[j] = Math.Sin(j * 90 * Math.PI / 180);
                _dataComplex[j] = new alglib.complex(Math.Sin(j * 90 * Math.PI / 180), 0);
            }
            alglib.fftc1d(ref _dataComplex);

        }
    }
}
