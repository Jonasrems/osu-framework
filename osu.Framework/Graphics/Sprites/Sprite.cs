// Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Textures;
using OpenTK;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.OpenGL;
using System.Diagnostics;

namespace osu.Framework.Graphics.Sprites
{
    public class Sprite : Drawable
    {
        private Shader textureShader;
        private Shader roundedTextureShader;

        public bool WrapTexture = false;

        public const int MAX_EDGE_SMOOTHNESS = 2;

        /// <summary>
        /// Determines over how many pixels of width the border of the sprite is smoothed
        /// in X and Y direction respectively.
        /// </summary>
        public Vector2 EdgeSmoothness = Vector2.Zero;

        public bool CanDisposeTexture { get; protected set; }

        #region Disposal

        protected override void Dispose(bool isDisposing)
        {
            if (CanDisposeTexture)
            {
                texture?.Dispose();
                texture = null;
            }

            base.Dispose(isDisposing);
        }

        #endregion

        protected override DrawNode CreateDrawNode() => new SpriteDrawNode();

        protected override void ApplyDrawNode(DrawNode node)
        {
            SpriteDrawNode n = node as SpriteDrawNode;

            n.ScreenSpaceDrawQuad = ScreenSpaceDrawQuad;
            n.DrawRectangle = DrawRectangle;
            n.Texture = Texture;
            n.WrapTexture = WrapTexture;

            n.TextureShader = textureShader;
            n.RoundedTextureShader = roundedTextureShader;
            n.InflationAmount = inflationAmount;

            base.ApplyDrawNode(node);
        }

        protected override void Load(BaseGame game)
        {
            base.Load(game);

            if (textureShader == null)
                textureShader = game?.Shaders?.Load(new ShaderDescriptor(VertexShaderDescriptor.Texture2D, FragmentShaderDescriptor.Texture));

            if (roundedTextureShader == null)
                roundedTextureShader = game?.Shaders?.Load(new ShaderDescriptor(VertexShaderDescriptor.Texture2D, FragmentShaderDescriptor.TextureRounded));
        }

        protected override bool CheckForcedPixelSnapping(Quad screenSpaceQuad)
        {
            return
                Rotation == 0
                && Math.Abs(screenSpaceQuad.Width - Math.Round(screenSpaceQuad.Width)) < 0.1f
                && Math.Abs(screenSpaceQuad.Height - Math.Round(screenSpaceQuad.Height)) < 0.1f;
        }

        private Texture texture;

        public Texture Texture
        {
            get { return texture; }
            set
            {
                if (value == texture)
                    return;

                if (texture != null && CanDisposeTexture)
                    texture.Dispose();

                texture = value;
                Invalidate(Invalidation.DrawNode);

                if (Size == Vector2.Zero)
                    Size = new Vector2(texture?.DisplayWidth ?? 0, texture?.DisplayHeight ?? 0);
            }
        }

        private Vector2 inflationAmount;
        protected override Quad ComputeScreenSpaceDrawQuad()
        {
            if (EdgeSmoothness == Vector2.Zero)
            {
                inflationAmount = Vector2.Zero;
                return base.ComputeScreenSpaceDrawQuad();
            }
            else
            {
                Debug.Assert(
                    EdgeSmoothness.X <= MAX_EDGE_SMOOTHNESS &&
                    EdgeSmoothness.Y <= MAX_EDGE_SMOOTHNESS,
                    $@"May not smooth more than {MAX_EDGE_SMOOTHNESS} or will leak neighboring textures in atlas.");

                Vector3 scale = DrawInfo.MatrixInverse.ExtractScale();
                
                inflationAmount = new Vector2(scale.X * EdgeSmoothness.X, scale.Y * EdgeSmoothness.Y);
                return ToScreenSpace(DrawRectangle.Inflate(inflationAmount));
            }
        }

        public override Drawable Clone()
        {
            Sprite clone = (Sprite)base.Clone();
            clone.texture = texture;

            return clone;
        }

        public override string ToString()
        {
            return base.ToString() + $" tex: {texture?.AssetName}";
        }
    }
}
