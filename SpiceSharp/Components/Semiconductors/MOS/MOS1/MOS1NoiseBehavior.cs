﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiceSharp.Behaviors;
using SpiceSharp.Circuits;

namespace SpiceSharp.Components.ComponentBehaviors
{
    /// <summary>
    /// Noise behaviour for a <see cref="MOS1"/>
    /// </summary>
    public class MOS1NoiseBehavior : CircuitObjectBehaviorNoise
    {
        /// <summary>
        /// Necessary behaviors
        /// </summary>
        private MOS1TemperatureBehavior temp;
        private MOS1LoadBehavior load;
        private MOS1ModelTemperatureBehavior modeltemp;
        private MOS1ModelNoiseBehavior modelnoise;

        private const int MOS1RDNOIZ = 0;
        private const int MOS1RSNOIZ = 1;
        private const int MOS1IDNOIZ = 2;
        private const int MOS1FLNOIZ = 3;

        /// <summary>
        /// Noise generators
        /// </summary>
        public ComponentNoise MOS1noise { get; } = new ComponentNoise(
            new Noise.NoiseThermal("rd", 0, 4),
            new Noise.NoiseThermal("rs", 2, 5),
            new Noise.NoiseThermal("id", 4, 5),
            new Noise.NoiseGain("1overf", 4, 5)
            );

        /// <summary>
        /// Setup the behaviour
        /// </summary>
        /// <param name="component">Component</param>
        /// <param name="ckt">Circuit</param>
        public override void Setup(CircuitObject component, Circuit ckt)
        {
            var mos1 = component as MOS1;

            // Get behaviors
            temp = GetBehavior<MOS1TemperatureBehavior>(component);
            load = GetBehavior<MOS1LoadBehavior>(component);
            modeltemp = GetBehavior<MOS1ModelTemperatureBehavior>(mos1.Model);
            modelnoise = GetBehavior<MOS1ModelNoiseBehavior>(mos1.Model);

            MOS1noise.Setup(ckt,
                mos1.MOS1dNode,
                mos1.MOS1gNode,
                mos1.MOS1sNode,
                mos1.MOS1bNode,
                load.MOS1dNodePrime,
                load.MOS1sNodePrime);
        }

        /// <summary>
        /// Execute behaviour
        /// </summary>
        /// <param name="ckt">Circuit</param>
        public override void Noise(Circuit ckt)
        {
            var state = ckt.State;
            var noise = state.Noise;

            double coxSquared;
            if (modeltemp.MOS1oxideCapFactor == 0.0)
                coxSquared = 3.9 * 8.854214871e-12 / 1e-7;
            else
                coxSquared = modeltemp.MOS1oxideCapFactor;
            coxSquared *= coxSquared;

            // Set noise parameters
            MOS1noise.Generators[MOS1RDNOIZ].Set(temp.MOS1drainConductance);
            MOS1noise.Generators[MOS1RSNOIZ].Set(temp.MOS1sourceConductance);
            MOS1noise.Generators[MOS1IDNOIZ].Set(2.0 / 3.0 * Math.Abs(load.MOS1gm));
            MOS1noise.Generators[MOS1FLNOIZ].Set(modelnoise.MOS1fNcoef * Math.Exp(modelnoise.MOS1fNexp * Math.Log(Math.Max(Math.Abs(load.MOS1cd), 1e-38))) 
                / (temp.MOS1w * (temp.MOS1l - 2 * modeltemp.MOS1latDiff) * coxSquared) / noise.Freq);

            // Evaluate noise sources
            MOS1noise.Evaluate(ckt);
        }
    }
}
