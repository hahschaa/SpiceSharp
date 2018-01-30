﻿using System;
using SpiceSharp.Attributes;
using SpiceSharp.Simulations;
using SpiceSharp.IntegrationMethods;
using SpiceSharp.Sparse;
using SpiceSharp.Behaviors;
using SpiceSharp.Components.MosfetBehaviors;

namespace SpiceSharp.Components.MosfetBehaviors.Level1
{
    /// <summary>
    /// Transient behavior for a <see cref="Mosfet1"/>
    /// </summary>
    public class TransientBehavior : Behaviors.TransientBehavior, IConnectedBehavior
    {
        /// <summary>
        /// Necessary behaviors and parameters
        /// </summary>
        BaseParameters bp;
        ModelBaseParameters mbp;
        TemperatureBehavior temp;
        LoadBehavior load;
        ModelTemperatureBehavior modeltemp;

        /// <summary>
        /// Shared parameters
        /// </summary>
        [PropertyName("cbd"), PropertyInfo("Bulk-Drain capacitance")]
        public double Capbd { get; protected set; }
        [PropertyName("cbs"), PropertyInfo("Bulk-Source capacitance")]
        public double Capbs { get; protected set; }

        /// <summary>
        /// Nodes
        /// </summary>
        int drainNode, gateNode, sourceNode, bulkNode, sourceNodePrime, drainNodePrime;
        protected MatrixElement DrainDrainPtr { get; private set; }
        protected MatrixElement GateGatePtr { get; private set; }
        protected MatrixElement SourceSourcePtr { get; private set; }
        protected MatrixElement BulkBulkPtr { get; private set; }
        protected MatrixElement DrainPrimeDrainPrimePtr { get; private set; }
        protected MatrixElement SourcePrimeSourcePrimePtr { get; private set; }
        protected MatrixElement DrainDrainPrimePtr { get; private set; }
        protected MatrixElement GateBulkPtr { get; private set; }
        protected MatrixElement GateDrainPrimePtr { get; private set; }
        protected MatrixElement GateSourcePrimePtr { get; private set; }
        protected MatrixElement SourceSourcePrimePtr { get; private set; }
        protected MatrixElement BulkDrainPrimePtr { get; private set; }
        protected MatrixElement BulkSourcePrimePtr { get; private set; }
        protected MatrixElement DrainPrimeSourcePrimePtr { get; private set; }
        protected MatrixElement DrainPrimeDrainPtr { get; private set; }
        protected MatrixElement BulkGatePtr { get; private set; }
        protected MatrixElement DrainPrimeGatePtr { get; private set; }
        protected MatrixElement SourcePrimeGatePtr { get; private set; }
        protected MatrixElement SourcePrimeSourcePtr { get; private set; }
        protected MatrixElement DrainPrimeBulkPtr { get; private set; }
        protected MatrixElement SourcePrimeBulkPtr { get; private set; }
        protected MatrixElement SourcePrimeDrainPrimePtr { get; private set; }

