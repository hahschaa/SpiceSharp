﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;
using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;

namespace SpiceSharpTest.Models
{
    [TestClass]
    public class MOS1Tests : Framework
    {
        /// <summary>
        /// Create a MOSFET
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="d">Drain</param>
        /// <param name="g">Gate</param>
        /// <param name="s">Source</param>
        /// <param name="b">Bulk</param>
        /// <param name="modelname">Model name</param>
        /// <param name="modelparams">Model parameters</param>
        /// <returns></returns>
        protected Mosfet1 CreateMOS1(Identifier name, Identifier d, Identifier g, Identifier s, Identifier b,
            Identifier modelname, string modelparams)
        {
            // Create model
            Mosfet1Model model = new Mosfet1Model(modelname);
            ApplyParameters(model, modelparams);

            // Create mosfet
            Mosfet1 mos = new Mosfet1(name);
            mos.SetModel(model);
            mos.Connect(d, g, s, b);
            return mos;
        }

        [TestMethod]
        public void When_MOS1DC_Expect_Spice3f5Reference()
        {
            /*
             * Mosfet biased by voltage sources
             * Current is expected to behave like the reference. Reference is from Spice 3f5.
             * The model is part from the ntd20n06 (OnSemi) device.
             */
            // Create circuit
            Circuit ckt = new Circuit();
            ckt.Objects.Add(
                new VoltageSource("V1", "g", "0", 0.0),
                new VoltageSource("V2", "d", "0", 0),
                CreateMOS1("M1", "d", "g", "0", "0",
                    "MM", "IS=1e-32 VTO=3.03646 LAMBDA=0 KP=5.28747 CGSO=6.5761e-06 CGDO=1e-11")
                );

            // Create simulation
            DC dc = new DC("dc", new SweepConfiguration[] {
                new SweepConfiguration("V2", 0, 5, 0.5),
                new SweepConfiguration("V1", 0, 5, 0.5)
            });

            // Create exports
            Export<double>[] exports = { new RealPropertyExport(dc, "V2", "i") };

            // Create references
            double[][] references = new double[1][];
            references[0] = new double[] { -9.806842412468131e-31, 0.000000000000000e+00, 0.000000000000000e+00, 0.000000000000000e+00, 0.000000000000000e+00, 0.000000000000000e+00, 0.000000000000000e+00, 0.000000000000000e+00, 0.000000000000000e+00, 0.000000000000000e+00, 0.000000000000000e+00, -5.000000000000000e-13, -5.000000000000000e-13, -5.000000000000000e-13, -5.000000000000000e-13, -5.000000000000000e-13, -5.000000000000000e-13, -5.000000000000000e-13, -5.680575723780237e-01, -1.886410671900499e+00, -3.208278171900499e+00, -4.530145671900499e+00, -1.000000000000000e-12, -1.000000000000000e-12, -1.000000000000000e-12, -1.000000000000000e-12, -1.000000000000000e-12, -1.000000000000000e-12, -1.000000000000000e-12, -5.680575723785246e-01, -2.454468244278523e+00, -5.094688843800999e+00, -7.738423843800998e+00, -1.500000000000000e-12, -1.500000000000000e-12, -1.500000000000000e-12, -1.500000000000000e-12, -1.500000000000000e-12, -1.500000000000000e-12, -1.500000000000000e-12, -5.680575723790238e-01, -2.454468244279024e+00, -5.662746416179022e+00, -9.624834515701494e+00, -2.000000000000000e-12, -2.000000000000000e-12, -2.000000000000000e-12, -2.000000000000000e-12, -2.000000000000000e-12, -2.000000000000000e-12, -2.000000000000000e-12, -5.680575723795247e-01, -2.454468244279525e+00, -5.662746416179523e+00, -1.019289208807952e+01, -2.500000000000000e-12, -2.500000000000000e-12, -2.500000000000000e-12, -2.500000000000000e-12, -2.500000000000000e-12, -2.500000000000000e-12, -2.500000000000000e-12, -5.680575723800239e-01, -2.454468244280026e+00, -5.662746416180024e+00, -1.019289208808002e+01, -3.000000000000000e-12, -3.000000000000000e-12, -3.000000000000000e-12, -3.000000000000000e-12, -3.000000000000000e-12, -3.000000000000000e-12, -3.000000000000000e-12, -5.680575723805248e-01, -2.454468244280523e+00, -5.662746416180521e+00, -1.019289208808052e+01, -3.500000000000000e-12, -3.500000000000000e-12, -3.500000000000000e-12, -3.500000000000000e-12, -3.500000000000000e-12, -3.500000000000000e-12, -3.500000000000000e-12, -5.680575723810239e-01, -2.454468244281024e+00, -5.662746416181022e+00, -1.019289208808102e+01, -4.000000000000000e-12, -4.000000000000000e-12, -4.000000000000000e-12, -4.000000000000000e-12, -4.000000000000000e-12, -4.000000000000000e-12, -4.000000000000000e-12, -5.680575723815249e-01, -2.454468244281525e+00, -5.662746416181523e+00, -1.019289208808152e+01, -4.500000000000000e-12, -4.500000000000000e-12, -4.500000000000000e-12, -4.500000000000000e-12, -4.500000000000000e-12, -4.500000000000000e-12, -4.500000000000000e-12, -5.680575723820240e-01, -2.454468244282026e+00, -5.662746416182024e+00, -1.019289208808202e+01, -5.000000000000000e-12, -5.000000000000000e-12, -5.000000000000000e-12, -5.000000000000000e-12, -5.000000000000000e-12, -5.000000000000000e-12, -5.000000000000000e-12, -5.680575723825250e-01, -2.454468244282523e+00, -5.662746416182522e+00, -1.019289208808252e+01 };

            // Run simulation
            AnalyzeDC(dc, ckt, exports, references);
        }

