﻿// Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using osu.Framework.Extensions.MatrixExtensions;
using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics.Colour;

namespace osu.Framework.Graphics
{
    public struct DrawInfo : IEquatable<DrawInfo>
    {
        public Matrix3 Matrix;
        public Matrix3 MatrixInverse;
        public ColourInfo Colour;
        public BlendingInfo Blending;

        public DrawInfo(Matrix3? matrix = null, Matrix3? matrixInverse = null, ColourInfo? colour = null, BlendingInfo? blending = null)
        {
            Matrix = matrix ?? Matrix3.Identity;
            MatrixInverse = matrixInverse ?? Matrix3.Identity;
            Colour = colour ?? new ColourInfo(Color4.White);
            Blending = blending ?? new BlendingInfo();
        }

        /// <summary>
        /// Applies a transformation to the current DrawInfo.
        /// </summary>
        /// <param name="target">The DrawInfo instance to be filled with the result.</param>
        /// <param name="translation">The amount by which to translate the current position.</param>
        /// <param name="scale">The amount by which to scale.</param>
        /// <param name="rotation">The amount by which to rotate.</param>
        /// <param name="origin">The center of rotation and scale.</param>
        /// <param name="shear">The shear amounts for both directions.</param>
        /// <param name="colour">An optional color to be applied multiplicatively.</param>
        /// <param name="blending">An optional blending change.</param>
        public void ApplyTransform(ref DrawInfo target, Vector2 translation, Vector2 scale, float rotation, Vector2 shear, Vector2 origin, ColourInfo? colour = null, BlendingInfo? blending = null)
        {
            target.Matrix = Matrix;
            target.MatrixInverse = MatrixInverse;

            if (translation != Vector2.Zero)
            {
                MatrixExtensions.TranslateFromLeft(ref target.Matrix, translation);
                MatrixExtensions.TranslateFromRight(ref target.MatrixInverse, -translation);
            }

            if (rotation != 0)
            {
                float radians = MathHelper.DegreesToRadians(rotation);
                MatrixExtensions.RotateFromLeft(ref target.Matrix, radians);
                MatrixExtensions.RotateFromRight(ref target.MatrixInverse, -radians);
            }

            if (shear != Vector2.Zero)
            {
                MatrixExtensions.ShearFromLeft(ref target.Matrix, -shear);
                MatrixExtensions.ShearFromRight(ref target.MatrixInverse, shear);
            }

            if (scale != Vector2.One)
            {
                Vector2 inverseScale = new Vector2(1.0f / scale.X, 1.0f / scale.Y);
                MatrixExtensions.ScaleFromLeft(ref target.Matrix, scale);
                MatrixExtensions.ScaleFromRight(ref target.MatrixInverse, inverseScale);
            }

            if (origin != Vector2.Zero)
            {
                MatrixExtensions.TranslateFromLeft(ref target.Matrix, -origin);
                MatrixExtensions.TranslateFromRight(ref target.MatrixInverse, origin);
            }

            //========================================================================================
            //== Uncomment the following 2 lines to use a ground-truth matrix inverse for debugging ==
            //========================================================================================
            //target.MatrixInverse = target.Matrix;
            //MatrixExtensions.FastInvert(ref target.MatrixInverse);

            if (colour != null)
                target.Colour = new ColourInfo(ref this, colour.Value);
            else
                target.Colour = Colour;

            target.Blending = blending == null ? Blending : blending.Value;
        }

        public bool Equals(DrawInfo other)
        {
            return Matrix.Equals(other.Matrix) && Colour.Equals(other.Colour) && Blending.Equals(other.Blending);
        }
    }
}
