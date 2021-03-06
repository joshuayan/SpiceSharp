﻿using SpiceSharp.Attributes;
using SpiceSharp.Components.Diodes;
using SpiceSharp.ParameterSets;
using System;

namespace SpiceSharp.Components
{
    /// <summary>
    /// A diode.
    /// </summary>
    /// <seealso cref="Component"/>
    /// <seealso cref="IParameterized{P}"/>
    /// <seealso cref="Diodes.Parameters"/>
    [Pin(0, "D+"), Pin(1, "D-")]
    public class Diode : Component<ComponentBindingContext>,
        IParameterized<Parameters>
    {
        /// <inheritdoc/>
        public Parameters Parameters { get; } = new Parameters();

        /// <summary>
        /// The pin count for diodes.
        /// </summary>
        [ParameterName("pincount"), ParameterInfo("Number of pins")]
        public const int PinCount = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="Diode"/> class.
        /// </summary>
        /// <param name="name">The name of the device.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>.</exception>
        public Diode(string name)
            : base(name, PinCount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Diode"/> class.
        /// </summary>
        /// <param name="name">The name of the device.</param>
        /// <param name="anode">The anode.</param>
        /// <param name="cathode">The cathode.</param>
        /// <param name="model">The model.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>.</exception>
        public Diode(string name, string anode, string cathode, string model)
            : this(name)
        {
            Connect(anode, cathode);
            Model = model;
        }
    }
}