        [TestMethod]
        public void When_MOS1CommonSourceAmplifierSmallSignal_Expect_Spice3f5Reference()
        {
            /*
             * Simple common source amplifier
             * Output voltage gain is expected to match reference. Reference by Spice 3f5.
             */
            // Build circuit
            Circuit ckt = new Circuit();
            ckt.Objects.Add(
                new VoltageSource("V1", "in", "0", 0.0),
                new VoltageSource("V2", "vdd", "0", 5.0),
                new Resistor("R1", "vdd", "out", 10.0e3),
                new Resistor("R2", "out", "g", 10.0e3),
                new Capacitor("Cin", "in", "g", 1e-6),
                CreateMOS1("M1", "out", "g", "0", "0",
                    "MM", "IS=1e-32 VTO=3.03646 LAMBDA=0 KP=5.28747 CGSO=6.5761e-06 CGDO=1e-11")
                );
            ckt.Objects["V1"].ParameterSets.SetProperty("acmag", 1.0);

            // Create simulation
            AC ac = new AC("ac", new SpiceSharp.Simulations.Sweeps.DecadeSweep(10, 10e9, 5));

            // Create exports
            Export<Complex>[] exports = { new ComplexVoltageExport(ac, "out") };

            // Create references
            double[] riref = { -1.725813644006744e-03, -6.255567388468394e-01, -4.334997991949969e-03, -9.914292083082819e-01, -1.088870790416865e-02, -1.571263986482406e+00, -2.734921201531804e-02, -2.490104807455213e+00, -6.868558931531524e-02, -3.945830610208745e+00, -1.724514213823252e-01, -6.250857335307440e+00, -4.326808652667278e-01, -9.895562775467608e+00, -1.083718679773610e+00, -1.563829379084251e+01, -2.702649136180583e+00, -2.460721571760895e+01, -6.668576859467226e+00, -3.830945453134210e+01, -1.603760999419659e+01, -5.813162362216580e+01, -3.639300462484001e+01, -8.323207309729999e+01, -7.356417703660050e+01, -1.061546849651946e+02, -1.239747410542734e+02, -1.128771300396776e+02, -1.704839090551324e+02, -9.793908744049646e+01, -2.004160562764431e+02, -7.264486862286662e+01, -2.154771197776309e+02, -4.928026345664456e+01, -2.221224344754558e+02, -3.205256931555954e+01, -2.248834695491267e+02, -2.047502071380190e+01, -2.260018549839163e+02, -1.298284168009341e+01, -2.264501941354974e+02, -8.207439641802505e+00, -2.266291765967058e+02, -5.181955183396055e+00, -2.267005095543059e+02, -3.269540294698130e+00, -2.267289201967292e+02, -2.061484698952238e+00, -2.267402326137311e+02, -1.298056697906094e+00, -2.267447363683708e+02, -8.147282590010622e-01, -2.267465291092059e+02, -5.072375792911769e-01, -2.267472421017376e+02, -3.092289341889395e-01, -2.267475241460346e+02, -1.779661469529321e-01, -2.267476318976589e+02, -8.511710389733645e-02, -2.267476634094340e+02, -1.064055031339878e-02, -2.267476473568060e+02, 6.153924230708851e-02, -2.267475691320166e+02, 1.470022622053010e-01, -2.267473575512703e+02, 2.641955750327690e-01, -2.267468200791099e+02, 4.384147465157958e-01, -2.267454676298356e+02, 7.072623998910071e-01, -2.267420695496819e+02, 1.128758783561459e+00, -2.267335340265157e+02, 1.793841675558252e+00, -2.267120964325891e+02, 2.845900637604031e+00, -2.266582653689669e+02, 4.511350906221913e+00, -2.265231600192664e+02, 7.147007490994596e+00, -2.261844969618107e+02, 1.131116522055704e+01, -2.253382440974266e+02, 1.786070290176738e+01, -2.232401028216283e+02, 2.804520605312238e+01, -2.181374806913368e+02, 4.343740586524000e+01, -2.062891648647118e+02, 6.512151536164929e+01 };
            Complex[][] references = new Complex[1][];
            references[0] = new Complex[riref.Length / 2];
            for (int i = 0; i < riref.Length; i += 2)
            {
                references[0][i / 2] = new Complex(riref[i], riref[i + 1]);
            }

            // Run test
            AnalyzeAC(ac, ckt, exports, references);
        }

