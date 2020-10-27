using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrabalhoGrupo
{
    class CameraSF
    {
        // variáveis
        public Matrix viewMatrix;
        Vector3 cameraPosition;
        float yaw, pitch;
        int viewportWidth, viewportHeight;
        float velocity, offset;

        public CameraSF(GraphicsDevice graphicsDevice, Vector3 cameraPosition, float yaw, float pitch)
        {
            // atribuição de valores às variáveis globais criadas anteriormente
            this.cameraPosition = cameraPosition;
            this.yaw = yaw;
            this.pitch = pitch;
            this.viewportWidth = graphicsDevice.Viewport.Width;
            this.viewportHeight = graphicsDevice.Viewport.Height;
            velocity = 4f;
            offset = 1.7f;

            // matrix de rotação da camera
            Matrix rotation = Matrix.CreateFromYawPitchRoll(this.yaw, this.pitch, 0f);

            // vetor de direção que sobre o eixo x roda conforme a matrix de rotação recebida
            Vector3 direction = Vector3.Transform(Vector3.UnitX, rotation);

            // o vetor target é para onde a camera aponta, a sua posição que é recebida no construtor e a rotação que pode ter devido ao vetor direção
            Vector3 target = this.cameraPosition + direction;
            
            // criação da matriz de visão e da matriz de projeção assim como o aspect ratio utilizado para a matriz de projeção
            float aspectRatio = (float)viewportWidth / viewportHeight;
            viewMatrix = Matrix.CreateLookAt(this.cameraPosition, target, Vector3.Up);
            Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 1000f);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, Ground ground)
        {
            // variáveis
            float degreesPerPixelX = 0.3f;
            float degreesPerPixelY = 0.3f;
            int deltaX = mouseState.X - viewportWidth / 2;
            int deltaY = mouseState.Y - viewportHeight / 2;

            // calculo do yaw e do pitch
            this.yaw -= deltaX * MathHelper.ToRadians(degreesPerPixelX);
            this.pitch += deltaY * MathHelper.ToRadians(degreesPerPixelY);

            // com o update, a matriz de rotação, o vetor de direção e o vetor target variam dependendo do movimento do rato.
            Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(this.yaw, this.pitch, 0.0f);
            Vector3 direction = Vector3.Transform(-Vector3.UnitZ, rotationMatrix);
            Vector3 target = this.cameraPosition + direction;

            // a viewMatrix vai sempre mudando porque o target muda
            viewMatrix = Matrix.CreateLookAt(this.cameraPosition, target, Vector3.Up);

            if (keyboardState.IsKeyDown(Keys.NumPad8))
                cameraPosition += direction * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (keyboardState.IsKeyDown(Keys.NumPad2))
                cameraPosition -= direction * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (keyboardState.IsKeyDown(Keys.NumPad4))
                cameraPosition -= Vector3.Cross(direction, Vector3.UnitY) * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (keyboardState.IsKeyDown(Keys.NumPad6))
                cameraPosition += Vector3.Cross(direction, Vector3.UnitY) * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (cameraPosition.X >= 0 && cameraPosition.X < ground.groundWidht && cameraPosition.Z >= 0 && cameraPosition.Z < ground.groundHeight)
                cameraPosition.Y = ground.getHeight(cameraPosition.X, cameraPosition.Z) + offset;

            // é definido a posição inicial do rato
            Mouse.SetPosition(viewportWidth / 2, viewportHeight / 2);

            
        }
    }
}
