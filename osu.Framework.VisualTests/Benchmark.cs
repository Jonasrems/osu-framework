﻿// Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using System.Diagnostics;
using osu.Framework.GameModes.Testing;

namespace osu.Framework.VisualTests
{
    public class Benchmark : BaseGame
    {
        private double timePerTest = 200;

        protected override void Load(BaseGame game)
        {
            base.Load(game);

            Host.MaximumDrawHz = int.MaxValue;
            Host.MaximumUpdateHz = int.MaxValue;
            Host.MaximumInactiveHz = int.MaxValue;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            TestBrowser f = new TestBrowser();
            Add(f);

            Console.WriteLine($@"{Time}: Running {f.TestCount} tests for {timePerTest}ms each...");

            for (int i = 1; i < f.TestCount; i++)
            {
                int loadableCase = i;
                Scheduler.AddDelayed(delegate
                {
                    f.LoadTest(loadableCase);
                    Console.WriteLine($@"{Time}: Switching to test #{loadableCase}");
                }, loadableCase * timePerTest);
            }

            Scheduler.AddDelayed(Host.Exit, f.TestCount * timePerTest);
        }
    }
}
