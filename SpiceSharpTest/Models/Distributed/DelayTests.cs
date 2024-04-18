﻿using NUnit.Framework;
using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;
using System;
using System.Numerics;

namespace SpiceSharpTest.Models
{
    [TestFixture]
    public class DelayTests : Framework
    {
        [Test]
        public void When_SimplePulsedTransient_Expect_Reference()
        {
            // Build the circuit
            var ckt = new Circuit(
                new VoltageSource("V1", "in", "0", new Pulse(0, 5, 1e-7, 1e-7, 1e-7, 1e-5, 2e-5)),
                new VoltageDelay("Delay1", "out", "0", "in", "0", 0.5e-5)
            );

            // Build the simulation
            var tran = new Transient("tran", 1e-7, 10e-5);
            var exports = new IExport<double>[]
            {
                new GenericExport<double>(tran, () => tran.GetState<IIntegrationMethod>().Time),
                new RealVoltageExport(tran, "in"),
                new RealVoltageExport(tran, "out"),
            };
            double[][] references =
            [
                [
                    0, 1E-09, 2E-09, 4E-09, 8E-09, 1.6E-08, 3.2E-08, 6.4E-08, 1E-07, 1.064E-07, 1.192E-07, 1.448E-07,
                    1.96E-07, 2E-07, 2.08E-07, 2.24E-07, 2.55999999999999E-07, 3.19999999999999E-07,
                    4.47999999999997E-07, 7.03999999999994E-07, 1.21599999999999E-06, 2.23999999999998E-06,
                    4.23999999999998E-06, 6.23999999999998E-06, 8.23999999999998E-06, 1.02E-05, 1.021E-05, 1.023E-05,
                    1.027E-05, 1.03E-05, 1.0308E-05, 1.0324E-05, 1.0356E-05, 1.042E-05, 1.0548E-05, 1.0804E-05,
                    1.1316E-05, 1.234E-05, 1.434E-05, 1.634E-05, 1.834E-05, 2.01E-05, 2.011E-05, 2.013E-05, 2.017E-05,
                    2.02E-05, 2.0208E-05, 2.0224E-05, 2.0256E-05, 2.032E-05, 2.0448E-05, 2.0704E-05, 2.1216E-05,
                    2.224E-05, 2.424E-05, 2.624E-05, 2.824E-05, 3.02E-05, 3.021E-05, 3.023E-05, 3.027E-05, 3.03E-05,
                    3.0308E-05, 3.0324E-05, 3.0356E-05, 3.042E-05, 3.0548E-05, 3.0804E-05, 3.1316E-05, 3.234E-05,
                    3.434E-05, 3.634E-05, 3.834E-05, 4.01E-05, 4.011E-05, 4.013E-05, 4.017E-05, 4.02E-05, 4.0208E-05,
                    4.0224E-05, 4.0256E-05, 4.032E-05, 4.0448E-05, 4.0704E-05, 4.1216E-05, 4.22400000000001E-05,
                    4.42400000000001E-05, 4.62400000000001E-05, 4.82400000000001E-05, 5.02E-05, 5.021E-05, 5.023E-05,
                    5.027E-05, 5.03E-05, 5.0308E-05, 5.0324E-05, 5.0356E-05, 5.042E-05, 5.0548E-05, 5.0804E-05,
                    5.1316E-05, 5.23400000000001E-05, 5.43400000000001E-05, 5.63400000000001E-05, 5.83400000000001E-05,
                    6.01E-05, 6.011E-05, 6.013E-05, 6.017E-05, 6.02E-05, 6.0208E-05, 6.0224E-05, 6.0256E-05, 6.032E-05,
                    6.0448E-05, 6.0704E-05, 6.1216E-05, 6.22400000000001E-05, 6.42400000000001E-05,
                    6.62400000000001E-05, 6.82400000000001E-05, 7.02E-05, 7.021E-05, 7.023E-05, 7.027E-05, 7.03E-05,
                    7.0308E-05, 7.0324E-05, 7.0356E-05, 7.042E-05, 7.0548E-05, 7.0804E-05, 7.1316E-05,
                    7.23400000000001E-05, 7.43400000000001E-05, 7.63400000000001E-05, 7.83400000000001E-05, 8.01E-05,
                    8.011E-05, 8.013E-05, 8.017E-05, 8.02E-05, 8.0208E-05, 8.0224E-05, 8.0256E-05, 8.032E-05,
                    8.0448E-05, 8.0704E-05, 8.1216E-05, 8.22400000000001E-05, 8.42400000000001E-05,
                    8.62400000000001E-05, 8.82400000000001E-05, 9.02E-05, 9.021E-05, 9.023E-05, 9.027E-05, 9.03E-05,
                    9.0308E-05, 9.0324E-05, 9.0356E-05, 9.042E-05, 9.0548E-05, 9.0804E-05, 9.1316E-05,
                    9.23400000000001E-05, 9.43400000000001E-05, 9.63400000000001E-05, 9.83400000000001E-05, 0.0001
                ],
                [
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0.32, 0.960000000000001, 2.24, 4.8, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 4.50000000000009, 3.5000000000001, 1.50000000000012, 4.79616346638068E-14, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0.50000000000008, 1.50000000000007, 3.50000000000005, 4.99999999999995, 5, 5, 5, 5,
                    5, 5, 5, 5, 5, 5, 5, 5, 4.49999999999992, 3.49999999999993, 1.49999999999995, 4.79616346638068E-14,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.499999999999741, 1.4999999999999, 3.49999999999988,
                    4.99999999999978, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4.49999999999992, 3.49999999999976,
                    1.49999999999978, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.50000000000008, 1.50000000000024,
                    3.50000000000022, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4.50000000000026, 3.5000000000001,
                    1.49999999999978, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.499999999999741, 1.4999999999999,
                    3.50000000000022, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4.50000000000026, 3.5000000000001,
                    1.49999999999978, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                ],
                [
                    0.0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 5, 0, 0, 0
                ],
            ];

            // Analyze
            AnalyzeTransient(tran, ckt, exports, references);
            DestroyExports(exports);
        }

