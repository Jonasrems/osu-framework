﻿// Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using System.Diagnostics;
using osu.Framework.Graphics.Drawables;
using osu.Framework.Graphics.Transformations;
using osu.Framework.Input;
using osu.Framework.MathUtils;
using OpenTK;
using OpenTK.Graphics;

namespace osu.Framework.Graphics.Containers
{
    public class ScrollContainer : Container
    {
        /// <summary>
        /// Determines whether the scroll dragger appears on the left side. If not, then it always appears on the right side.
        /// </summary>
        public bool ScrollDraggerOnLeft
        {
            get { return scrollbar.Anchor == Anchor.TopLeft; }

            set
            {
                scrollbar.Anchor = value ? Anchor.TopLeft : Anchor.TopRight;
                scrollbar.Origin = value ? Anchor.TopLeft : Anchor.TopRight;
            }
        }

        private AutoSizeContainer content;
        private ScrollBar scrollbar;

        /// <summary>
        /// Vertical size of available content (content.Size)
        /// </summary>
        private float availableContent = -1;

        private float displayableContent => Size.Y;

        private float current;

        private float currentClamped => MathHelper.Clamp(current, 0, availableContent - displayableContent);

        protected override Container Content => content;

        public ScrollContainer()
        {
            RelativeSizeAxes = Axes.Both;

            AddInternal(new Drawable[]
            {
                content = new AutoSizeContainer { RelativeSizeAxes = Axes.X },
                scrollbar = new ScrollBar(offset),
            });
        }

        public override void Load(BaseGame game)
        {
            base.Load(game);

            Masking = true;

            content.OnAutoSize += contentAutoSize;
        }

        public override bool Invalidate(Invalidation invalidation = Invalidation.All, Drawable source = null, bool shallPropagate = true)
        {
            if (!base.Invalidate(invalidation, source, shallPropagate))
                return false;

            return true;
        }

        private void contentAutoSize()
        {
            if (Precision.AlmostEquals(availableContent, content.Size.Y))
                return;

            availableContent = content.Size.Y;
            updateSize();
            offset(0);

            scrollbar.Alpha = availableContent > displayableContent ? 1 : 0;
        }

        protected override bool OnDragStart(InputState state) => true;

        protected override bool OnMouseDown(InputState state, MouseDownEventArgs args)
        {
            offset(0, false, false);
            return base.OnMouseDown(state, args);
        }

        protected override bool OnDrag(InputState state)
        {
            offset(-state.Mouse.Delta.Y, false, false);
            return base.OnDrag(state);
        }

        protected override bool OnDragEnd(InputState state)
        {
            //forces a clamped state to return to correct location.
            offset(-state.Mouse.Delta.Y * 10);

            return base.OnDragEnd(state);
        }

        protected override bool OnWheelDown(InputState state)
        {
            offset(80);
            return base.OnWheelDown(state);
        }

        protected override bool OnWheelUp(InputState state)
        {
            offset(-80);
            return base.OnWheelUp(state);
        }

        private void offset(float value, bool clamp = true, bool animated = true)
        {
            scrollTo(current + value, clamp, animated);
        }

        private void scrollTo(float value, bool clamp = true, bool animated = true)
        {
            current = value;

            if (clamp && current != currentClamped)
            {
                updateScroll(false);
                current = currentClamped;
            }

            updateScroll(animated);
        }

        private void updateSize()
        {
            scrollbar?.ScaleTo(new Vector2(1, Math.Min(1, displayableContent / availableContent)), 200, EasingTypes.OutExpo);
        }

        private void updateScroll(bool animated = true)
        {
            float adjusted = (current + currentClamped) / 2;

            scrollbar?.MoveToY(adjusted * scrollbar.Scale.Y, animated ? 800 : 0, EasingTypes.OutExpo);
            content.MoveToY(-adjusted, animated ? 800 : 0, EasingTypes.OutExpo);
        }

        private class ScrollBar : Container
        {
            private readonly Action<float, bool, bool> offsetDelegate;

            private Color4 hoverColour = Color4.White;
            private Color4 defaultColour = Color4.LightGray;
            private Color4 highlightColour = Color4.GreenYellow;
            private Box box;

            public ScrollBar(Action<float, bool, bool> offsetDelegate)
            {
                this.offsetDelegate = offsetDelegate;
            }

            public override void Load(BaseGame game)
            {
                base.Load(game);

                Add(box = new Box
                {
                    RelativeSizeAxes = Axes.Both
                });

                Anchor = Anchor.TopRight;
                Origin = Anchor.TopRight;
                RelativeSizeAxes = Axes.Y;
                Size = new Vector2(10, 1);
                Colour = defaultColour;
            }

            protected override bool OnHover(InputState state)
            {
                FadeColour(hoverColour, 100);
                return true;
            }

            protected override void OnHoverLost(InputState state)
            {
                FadeColour(defaultColour, 100);
            }

            protected override bool OnDragStart(InputState state) => true;

            protected override bool OnMouseDown(InputState state, MouseDownEventArgs args)
            {
                //note that we are changing the colour of the box here as to not interfere with the hover effect.
                box.FadeColour(highlightColour, 100);
                return true;
            }

            protected override bool OnMouseUp(InputState state, MouseUpEventArgs args)
            {
                box.FadeColour(Color4.White, 100);

                return base.OnMouseUp(state, args);
            }

            protected override bool OnDrag(InputState state)
            {
                offsetDelegate(state.Mouse.Delta.Y / (Scale.Y * Scale.Y), true, false);
                return true;
            }
        }
    }
}
