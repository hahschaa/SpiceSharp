using System;
using System.Collections.Generic;
using System.Numerics;

namespace SpiceSharp.Simulations
{
    /// <summary>
    /// Class that implements a frequency-domain analysis (AC analysis).
    /// </summary>
    /// <seealso cref="FrequencySimulation" />
    public class AC : FrequencySimulation
    {
        private bool _isInIteratorMode = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AC"/> class.
        /// </summary>
        /// <param name="name">The name of the simulation.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>.</exception>
        public AC(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AC"/> class.
        /// </summary>
        /// <param name="name">The name of the simulation.</param>
        /// <param name="frequencySweep">The frequency points.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>.</exception>
        public AC(string name, IEnumerable<double> frequencySweep)
            : base(name, frequencySweep)
        {
        }

        /// <summary>
        /// Creates an <see cref="IIterator"/>.
        /// </summary>
        /// <param name="circuit">The circuit on which to run the simulation.</param>
        /// <returns>The created instance.</returns>
        public IIterator CreateIterator(Circuit circuit)
        {
            this._isInIteratorMode = true;

            this.InitializeRun(circuit);

            var state = InitExecute();
            return new Iterator(
                this,
                state,
                OnIteratorDisposed);
        }

        /// <inheritdoc/>
        protected override void Execute()
        {
            if (_isInIteratorMode)
            {
                throw new InvalidOperationException(
                    $"Cannot execute simulation when using an {nameof(IIterator)}");
            }

            try
            {
                var cstate = InitExecute();

                // Export operating point if requested
                var exportargs = new ExportDataEventArgs(this);
                if (FrequencyParameters.KeepOpInfo)
                    OnExport(exportargs);

                // Sweep the frequency
                foreach (double freq in FrequencyParameters.Frequencies)
                {
                    // Calculate the current frequency
                    cstate.Laplace = new Complex(0.0, 2.0 * Math.PI * freq);

                    // Solve
                    AcIterate();

                    // Export the timepoint
                    OnExport(exportargs);
                }
            }
            finally
            {
                Statistics.ComplexTime.Stop();
            }
        }

        private ComplexSimulationState InitExecute()
        {
            // Execute base behavior
            base.Execute();

            var cstate = (ComplexSimulationState)GetState<IComplexSimulationState>();

            // Calculate the operating point
            cstate.Laplace = 0.0;
            Op(BiasingParameters.DcMaxIterations);

            // Load all in order to calculate the AC info for all devices
            Statistics.ComplexTime.Start();

            InitializeAcParameters();

            return cstate;
        }

        private void OnIteratorDisposed()
        {
            _isInIteratorMode = false;
            DeinitializeRun();
        }

        /// <summary>
        /// Can be used to retrieve results for a single frequency.
        /// </summary>
        public interface IIterator : IDisposable
        {
            /// <summary>
            /// Execute a single frequency.
            /// </summary>
            /// <param name="freq"></param>
            /// <returns></returns>
            public ExportDataEventArgs ExecuteSingle(double freq);
        }

        private class Iterator : IIterator
        {
            private readonly AC _ac;
            private readonly FrequencyParameters _originalFrequencyParameters;
            private readonly ComplexSimulationState _state;
            private readonly Action _disposeCallback;
            private readonly ExportDataEventArgs _exportArgs;

            internal Iterator(AC ac, ComplexSimulationState state, Action disposeCallback)
            {
                _ac = ac;
                _state = state;
                _disposeCallback = disposeCallback;
                _exportArgs = new(ac);

                _originalFrequencyParameters = _ac.FrequencyParameters.Clone();
                _ac.FrequencyParameters.KeepOpInfo = false;
                _ac.FrequencyParameters.Frequencies = Array.Empty<double>();
            }

            public ExportDataEventArgs ExecuteSingle(double freq)
            {
                _ac.FrequencyParameters.Frequencies = new[] { freq };

                // Calculate the current frequency
                _state.Laplace = new Complex(0.0, 2.0 * Math.PI * freq);

                // Solve
                _ac.AcIterate();

                return _exportArgs;
            }

            public void Dispose()
            {
                _ac.FrequencyParameters.KeepOpInfo = _originalFrequencyParameters.KeepOpInfo;
                _ac.FrequencyParameters.Frequencies = _originalFrequencyParameters.Frequencies;
                _disposeCallback();
            }
        }
    }
}
