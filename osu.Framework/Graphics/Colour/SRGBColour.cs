﻿// Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using OpenTK.Graphics;
using osu.Framework.Extensions.Color4Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Framework.Graphics.Colour
{
    /// <summary>
    /// A wrapper struct around Color4 that takes care of converting between sRGB and linear colour spaces.
    /// Internally this struct stores the colour in linear space, which is exposed by the Linear member.
    /// This struct implicitly converts to sRGB space Color4 values (i.e. it can be assigned and implicitly cast)
    /// to sRGB Color4.
    /// </summary>
    public struct SRGBColour : IEquatable<SRGBColour>
    {
        public Color4 Linear;

        public static implicit operator SRGBColour(Color4 value) => new SRGBColour { Linear = value.ToLinear() };
        public static implicit operator Color4(SRGBColour value) => value.Linear.ToSRGB();

        /// <summary>
        /// Multiplies 2 colours in linear colour space.
        /// </summary>
        /// <param name="first">First factor.</param>
        /// <param name="second">Second factor.</param>
        /// <returns>Product of first and second.</returns>
        public static SRGBColour operator *(SRGBColour first, SRGBColour second)
        {
            return new SRGBColour { Linear = Color4Extensions.Multiply(first.Linear, second.Linear) };
        }

        /// <summary>
        /// Multiplies the alpha value of this colour by the given alpha factor.
        /// </summary>
        /// <param name="alpha">The alpha factor to multiply with.</param>
        public void MultiplyAlpha(float alpha) => Linear.A *= alpha;

        public bool Equals(SRGBColour other) => Linear.Equals(other.Linear);
    }
}