        [TestMethod]
        public void When_MOS1SwitchTransient_Expect_Spice3f5Reference()
        {
            // Create circuit
            Circuit ckt = new Circuit();
            ckt.Objects.Add(
                new VoltageSource("V1", "in", "0", new Pulse(1, 5, 1e-6, 1e-9, 0.5e-6, 2e-6, 6e-6)),
                new VoltageSource("Vsupply", "vdd", "0", 5),
                new Resistor("R1", "out", "vdd", 1.0e3),
                CreateMOS1("M1", "out", "in", "0", "0",
                    "MM", "IS=1e-32 VTO=3.03646 LAMBDA=0 KP=5.28747 CGSO=6.5761e-06 CGDO=1e-11")
                );

            // Create simulation
            Transient tran = new Transient("tran", 1e-9, 10e-6);

            // Create exports
            Export<double>[] exports = { new RealVoltageExport(tran, "out") };

            // Create references
            double[][] references = new double[1][];
            references[0] = new double[] { 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 4.999999995000000e+00, 5.003960391035683e+00, 5.004038814719480e+00, 5.003961923904320e+00, 5.822926169346066e-04, 4.810560374122478e-04, 4.816080793679862e-04, 4.816080824751261e-04, 4.816080793679973e-04, 4.816080824751207e-04, 4.816080793680000e-04, 4.816080824751193e-04, 4.816080793680006e-04, 4.816080824751191e-04, 4.816080793680011e-04, 4.816080824751189e-04, 4.816080793680010e-04, 4.816080824751191e-04, 4.816080793680009e-04, 4.816080824751189e-04, 4.816080793680010e-04, 4.816080824751189e-04, 4.816080793680010e-04, 4.816080824751189e-04, 4.816080793680010e-04, 4.816080824751189e-04, 4.816080793680010e-04, 4.816080824751188e-04, 5.243402581575843e-04, 6.374704766839068e-04, 1.121520168458353e-03, -1.720170200560100e+01, 4.999968188570249e+00, 5.000015800431791e+00, 4.999968190756332e+00, 4.999999991842038e+00, 4.999999998157336e+00, 4.999999991842977e+00, 4.999999998156865e+00, 4.999999991843213e+00, 4.999999998156724e+00, 4.999999991843340e+00, 4.999999998156598e+00, 4.999999991843466e+00, 4.999999998156472e+00, 4.999999991843592e+00, 4.999999998156345e+00, 4.999999991843718e+00, 4.999999998156220e+00, 4.999999991843845e+00, 4.999999998156093e+00, 4.999999991843971e+00, 4.999999998155968e+00, 4.999999991844097e+00, 4.999999998155841e+00, 4.999999991844228e+00, 5.003960391004448e+00, 5.004038814750084e+00, 5.003961923874295e+00, 5.822926152947390e-04, 4.810560374114067e-04, 4.816080793679895e-04, 4.816080824751229e-04, 4.816080793680006e-04, 4.816080824751175e-04, 4.816080793680033e-04, 4.816080824751161e-04, 4.816080793680040e-04, 4.816080824751158e-04, 4.816080793680041e-04, 4.816080824751156e-04, 4.816080793680043e-04, 4.816080824751156e-04, 4.816080793680043e-04, 4.816080824751157e-04, 4.816080793680042e-04, 4.816080824751155e-04, 4.816080793680042e-04, 4.816080824751155e-04, 4.816080793680042e-04, 4.816080824751155e-04, 4.816080793680042e-04, 4.816080824751157e-04, 5.243402581575805e-04, 6.374704766839011e-04, 1.121520168458326e-03, -1.720170200561121e+01, 4.999968188570249e+00, 5.000015800431791e+00, 4.999968190756332e+00, 4.999999991842038e+00, 4.999999998157336e+00, 4.999999991842977e+00, 4.999999998156865e+00, 4.999999991843213e+00, 4.999999998156719e+00 };

            // Run test
            AnalyzeTransient(tran, ckt, exports, references);
        }