        /// <summary>
        /// State variables
        /// </summary>
        protected StateDerivative Qgs { get; private set; }
        protected StateDerivative Qgd { get; private set; }
        protected StateDerivative Qgb { get; private set; }
        protected StateDerivative Qbd { get; private set; }
        protected StateDerivative Qbs { get; private set; }
        protected StateHistory Capgs { get; private set; }
        protected StateHistory Capgd { get; private set; }
        protected StateHistory Capgb { get; private set; }
        protected StateHistory Vgs { get; private set; }
        protected StateHistory Vds { get; private set; }
        protected StateHistory Vbs { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        public TransientBehavior(Identifier name) : base(name) { }

        /// <summary>
        /// Setup behavior
        /// </summary>
        /// <param name="provider">Data provider</param>
        public override void Setup(SetupDataProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            // Get parameters
            bp = provider.GetParameterSet<BaseParameters>(0);
            mbp = provider.GetParameterSet<ModelBaseParameters>(1);

            // Get behaviors
            temp = provider.GetBehavior<TemperatureBehavior>(0);
            load = provider.GetBehavior<LoadBehavior>(0);
            modeltemp = provider.GetBehavior<ModelTemperatureBehavior>(1);
        }

        /// <summary>
        /// Connect
        /// </summary>
        /// <param name="pins">Pins</param>
        public void Connect(params int[] pins)
        {
            if (pins == null)
                throw new ArgumentNullException(nameof(pins));
            if (pins.Length != 4)
                throw new Diagnostics.CircuitException("Pin count mismatch: 4 pins expected, {0} given".FormatString(pins.Length));
            drainNode = pins[0];
            gateNode = pins[1];
            sourceNode = pins[2];
            bulkNode = pins[3];
        }

        /// <summary>
        /// Get matrix pointers
        /// </summary>
        /// <param name="matrix">Matrix</param>
        public override void GetMatrixPointers(Matrix matrix)
        {
			if (matrix == null)
				throw new ArgumentNullException(nameof(matrix));

            // Get extra equations
            drainNodePrime = load.DrainNodePrime;
            sourceNodePrime = load.SourceNodePrime;

            // Get matrix pointers
            DrainDrainPtr = matrix.GetElement(drainNode, drainNode);
            GateGatePtr = matrix.GetElement(gateNode, gateNode);
            SourceSourcePtr = matrix.GetElement(sourceNode, sourceNode);
            BulkBulkPtr = matrix.GetElement(bulkNode, bulkNode);
            DrainPrimeDrainPrimePtr = matrix.GetElement(drainNodePrime, drainNodePrime);
            SourcePrimeSourcePrimePtr = matrix.GetElement(sourceNodePrime, sourceNodePrime);
            DrainDrainPrimePtr = matrix.GetElement(drainNode, drainNodePrime);
            GateBulkPtr = matrix.GetElement(gateNode, bulkNode);
            GateDrainPrimePtr = matrix.GetElement(gateNode, drainNodePrime);
            GateSourcePrimePtr = matrix.GetElement(gateNode, sourceNodePrime);
            SourceSourcePrimePtr = matrix.GetElement(sourceNode, sourceNodePrime);
            BulkDrainPrimePtr = matrix.GetElement(bulkNode, drainNodePrime);
            BulkSourcePrimePtr = matrix.GetElement(bulkNode, sourceNodePrime);
            DrainPrimeSourcePrimePtr = matrix.GetElement(drainNodePrime, sourceNodePrime);
            DrainPrimeDrainPtr = matrix.GetElement(drainNodePrime, drainNode);
            BulkGatePtr = matrix.GetElement(bulkNode, gateNode);
            DrainPrimeGatePtr = matrix.GetElement(drainNodePrime, gateNode);
            SourcePrimeGatePtr = matrix.GetElement(sourceNodePrime, gateNode);
            SourcePrimeSourcePtr = matrix.GetElement(sourceNodePrime, sourceNode);
            DrainPrimeBulkPtr = matrix.GetElement(drainNodePrime, bulkNode);
            SourcePrimeBulkPtr = matrix.GetElement(sourceNodePrime, bulkNode);
            SourcePrimeDrainPrimePtr = matrix.GetElement(sourceNodePrime, drainNodePrime);
        }

        /// <summary>
        /// Unsetup
        /// </summary>
        public override void Unsetup()
        {
            // Remove references
            DrainDrainPtr = null;
            GateGatePtr = null;
            SourceSourcePtr = null;
            BulkBulkPtr = null;
            DrainPrimeDrainPrimePtr = null;
            SourcePrimeSourcePrimePtr = null;
            DrainDrainPrimePtr = null;
            GateBulkPtr = null;
            GateDrainPrimePtr = null;
            GateSourcePrimePtr = null;
            SourceSourcePrimePtr = null;
            BulkDrainPrimePtr = null;
            BulkSourcePrimePtr = null;
            DrainPrimeSourcePrimePtr = null;
            DrainPrimeDrainPtr = null;
            BulkGatePtr = null;
            DrainPrimeGatePtr = null;
            SourcePrimeGatePtr = null;
            SourcePrimeSourcePtr = null;
            DrainPrimeBulkPtr = null;
            SourcePrimeBulkPtr = null;
            SourcePrimeDrainPrimePtr = null;
        }

        /// <summary>
        /// Create states
        /// </summary>
        /// <param name="states">States</param>
        public override void CreateStates(StatePool states)
        {
			if (states == null)
				throw new ArgumentNullException(nameof(states));

            Qgs = states.CreateDerivative();
            Qgd = states.CreateDerivative();
            Qgb = states.CreateDerivative();
            Qbd = states.CreateDerivative();
            Qbs = states.CreateDerivative();

            Capgs = states.CreateHistory();
            Capgd = states.CreateHistory();
            Capgb = states.CreateHistory();
            Vgs = states.CreateHistory();
            Vds = states.CreateHistory();
            Vbs = states.CreateHistory();
        }

        /// <summary>
        /// Calculate DC state variables
        /// </summary>
        /// <param name="simulation">Time-based simulation</param>
        public override void GetDCstate(TimeSimulation simulation)
        {
			if (simulation == null)
				throw new ArgumentNullException(nameof(simulation));

            double arg, sarg, sargsw;
            double capgs, capgd, capgb;

            // Get voltages
            double vbd = load.Vbd;
            double vbs = load.Vbs;
            double vgs = load.Vgs;
            double vds = load.Vds;
            double vgd = vgs - vds;
            double vgb = vgs - vbs;

            double EffectiveLength = bp.Length - 2 * mbp.LatDiff;
            double GateSourceOverlapCap = mbp.GateSourceOverlapCapFactor * bp.Width;
            double GateDrainOverlapCap = mbp.GateDrainOverlapCapFactor * bp.Width;
            double GateBulkOverlapCap = mbp.GateBulkOverlapCapFactor * EffectiveLength;
            double OxideCap = modeltemp.OxideCapFactor * EffectiveLength * bp.Width;
            
            if (vbs < temp.TDepCap)
            {
                arg = 1 - vbs / temp.TBulkPot;
                if (mbp.BulkJctBotGradingCoeff.Value == mbp.BulkJctSideGradingCoeff.Value)
                {
                    if (mbp.BulkJctBotGradingCoeff.Value == .5)
                        sarg = sargsw = 1 / Math.Sqrt(arg);
                    else
                        sarg = sargsw = Math.Exp(-mbp.BulkJctBotGradingCoeff * Math.Log(arg));
                }
                else
                {
                    if (mbp.BulkJctBotGradingCoeff.Value == .5)
                        sarg = 1 / Math.Sqrt(arg);
                    else
                        sarg = Math.Exp(-mbp.BulkJctBotGradingCoeff * Math.Log(arg));
                    if (mbp.BulkJctSideGradingCoeff.Value == .5)
                        sargsw = 1 / Math.Sqrt(arg);
                    else
                        sargsw = Math.Exp(-mbp.BulkJctSideGradingCoeff * Math.Log(arg));
                }
                Qbs.Current = temp.TBulkPot * (temp.Cbs * (1 - arg * sarg) / (1 - mbp.BulkJctBotGradingCoeff) +
                    temp.Cbssw * (1 - arg * sargsw) / (1 - mbp.BulkJctSideGradingCoeff));
            }
            else
                Qbs.Current = temp.F4s + vbs * (temp.F2s + vbs * (temp.F3s / 2));

            if (vbd < temp.TDepCap)
            {
                arg = 1 - vbd / temp.TBulkPot;
                if (mbp.BulkJctBotGradingCoeff.Value == .5 && mbp.BulkJctSideGradingCoeff.Value == .5)
                    sarg = sargsw = 1 / Math.Sqrt(arg);
                else
                {
                    if (mbp.BulkJctBotGradingCoeff.Value == .5)
                        sarg = 1 / Math.Sqrt(arg);
                    else
                        sarg = Math.Exp(-mbp.BulkJctBotGradingCoeff * Math.Log(arg));
                    if (mbp.BulkJctSideGradingCoeff.Value == .5)
                        sargsw = 1 / Math.Sqrt(arg);
                    else
                        sargsw = Math.Exp(-mbp.BulkJctSideGradingCoeff * Math.Log(arg));
                }
                Qbd.Current = temp.TBulkPot * (temp.Cbd * (1 - arg * sarg) / (1 - mbp.BulkJctBotGradingCoeff) +
                    temp.Cbdsw * (1 - arg * sargsw) / (1 - mbp.BulkJctSideGradingCoeff));
            }
            else
                Qbd.Current = temp.F4d + vbd * (temp.F2d + vbd * temp.F3d / 2);

            /* 
             * calculate meyer's capacitors
             */
            double icapgs, icapgd, icapgb;
            if (load.Mode > 0)
            {
                Transistor.DEVqmeyer(vgs, vgd, mbp.MosfetType * load.Von, mbp.MosfetType * load.Vdsat,
                    out icapgs, out icapgd, out icapgb,
                    temp.TPhi, OxideCap);
            }
            else
            {
                Transistor.DEVqmeyer(vgd, vgs, mbp.MosfetType * load.Von, mbp.MosfetType * load.Vdsat,
                    out icapgd, out icapgs, out icapgb,
                    temp.TPhi, OxideCap);
            }
            Capgs.Current = icapgs;
            Capgd.Current = icapgd;
            Capgb.Current = icapgb;
            capgs = 2 * Capgs.Current + GateSourceOverlapCap;
            capgd = 2 * Capgd.Current + GateDrainOverlapCap;
            capgb = 2 * Capgb.Current + GateBulkOverlapCap;

            /* TRANOP only */
            Qgs.Current = vgs * capgs;
            Qgd.Current = vgd * capgd;
            Qgb.Current = vgb * capgb;

            // Store these voltages
            Vgs.Current = vgs;
            Vds.Current = vds;
            Vbs.Current = vbs;
        }

        /// <summary>
        /// Transient behavior
        /// </summary>
        /// <param name="simulation">Time-based simulation</param>
        public override void Transient(TimeSimulation simulation)
        {
			if (simulation == null)
				throw new ArgumentNullException(nameof(simulation));

            var state = simulation.State;
            double arg, sarg, sargsw;
            double vgs1, vgd1, vgb1, capgs, capgd, capgb;

            // Get voltages
            double vbd = load.Vbd;
            double vbs = load.Vbs;
            double vgs = load.Vgs;
            double vds = load.Vds;
            double vgd = vgs - vds;
            double vgb = vgs - vbs;

            double EffectiveLength = bp.Length - 2 * mbp.LatDiff;
            double GateSourceOverlapCap = mbp.GateSourceOverlapCapFactor * bp.Width;
            double GateDrainOverlapCap = mbp.GateDrainOverlapCapFactor * bp.Width;
            double GateBulkOverlapCap = mbp.GateBulkOverlapCapFactor * EffectiveLength;
            double OxideCap = modeltemp.OxideCapFactor * EffectiveLength * bp.Width;

            double Gbd = 0.0;
            double Cbd = 0.0;
            double Cd = 0.0;
            double Gbs = 0.0;
            double Cbs = 0.0;

            // Store these voltages
            Vgs.Current = vgs;
            Vds.Current = vds;
            Vbs.Current = vbs;

            /* 
             * now we do the hard part of the bulk - drain and bulk - source
             * diode - we evaluate the non - linear capacitance and
             * charge
             * 
             * the basic equations are not hard, but the implementation
             * is somewhat long in an attempt to avoid log / exponential
             * evaluations
             */
            /* 
             * charge storage elements
             * 
             * .. bulk - drain and bulk - source depletion capacitances
             */
            /* CAPBYPASS */

            /* can't bypass the diode capacitance calculations */
            /* CAPZEROBYPASS */
            if (vbs < temp.TDepCap)
            {
                arg = 1 - vbs / temp.TBulkPot;
                /* 
                 * the following block looks somewhat long and messy, 
                 * but since most users use the default grading
                 * coefficients of .5, and sqrt is MUCH faster than an
                 * Math.Exp(Math.Log()) we use this special case code to buy time.
                 * (as much as 10% of total job time!)
                 */
                if (mbp.BulkJctBotGradingCoeff.Value == mbp.BulkJctSideGradingCoeff.Value)
                {
                    if (mbp.BulkJctBotGradingCoeff.Value == .5)
                    {
                        sarg = sargsw = 1 / Math.Sqrt(arg);
                    }
                    else
                    {
                        sarg = sargsw = Math.Exp(-mbp.BulkJctBotGradingCoeff * Math.Log(arg));
                    }
                }
                else
                {
                    if (mbp.BulkJctBotGradingCoeff.Value == .5)
                    {
                        sarg = 1 / Math.Sqrt(arg);
                    }
                    else
                    {
                        /* NOSQRT */
                        sarg = Math.Exp(-mbp.BulkJctBotGradingCoeff * Math.Log(arg));
                    }
                    if (mbp.BulkJctSideGradingCoeff.Value == .5)
                    {
                        sargsw = 1 / Math.Sqrt(arg);
                    }
                    else
                    {
                        /* NOSQRT */
                        sargsw = Math.Exp(-mbp.BulkJctSideGradingCoeff * Math.Log(arg));
                    }
                }

                /* NOSQRT */
                Qbs.Current = temp.TBulkPot * (temp.Cbs * (1 - arg * sarg) / (1 - mbp.BulkJctBotGradingCoeff) +
                    temp.Cbssw * (1 - arg * sargsw) / (1 - mbp.BulkJctSideGradingCoeff));
                Capbs = temp.Cbs * sarg + temp.Cbssw * sargsw;
            }
            else
            {
                Qbs.Current = temp.F4s + vbs * (temp.F2s + vbs * (temp.F3s / 2));
                Capbs = temp.F2s + temp.F3s * vbs;
            }

            /* can't bypass the diode capacitance calculations */

            /* CAPZEROBYPASS */
            if (vbd < temp.TDepCap)
            {
                arg = 1 - vbd / temp.TBulkPot;
                /* 
                * the following block looks somewhat long and messy, 
                * but since most users use the default grading
                * coefficients of .5, and sqrt is MUCH faster than an
                * Math.Exp(Math.Log()) we use this special case code to buy time.
                * (as much as 10% of total job time!)
                */
                if (mbp.BulkJctBotGradingCoeff.Value == .5 && mbp.BulkJctSideGradingCoeff.Value == .5)
                {
                    sarg = sargsw = 1 / Math.Sqrt(arg);
                }
                else
                {
                    if (mbp.BulkJctBotGradingCoeff.Value == .5)
                    {
                        sarg = 1 / Math.Sqrt(arg);
                    }
                    else
                    {
                        /* NOSQRT */
                        sarg = Math.Exp(-mbp.BulkJctBotGradingCoeff * Math.Log(arg));
                    }
                    if (mbp.BulkJctSideGradingCoeff.Value == .5)
                    {
                        sargsw = 1 / Math.Sqrt(arg);
                    }
                    else
                    {
                        /* NOSQRT */
                        sargsw = Math.Exp(-mbp.BulkJctSideGradingCoeff * Math.Log(arg));
                    }
                }
                /* NOSQRT */
                Qbd.Current = temp.TBulkPot * (temp.Cbd * (1 - arg * sarg) / (1 - mbp.BulkJctBotGradingCoeff) +
                    temp.Cbdsw * (1 - arg * sargsw) / (1 - mbp.BulkJctSideGradingCoeff));
                Capbd = temp.Cbd * sarg + temp.Cbdsw * sargsw;
            }
            else
            {
                Qbd.Current = temp.F4d + vbd * (temp.F2d + vbd * temp.F3d / 2);
                Capbd = temp.F2d + vbd * temp.F3d;
            }

            // integrate the capacitors and save results
            Qbd.Integrate();
            Gbd += Qbd.Jacobian(Capbd);
            Cbd += Qbd.Derivative;
            Cd -= Qbd.Derivative;
            // NOTE: The derivative of Qbd should be added to Cd (drain current). Figure out a way later.
            Qbs.Integrate();
            Gbs += Qbs.Jacobian(Capbs);
            Cbs += Qbs.Derivative;

            /* 
             * calculate meyer's capacitors
             */
            /* 
             * new cmeyer - this just evaluates at the current time, 
             * expects you to remember values from previous time
             * returns 1 / 2 of non-constant portion of capacitance
             * you must add in the other half from previous time
             * and the constant part
             */
            double icapgs, icapgd, icapgb;
            if (load.Mode > 0)
            {
                Transistor.DEVqmeyer(vgs, vgd,  mbp.MosfetType * load.Von, mbp.MosfetType * load.Vdsat,
                    out icapgs, out icapgd, out icapgb,
                    temp.TPhi, OxideCap);
            }
            else
            {
                Transistor.DEVqmeyer(vgd, vgs, mbp.MosfetType * load.Von, mbp.MosfetType * load.Vdsat,
                    out icapgd, out icapgs, out icapgb,
                    temp.TPhi, OxideCap);
            }
            Capgs.Current = icapgs;
            Capgd.Current = icapgd;
            Capgb.Current = icapgb;
            vgs1 = Vgs[1];
            vgd1 = vgs1 - Vds[1];
            vgb1 = vgs1 - Vbs[1];
            capgs = Capgs.Current + Capgs[1] + GateSourceOverlapCap;
            capgd = Capgd.Current + Capgd[1] + GateDrainOverlapCap;
            capgb = Capgb.Current + Capgb[1] + GateBulkOverlapCap;

            Qgs.Current = (vgs - vgs1) * capgs + Qgs[1];
            Qgd.Current = (vgd - vgd1) * capgd + Qgd[1];
            Qgb.Current = (vgb - vgb1) * capgb + Qgb[1];


            /* NOTE: We can't reset derivatives!
            if (capgs == 0)
                state.States[0][States + Cqgs] = 0;
            if (capgd == 0)
                state.States[0][States + Cqgd] = 0;
            if (capgb == 0)
                state.States[0][States + Cqgb] = 0;
            */

            /* NOTE: The formula with the method.Slope is to make it work for nonlinear capacitances!
             * The correct formula is: ceq = dQ/dt - geq * vq where geq = slope * dQ/dvq
             * The formula in Spice 3f5 is: ceq = dQ/dt - slope * Q where it assumes a linear capacitance
            method.Integrate(state, out gcgs, out ceqgs, States + Qgs, capgs);
            method.Integrate(state, out gcgd, out ceqgd, States + Qgd, capgd);
            method.Integrate(state, out gcgb, out ceqgb, States + Qgb, capgb);
            ceqgs = ceqgs - gcgs * vgs + method.Slope * state.States[0][States + Qgs];
            ceqgd = ceqgd - gcgd * vgd + method.Slope * state.States[0][States + Qgd];
            ceqgb = ceqgb - gcgb * vgb + method.Slope * state.States[0][States + Qgb];
            */

            Qgs.Integrate();
            double gcgs = Qgs.Jacobian(capgs);
            double ceqgs = Qgs.RhsCurrent(gcgs, vgs);
            Qgd.Integrate();
            double gcgd = Qgd.Jacobian(capgd);
            double ceqgd = Qgd.RhsCurrent(gcgd, vgd);
            Qgb.Integrate();
            double gcgb = Qgb.Jacobian(capgb);
            double ceqgb = Qgb.RhsCurrent(gcgb, vgb);

            /* 
			 * load current vector
			 */
            double ceqbs = mbp.MosfetType * (Cbs - Gbs * vbs);
            double ceqbd = mbp.MosfetType * (Cbd - Gbd * vbd);
            state.Rhs[gateNode] -= mbp.MosfetType * (ceqgs + ceqgb + ceqgd);
            state.Rhs[bulkNode] -= ceqbs + ceqbd - mbp.MosfetType * ceqgb;
            state.Rhs[drainNodePrime] += ceqbd + mbp.MosfetType * ceqgd;
            state.Rhs[sourceNodePrime] += ceqbs + mbp.MosfetType * ceqgs;

            /* 
			 * load y matrix
			 */
            GateGatePtr.Add(gcgd + gcgs + gcgb);
            BulkBulkPtr.Add((Gbd + Gbs + gcgb));
            DrainPrimeDrainPrimePtr.Add(Gbd + gcgd);
            SourcePrimeSourcePrimePtr.Add(Gbs + gcgs);
            GateBulkPtr.Sub(gcgb);
            GateDrainPrimePtr.Sub(gcgd);
            GateSourcePrimePtr.Sub(gcgs);
            BulkGatePtr.Sub(gcgb);
            BulkDrainPrimePtr.Sub(Gbd);
            BulkSourcePrimePtr.Sub(Gbs);
            DrainPrimeGatePtr.Sub(gcgd);
            DrainPrimeBulkPtr.Sub(Gbd);
            SourcePrimeGatePtr.Sub(gcgs);
            SourcePrimeBulkPtr.Sub(Gbs);
        }

        /// <summary>
        /// Truncate timeStep
        /// </summary>
        /// <param name="timeStep">TimeStep</param>
        public override void Truncate(ref double timeStep)
        {
            Qgs.LocalTruncationError(ref timeStep);
            Qgd.LocalTruncationError(ref timeStep);
            Qgb.LocalTruncationError(ref timeStep);
        }
    }
}
