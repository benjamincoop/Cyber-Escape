using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cyber_Escape
{
    public class PixieParticleSystem : ParticleSystem
    {
        IParticleEmitter _emitter;
        public PixieParticleSystem(Game game, IParticleEmitter emitter) : base(game, 2000)
        {
            _emitter = emitter;
        }
        protected override void InitializeConstants()
        {
            textureFilename = "particle";
            minNumParticles = 4;
            maxNumParticles = 4;
            blendState = BlendState.Additive;
            DrawOrder = AdditiveBlendDrawOrder;
        }
        protected override void InitializeParticle(ref Particle p, Vector2 where)
        {
            var velocity = _emitter.Velocity;
            var acceleration = Vector2.UnitY * 400;
            var scale = RandomHelper.NextFloat(0.2f, 0.4f);
            var lifetime = RandomHelper.NextFloat(0.25f, 0.25f);
            p.Initialize(where, velocity, acceleration, Color.Cyan, scale: scale, lifetime: lifetime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            AddParticles(_emitter.Position);
        }
    }
}