        [TestMethod]
        public void When_MOS1CommonSourceAmplifierNoise_Expect_Spice3f5Reference()
        {
            // Create circuit
            Circuit ckt = new Circuit();
            ckt.Objects.Add(
                new VoltageSource("V1", "in", "0", 0.0),
                new VoltageSource("V2", "vdd", "0", 5.0),
                new Resistor("R1", "vdd", "out", 10e3),
                new Resistor("R2", "out", "g", 10e3),
                new Capacitor("Cin", "in", "g", 1e-6),
                CreateMOS1("M1", "out", "g", "0", "0",
                    "MM", "IS = 1e-32 VTO = 3.03646 LAMBDA = 0 KP = 5.28747 CGSO = 6.5761e-06 CGDO = 1e-11 KF = 1e-25")
                );
            ckt.Objects["V1"].ParameterSets.SetProperty("acmag", 1.0);
            ckt.Objects["M1"].ParameterSets.SetProperty("w", 100e-6);
            ckt.Objects["M1"].ParameterSets.SetProperty("l", 100e-6);

            // Create simulation, exports and references
            Noise noise = new Noise("noise", "out", "V1", new SpiceSharp.Simulations.Sweeps.DecadeSweep(10, 10e9, 10));
            Export<double>[] exports = { new InputNoiseDensityExport(noise), new OutputNoiseDensityExport(noise) };
            double[][] references = new double[2][];
            references[0] = new double[] { 2.815379564675864e-12, 1.644868840839549e-12, 1.010121306101684e-12, 6.537880726721799e-13, 4.448524865997785e-13, 3.160333336157883e-13, 2.323257132196350e-13, 1.751655483262268e-13, 1.344386048382044e-13, 1.044322995501785e-13, 8.177248667148335e-14, 6.436215686569252e-14, 5.082789164771354e-14, 4.022525188195949e-14, 3.187747693804598e-14, 2.528380544270212e-14, 2.006491938974725e-14, 1.592876491956225e-14, 1.264799208788146e-14, 1.004433497404203e-14, 7.977357413355818e-15, 6.336091407348683e-15, 5.032685020810389e-15, 3.997501981674192e-15, 3.175301978989497e-15, 2.522243079680978e-15, 2.003518973571566e-15, 1.591491353272600e-15, 1.264211017007844e-15, 1.004245449843196e-15, 7.977486981440409e-16, 6.337231268378405e-16, 5.034333047893085e-16, 3.999405843517482e-16, 3.177334786554185e-16, 2.524340970390206e-16, 2.005649771592597e-16, 1.593638826030865e-16, 1.266366961773762e-16, 1.006405713131741e-16, 7.999111715405073e-17, 6.358867367592469e-17, 5.055975025237714e-17, 4.021050881748337e-17, 3.198981431322685e-17, 2.545988466054381e-17, 2.027297722561883e-17, 1.615287023394526e-17, 1.288015294111360e-17, 1.028054120362606e-17, 8.215596208788723e-18, 6.575352100860561e-18, 5.272459896934062e-18, 4.237535834307345e-18, 3.415466431655031e-18, 2.762473494901996e-18, 2.243782768585602e-18, 1.831772079846699e-18, 1.504500356938407e-18, 1.244539187109050e-18, 1.038044690046574e-18, 8.740202807553306e-19, 7.437310612966987e-19, 6.402386556161864e-19, 5.580317157140351e-19, 4.927324222647428e-19, 4.408633497727489e-19, 3.996622809833569e-19, 3.669351087408033e-19, 3.409389917808688e-19, 3.202895420778084e-19, 3.038871011332858e-19, 2.908581791507522e-19, 2.805089385176244e-19, 2.722882444208535e-19, 2.657583149061083e-19, 2.605714073887111e-19, 2.564513000872298e-19, 2.531785821973098e-19, 2.505789694518999e-19, 2.485140228257554e-19, 2.468737761164706e-19, 2.455708797860733e-19, 2.445359491890543e-19, 2.437138694434664e-19, 2.430608601350282e-19, 2.425421434899278e-19, 2.421300917602111e-19, 2.418027550399525e-19, 2.415426909176026e-19, 2.413360333294781e-19 };
            references[1] = new double[] { 1.101938772677249e-12, 1.020351798553683e-12, 9.930914904110209e-13, 1.018704151068570e-12, 1.098549779218339e-12, 1.236872518232169e-12, 1.441022728380007e-12, 1.721840550874621e-12, 2.094217487153999e-12, 2.577858124390719e-12, 3.198267097692130e-12, 3.987983099859464e-12, 4.988064706261346e-12, 6.249786808519018e-12, 7.836401914012315e-12, 9.824601582961330e-12, 1.230487975385098e-11, 1.537918793701761e-11, 1.915286183784488e-11, 2.371564318075358e-11, 2.910415518231322e-11, 3.523777729653164e-11, 4.182805832714524e-11, 4.828952148482491e-11, 5.372934740827330e-11, 5.712303094004328e-11, 5.769531786489053e-11, 5.530480561891652e-11, 5.052178742786626e-11, 4.432835400406183e-11, 3.770017350647562e-11, 3.134531801206027e-11, 2.565584102156048e-11, 2.077916019829584e-11, 1.671370313489508e-11, 1.338397388087558e-11, 1.068731671781018e-11, 8.518878155014311e-12, 6.783039968978641e-12, 5.397456921217182e-12, 4.293446467982328e-12, 3.414789199740856e-12, 2.715987838819629e-12, 2.160479000532345e-12, 1.719006299308645e-12, 1.368223534346394e-12, 1.089532434335449e-12, 8.681329107747076e-13, 6.922553133235565e-13, 5.525438940163232e-13, 4.415637162479891e-13, 3.534072935825517e-13, 2.833812867202454e-13, 2.277572142155273e-13, 1.835732224046120e-13, 1.484765190559835e-13, 1.205981604962914e-13, 9.845356466124910e-14, 8.086347233940640e-14, 6.689115769628357e-14, 5.579254942299496e-14, 4.697660884412611e-14, 3.997385626141668e-14, 3.441137009036628e-14, 2.999292772256991e-14, 2.648323069715085e-14, 2.369537418245358e-14, 2.148089349130362e-14, 1.972185755582100e-14, 1.832458826459864e-14, 1.721467110986965e-14, 1.633299130424805e-14, 1.563258405234841e-14, 1.507613088730997e-14, 1.463396817940087e-14, 1.428250087013048e-14, 1.400293554605381e-14, 1.378026315480551e-14, 1.360243367639641e-14, 1.345967328032927e-14, 1.334389898454301e-14, 1.324818629450979e-14, 1.316624109090014e-14, 1.309181726819219e-14, 1.301800572736213e-14, 1.293629969876902e-14, 1.283532383362070e-14, 1.269912466960263e-14, 1.250502043061476e-14, 1.222133156026873e-14, 1.180607868779705e-14 };

            // Run test
            AnalyzeNoise(noise, ckt, exports, references);
        }
    }
}
