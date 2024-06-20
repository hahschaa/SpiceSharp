using NUnit.Framework;
using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;
using System.Collections.Generic;
using System.Numerics;

namespace SpiceSharpTest.Simulations
{
    [TestFixture]
    public class ACTests
    {
        [Test]
        public void When_ACRerun_Expect_Same()
        {
            // Create the circuit
            var ckt = new Circuit(
                new VoltageSource("V1", "in", "0", 10.0).SetParameter("acmag", 1.0),
                new Resistor("R1", "in", "out", 10),
                new Capacitor("C1", "out", "0", 20)
            );

            // Create the ac analysis
            var ac = new AC("ac 1", new DecadeSweep(1, 1e9, 10));
            var export = new ComplexVoltageExport(ac, "out");

            // Run the simulation a first time for building the reference values
            var r = new List<Complex>();
            void BuildReference(object sender, ExportDataEventArgs args) => r.Add(export.Value);
            ac.ExportSimulationData += BuildReference;
            ac.Run(ckt);
            ac.ExportSimulationData -= BuildReference;

            // Rerun the simulation for building the reference values
            int index = 0;
            void CheckReference(object sender, ExportDataEventArgs args)
            {
                Assert.AreEqual(r[index].Real, export.Value.Real, 1e-20);
                Assert.AreEqual(r[index++].Imaginary, export.Value.Imaginary, 1e-20);
            }
            ac.ExportSimulationData += CheckReference;
            ac.Rerun();
            ac.ExportSimulationData -= CheckReference;
        }

        [Test]
        public void IteratorTest()
        {
            // Create the circuit
            var voltage = 1d;
            var ckt = new Circuit(
                new VoltageSource("V1", "in", "0", 0).SetParameter("acmag", voltage),
                new Resistor("R1", "in", "out", 10),
                new Capacitor("C1", "out", "0", 20e-6)
            );

            // Create the ac analysis
            var ac = new AC("ac 1");
            var export = new ComplexPropertyExport(ac, "R1", "i");
            var iterator = ac.CreateIterator(ckt);

            var frequencies = new double[] { 1000, 5000, 10_000, 50_000 };
            var expectedImpedance = new[] { 12.7799, 10.126, 10.0326, 10.0013 };

            for (var i = 0; i < frequencies.Length; i++)
            {
                double f = frequencies[i];
                var result = iterator.ExecuteSingle(f);
                Assert.That(result.Frequency, Is.EqualTo(f).Within(0.1).Percent);
                var current = export.Value.Magnitude;
                var impedance = voltage / current;
                Assert.That(impedance, Is.EqualTo(expectedImpedance[i]).Within(0.1).Percent);
            }
        }
    }
}
