﻿using System;

namespace SpiceSharp.Simulations
{
    /// <summary>
    /// A class that represents noise
    /// </summary>
    public class StateNoise
    {
        /// <summary>
        /// Private variables
        /// </summary>
        double gainSquareInverted, frequency, lastFrequency, logLastFrequency, deltaFrequency, deltaLogFrequency, logFrequency;

        /// <summary>
        /// Current frequency point
        /// </summary>
        public double Frequency
        {
            get => frequency;
            set
            {
                // Shift current frequency to last frequency
                lastFrequency = frequency;
                logLastFrequency = logFrequency;

                // Update new values
                frequency = value;
                logFrequency = Math.Log(Math.Max(frequency, 1e-38));

                // Delta
                deltaFrequency = frequency - lastFrequency;
                deltaLogFrequency = logFrequency - logLastFrequency;
            }
        }

        /// <summary>
        /// Get the frequency step
        /// </summary>
        public double DeltaFrequency { get => deltaFrequency; }

        /// <summary>
        /// Output referred noise
        /// </summary>
        public double OutputNoise { get; set; } = 0.0;

        /// <summary>
        /// Input referred noise
        /// </summary>
        public double InputNoise { get; set; } = 0.0;

        /// <summary>
        /// Output noise density
        /// </summary>
        public double OutputNoiseDensity { get; set; } = 0.0;

        /// <summary>
        /// Gets or sets the inverse squared gain
        /// </summary>
        public double GainInverseSquared
        {
            get => gainSquareInverted;
            set
            {
                gainSquareInverted = value;
                LogInverseGain = Math.Log(value);
            }
        }

        /// <summary>
        /// Gets the logarithm of the gain squared
        /// </summary>
        public double LogInverseGain { get; private set; } = 0.0;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="frequency">Starting frequency</param>
        public void Initialize(double frequency)
        {
            this.frequency = frequency;
            lastFrequency = frequency;

            OutputNoise = 0.0;
            InputNoise = 0.0;
        }
        
        /// <summary>
        /// This subroutine evaluate the integratl of the function
        /// NOISE = a * (FREQUENCY) ^ (EXPONENT)
        /// given two points from the curve. If EXPONENT is relatively close to 0, the noise is simply multiplied
        /// by the change in frequency.
        /// If it isn't, a more complicated expression must be used.
        /// Note that EXPONENT = -1 gives a different equation than EXPONENT != -1.
        /// </summary>
        /// <param name="noiseDensity">Noise density</param>
        /// <param name="logNoiseDensity">Last noise density</param>
        /// <param name="lastLogNoiseDensity">Last log noise density</param>
        /// <returns></returns>
        public double Integrate(double noiseDensity, double logNoiseDensity, double lastLogNoiseDensity)
        {
            double exponent = (logNoiseDensity - lastLogNoiseDensity) / deltaLogFrequency;
            if (Math.Abs(exponent) < 1e-10)
                return noiseDensity * deltaFrequency;
            else
            {
                double a = Math.Exp(logNoiseDensity - exponent * logFrequency);
                exponent += 1.0;
                if (Math.Abs(exponent) < 1e-10)
                    return a * (logFrequency - logLastFrequency);
                else
                    return a * (Math.Exp(exponent * logFrequency) - Math.Exp(exponent * logLastFrequency)) / exponent;
            }
        }
    }
}
