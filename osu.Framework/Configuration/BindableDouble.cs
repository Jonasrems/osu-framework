﻿// Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using System.Globalization;
using OpenTK;

namespace osu.Framework.Configuration
{
    public class BindableDouble : Bindable<double>
    {
        public override bool IsDefault => Math.Abs(Value - Default) < Precision;

        public double Precision = double.Epsilon;

        public override double Value
        {
            get { return base.Value; }
            set
            {
                double boundValue = MathHelper.Clamp(value, MinValue, MaxValue);

                if (Precision > double.Epsilon)
                    boundValue = Math.Round(boundValue / Precision) * Precision;

                base.Value = boundValue;
            }
        }

        internal double MinValue = double.MinValue;
        internal double MaxValue = double.MaxValue;

        public BindableDouble(double value = 0)
            : base(value)
        {
        }

        public static implicit operator double(BindableDouble value) => value?.Value ?? 0;

        public override string ToString() => Value.ToString("0.0###", NumberFormatInfo.InvariantInfo);

        public override bool Parse(object s)
        {
            string str = s as string;
            if (str == null) return false;

            Value = double.Parse(str, NumberFormatInfo.InvariantInfo);
            return true;
        }
    }
}
