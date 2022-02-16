using System;
using Asv.Tools.Dsp;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using ManyConsole;

namespace Asv.Tools.Shell.Commands
{
    public class BenchmarkFiltersCommand : ConsoleCommand
    {
        public BenchmarkFiltersCommand()
        {
            IsCommand("benchmark-filter", "Benchmark filters test");
        }

        public override int Run(string[] remainingArguments)
        {
            var summary = BenchmarkRunner.Run<Filters>();
            return 0;
        }
    }



    /// <summary>
        /// 
        /// </summary>
        public class CustomLowPassElliptic8kHzFilter : EllipticFilterBase
    {
        public CustomLowPassElliptic8kHzFilter() : base(
            new[] { 3.55913675e-02, 1.543561252e-02, 1.554841621e-02, 7.140834236e-02 },
            new[] { 9.78033577e-02, -0.6706407089, 2.124672252, -3.813517492, 4.307268182, -2.841026455 })
        {
        }
    }

    public class CustomLowPassElliptic16kHzFilter : EllipticFilterBase
    {
        public CustomLowPassElliptic16kHzFilter() : base(
            new[] { 0.04667004718, 0.04749984526, 0.05705757177, 0.1124555474 },
            new[] { 0.06936510206, -0.4529627909, 1.475483766, -2.663272098, 3.252017007, -2.26572051 })
        {
        }
    }

    public class CustomLowPassElliptic200HzFilter : EllipticFilterBase
    {
        public CustomLowPassElliptic200HzFilter() : base(
            new[] { 2.412991323e-03, -7.227585943e-03, 4.814606192e-03 },
            new[] { -0.9356100233, 4.739455149, -9.604616258, 9.733305413, -4.932534258 })
        {
        }
    }

    /// <summary>
    /// 6th Order High Pass Elliptic
    /// Bilinear Transformation with Prewarping
    /// Sample Frequency = 48.00 KHz
    /// Standard Form
    /// Arithmetic Precision = 10 Digits
    ///
    /// Pass Band Frequency = 9.500 KHz
    /// Pass Band Ripple = 0.01 dB
    ///
    /// Stop Band Ratio = 1.252
    /// Stop Band Frequency = 7.590 KHz
    /// Stop Band Attenuation = 30 dB
    /// 
    /// На 12kHz и 16kHz - говно. Не попадать на 12kHz и 16kHz!
    /// </summary>
    public class CustomHighPassElliptic8kHzFilter : EllipticFilterBase
    {
        public CustomHighPassElliptic8kHzFilter() : base(
            new[] { 0.2054803206, -0.9055125671, 1.898034831, -2.39600517 },
            new[] { 4.686630908e-02, -0.2775068232, 1.015244724, -1.805542926, 2.502835814, -1.766064012 })
        {
        }
    }

    /// <summary>
    /// 6th Order High Pass Elliptic
    /// Bilinear Transformation with Prewarping
    /// Sample Frequency = 96.00 KHz
    /// Standard Form
    /// Arithmetic Precision = 10 Digits
    ///
    /// Pass Band Frequency = 18.00 KHz
    /// Pass Band Ripple = 0.01 dB
    ///
    /// Stop Band Ratio = 1.252
    /// Stop Band Frequency = 14.38 KHz
    /// Stop Band Attenuation = 30 dB
    /// </summary>
    public class CustomHighPassElliptic16kHzFilter : EllipticFilterBase
    {
        public CustomHighPassElliptic16kHzFilter() : base(
            new[] { 0.2249663134, -1.029515124, 2.202685094, -2.796272568 },
            new[] { 0.05436270034, -0.3448862195, 1.205663801, -2.202221502, 2.866766416, -2.036704992 })
        {
        }
    }


    [SimpleJob(RuntimeMoniker.CoreRt60)]
    [RPlotExporter]
    [MemoryDiagnoser]
    public class Filters
    {
        
        private double[] _data = new double[19200];
        

        [GlobalSetup]
        public void Setup()
        {
            for (var i = 0; i < _data.Length; i++)
            {
                _data[i] = Math.Sin(i * Math.PI / 180);
            }
            
        }
         
        [Benchmark]
        public void BiQuad()
        {
            BiQuad filter = new LowpassFilter(96_000, 32_000);
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = filter.Process(_data[i]);
            }

        }

        [Benchmark]
        public void CustomLowPassElliptic8kHzFilter()
        {

            var filter = new CustomLowPassElliptic8kHzFilter();
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = filter.Process(_data[i]);
            }

        }

        [Benchmark]
        public void CustomLowPassElliptic16kHzFilter()
        {

            var filter = new CustomLowPassElliptic16kHzFilter();
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = filter.Process(_data[i]);
            }

        }

        [Benchmark]
        public void CustomHighPassElliptic8kHzFilter()
        {

            var filter = new CustomHighPassElliptic8kHzFilter();
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = filter.Process(_data[i]);
            }

        }

        [Benchmark]
        public void CustomHighPassElliptic16kHzFilter()
        {

            var filter = new CustomHighPassElliptic16kHzFilter();
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = filter.Process(_data[i]);
            }

        }




    }
}