        [Test]
        public void When_SimpleSineTransient_Expect_NoException()
        {
            var ckt = new Circuit(
                new VoltageSource("V1", "1", "0", new Sine(0, 5, 50, 0, 0, 90)),
                new VoltageDelay("BVD1", "2", "0", "1", "0", 1e-2),
                new Resistor("R1", "1", "0", 10),
                new Resistor("R2", "2", "0", 10)
            );

            var tran = new Transient("tran", 1e-8, 1e-5);
            tran.Run(ckt);
        }

        [Test]
        public void When_BreakpointsTransient_Expect_Reference()
        {
            // Build the circuit
            var ckt = new Circuit(
                new VoltageSource("V1", "in", "0", new Pulse(0, 5, 1e-7, 1e-7, 1e-7, 1e-5, 2e-5)),
                new VoltageDelay("Delay1", "out", "0", "in", "0", 0.5e-5)
                    // This will make the delay element add a breakpoint when the input changes rapidly
                    .SetParameter("reltol", 0.5)
            );

            // Build the simulation
            var tran = new Transient("tran", 1e-7, 10e-5);
            var exports = new IExport<double>[]
            {
                new GenericExport<double>(tran, () => tran.GetState<IIntegrationMethod>().Time),
                new RealVoltageExport(tran, "in"),
                new RealVoltageExport(tran, "out"),
            };
            double[][] references =
            [
                [
                    0, 1E-09, 2E-09, 4E-09, 8E-09, 1.6E-08, 3.2E-08, 6.4E-08, 1E-07, 1.064E-07, 1.192E-07, 1.448E-07,
                    1.96E-07, 2E-07, 2.08E-07, 2.24E-07, 2.55999999999999E-07, 3.19999999999999E-07,
                    4.47999999999997E-07, 7.03999999999994E-07, 1.21599999999999E-06, 2.23999999999998E-06,
                    4.23999999999998E-06, 5.1E-06, 5.11E-06, 5.13E-06, 5.17E-06, 5.2E-06, 5.208E-06, 5.224E-06,
                    5.256E-06, 5.32E-06, 5.448E-06, 5.704E-06, 6.216E-06, 7.24E-06, 9.24E-06, 1.02E-05, 1.021E-05,
                    1.023E-05, 1.027E-05, 1.03E-05, 1.0308E-05, 1.0324E-05, 1.0356E-05, 1.042E-05, 1.0548E-05,
                    1.0804E-05, 1.1316E-05, 1.234E-05, 1.434E-05, 1.52E-05, 1.521E-05, 1.523E-05, 1.527E-05, 1.53E-05,
                    1.5308E-05, 1.5324E-05, 1.5356E-05, 1.542E-05, 1.5548E-05, 1.5804E-05, 1.6316E-05,
                    1.73400000000001E-05, 1.93400000000001E-05, 2.01E-05, 2.011E-05, 2.013E-05, 2.017E-05, 2.02E-05,
                    2.0208E-05, 2.0224E-05, 2.0256E-05, 2.032E-05, 2.0448E-05, 2.0704E-05, 2.1216E-05, 2.224E-05,
                    2.424E-05, 2.51E-05, 2.511E-05, 2.513E-05, 2.517E-05, 2.52E-05, 2.5208E-05, 2.5224E-05, 2.5256E-05,
                    2.532E-05, 2.5448E-05, 2.5704E-05, 2.6216E-05, 2.724E-05, 2.924E-05, 3.02E-05, 3.021E-05, 3.023E-05,
                    3.027E-05, 3.03E-05, 3.0308E-05, 3.0324E-05, 3.0356E-05, 3.042E-05, 3.0548E-05, 3.0804E-05,
                    3.1316E-05, 3.234E-05, 3.434E-05, 3.52E-05, 3.521E-05, 3.523E-05, 3.527E-05, 3.53E-05, 3.5308E-05,
                    3.5324E-05, 3.5356E-05, 3.542E-05, 3.5548E-05, 3.5804E-05, 3.6316E-05, 3.73400000000001E-05,
                    3.93400000000001E-05, 4.01E-05, 4.011E-05, 4.013E-05, 4.017E-05, 4.02E-05, 4.0208E-05, 4.0224E-05,
                    4.0256E-05, 4.032E-05, 4.0448E-05, 4.0704E-05, 4.1216E-05, 4.22400000000001E-05,
                    4.42400000000001E-05, 4.51E-05, 4.511E-05, 4.513E-05, 4.517E-05, 4.52E-05, 4.5208E-05, 4.5224E-05,
                    4.5256E-05, 4.532E-05, 4.5448E-05, 4.5704E-05, 4.6216E-05, 4.724E-05, 4.92400000000001E-05,
                    5.02E-05, 5.021E-05, 5.023E-05, 5.027E-05, 5.03E-05, 5.0308E-05, 5.0324E-05, 5.0356E-05, 5.042E-05,
                    5.0548E-05, 5.0804E-05, 5.1316E-05, 5.23400000000001E-05, 5.43400000000001E-05, 5.52E-05, 5.521E-05,
                    5.523E-05, 5.527E-05, 5.53E-05, 5.5308E-05, 5.5324E-05, 5.5356E-05, 5.542E-05, 5.5548E-05,
                    5.5804E-05, 5.6316E-05, 5.73400000000001E-05, 5.93400000000001E-05, 6.01E-05, 6.011E-05, 6.013E-05,
                    6.017E-05, 6.02E-05, 6.0208E-05, 6.0224E-05, 6.0256E-05, 6.032E-05, 6.0448E-05, 6.0704E-05,
                    6.1216E-05, 6.22400000000001E-05, 6.42400000000001E-05, 6.51E-05, 6.511E-05, 6.513E-05, 6.517E-05,
                    6.52E-05, 6.5208E-05, 6.5224E-05, 6.5256E-05, 6.532E-05, 6.5448E-05, 6.5704E-05, 6.6216E-05,
                    6.72400000000001E-05, 6.92400000000001E-05, 7.02E-05, 7.021E-05, 7.023E-05, 7.027E-05, 7.03E-05,
                    7.0308E-05, 7.0324E-05, 7.0356E-05, 7.042E-05, 7.0548E-05, 7.0804E-05, 7.1316E-05,
                    7.23400000000001E-05, 7.43400000000001E-05, 7.52E-05, 7.521E-05, 7.523E-05, 7.527E-05, 7.53E-05,
                    7.5308E-05, 7.5324E-05, 7.5356E-05, 7.542E-05, 7.5548E-05, 7.5804E-05, 7.6316E-05,
                    7.73400000000001E-05, 7.93400000000001E-05, 8.01E-05, 8.011E-05, 8.013E-05, 8.017E-05, 8.02E-05,
                    8.0208E-05, 8.0224E-05, 8.0256E-05, 8.032E-05, 8.0448E-05, 8.0704E-05, 8.1216E-05,
                    8.22400000000001E-05, 8.42400000000001E-05, 8.51E-05, 8.511E-05, 8.513E-05, 8.517E-05, 8.52E-05,
                    8.5208E-05, 8.5224E-05, 8.5256E-05, 8.532E-05, 8.5448E-05, 8.5704E-05, 8.6216E-05,
                    8.72400000000001E-05, 8.92400000000001E-05, 9.02E-05, 9.021E-05, 9.023E-05, 9.027E-05, 9.03E-05,
                    9.0308E-05, 9.0324E-05, 9.0356E-05, 9.042E-05, 9.0548E-05, 9.0804E-05, 9.1316E-05,
                    9.23400000000001E-05, 9.43400000000001E-05, 9.52E-05, 9.521E-05, 9.523E-05, 9.527E-05, 9.53E-05,
                    9.5308E-05, 9.5324E-05, 9.5356E-05, 9.542E-05, 9.5548E-05, 9.5804E-05, 9.6316E-05,
                    9.73400000000001E-05, 9.93400000000001E-05, 0.0001
                ],
                [
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0.32, 0.960000000000001, 2.24, 4.8, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4.50000000000009, 3.5000000000001, 1.50000000000012,
                    4.79616346638068E-14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0.50000000000008, 1.50000000000007, 3.50000000000005, 4.99999999999995, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4.49999999999992, 3.49999999999993, 1.49999999999995,
                    4.79616346638068E-14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0.499999999999741, 1.4999999999999, 3.49999999999988, 4.99999999999978, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4.49999999999992, 3.49999999999976, 1.49999999999978,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.50000000000008,
                    1.50000000000024, 3.50000000000022, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 5, 5, 5, 4.50000000000026, 3.5000000000001, 1.49999999999978, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.499999999999741, 1.4999999999999, 3.50000000000022, 5, 5,
                    5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4.50000000000026,
                    3.5000000000001, 1.49999999999978, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0
                ],
                [
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.49999999999999,
                    1.49999999999998, 3.49999999999996, 4.99999999999999, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 5, 5, 5, 5, 5, 5, 5, 5, 4.50000000000018, 3.50000000000018, 1.50000000000003,
                    4.79616346637966E-14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0.50000000000008, 1.50000000000007, 3.50000000000005, 4.99999999999995, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4.49999999999992, 3.49999999999976, 1.49999999999978,
                    4.79616346637864E-14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0.499999999999741, 1.4999999999999, 3.49999999999988, 4.99999999999978, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4.49999999999992, 3.49999999999976, 1.49999999999978,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.499999999999741,
                    1.4999999999999, 3.50000000000022, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                    5, 5, 5, 4.50000000000026, 3.5000000000001, 1.49999999999978, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.499999999999741, 1.4999999999999, 3.50000000000022, 5, 5, 5,
                    5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4.50000000000026, 3.5000000000001,
                    1.49999999999978, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                ],
            ];

            // Analyze
            AnalyzeTransient(tran, ckt, exports, references);
            DestroyExports(exports);
        }

        [Test]
        public void When_SimpleSmallSignal_Expect_Reference()
        {
            double delay = 1e-6;

            // Build the circuit
            var ckt = new Circuit(
                new VoltageSource("V1", "in", "0", 1.0)
                    .SetParameter("acmag", 1.0),
                new VoltageDelay("Delay1", "out", "0", "in", "0", delay));

            // Build the analysis
            var ac = new AC("ac", new DecadeSweep(1e-3, 1e5, 5));
            var exports = new IExport<Complex>[]
            {
                new ComplexVoltageExport(ac, "out")
            };
            var references = new Func<double, Complex>[]
            {
                frequency => Complex.Exp(-ac.GetState<IComplexSimulationState>().Laplace * delay)
            };

            // Analyze the AC behavior
            AnalyzeAC(ac, ckt, exports, references);
            DestroyExports(exports);
        }
    }
}
