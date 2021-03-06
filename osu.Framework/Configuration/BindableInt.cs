﻿// Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System.Globalization;
using OpenTK;

namespace osu.Framework.Configuration
{
    public class BindableInt : Bindable<int>
    {
        public override int Value
        {
            get { return base.Value; }
            set { base.Value = MathHelper.Clamp(value, MinValue, MaxValue); }
        }

        internal int MinValue = int.MinValue;
        internal int MaxValue = int.MaxValue;

        public BindableInt(int value = 0)
            : base(value)
        {
        }

        public static implicit operator int(BindableInt value) => value?.Value ?? 0;

        public override bool Parse(object s)
        {
            string str = s as string;
            if (str == null) return false;

            Value = int.Parse(str, NumberFormatInfo.InvariantInfo);
            return true;
        }

        public override string ToString() => Value.ToString(NumberFormatInfo.InvariantInfo);
    }
}
