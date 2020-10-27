using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrabalhoGrupo
{
    class Ground
    {
        // Variáveis globais 
        BasicEffect effect;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        public int vertexCount, indexCount, groundWidht, groundHeight;
        float[] heightMapPositions;

        // Construtor do terreno que recebe graphicsDevice, o mapa de alturas e a textura do terreno
        public Ground(GraphicsDevice graphicsDevice, Texture2D heightMap, Texture2D groundTexture)
        {
            //variáveis
            float aspectRatio;

            // uso do efeito básico
            effect = new BasicEffect(graphicsDevice);

            // Calculo do aspeto visual
            aspectRatio = (float)graphicsDevice.Viewport.Width / graphicsDevice.Viewport.Height;

            // Visão da camara relativa ao mapa 
            effect.View = Matrix.CreateLookAt(new Vector3(64f, 5f, 64f), new Vector3(64f, 0f, 0f), Vector3.Up);

            // Projeção da Camara 
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 1000f);

            // efeito de textura para o mapa conter a textura passada como argumento no construtor
            effect.TextureEnabled = true;
            effect.Texture = groundTexture;

            // não há efeito de luz
            effect.LightingEnabled = false;

            // o efeito das cores nos vertices não está visivel
            effect.VertexColorEnabled = false;

            // Cria o mapa com textura e altura
            CreateGround(graphicsDevice, heightMap);
        }

        private void CreateGround(GraphicsDevice graphicsDevice, Texture2D heightMap)
        {
            // variáveis
            Color[] heightMapColors;
            VertexPositionNormalTexture[] vertices;
            short[] indexes;
            float verticalScale = 0.02f;

            // é definido o tamanho do array de cores do mapa de alturas e em seguida é guardado nele as suas cores
            heightMapColors = new Color[heightMap.Width * heightMap.Height];
            heightMap.GetData<Color>(heightMapColors);

            // contagem de vertices que em seguida são usados para definir o tamanho do array dos vértices
            groundHeight = heightMap.Height; //altura do mapa das alturas
            groundWidht = heightMap.Width;  // largura do mapa das alturas
            vertexCount = groundWidht * groundHeight;
            vertices = new VertexPositionNormalTexture[vertexCount];

            // definido o tamanho do array das posições de cada cor contida no mapa das alturas
            heightMapPositions = new float[groundWidht * groundHeight];

            // em cada posição é guardada a cor correspondente com a altura em relação à escala
            for (int i = 0; i < groundHeight * groundWidht; i++)
                heightMapPositions[i] = heightMapColors[i].R * verticalScale;
            // 2 ciclos for para percorrer todas as posições do terreno de jogo
            for (int z = 0; z < groundHeight; z++)
                for (int x = 0; x < groundWidht; x++)
                {
                    // variáveis
                    int i;
                    float h, tx, ty;

                    // variável i para percorrer todas as posições do array das posições do mapa das alturas
                    // variável h guarda a altura correspondente à posição do array
                    i = z * groundWidht + x;
                    h = heightMapPositions[i];

                    // tx e ty são as coordenadas 2D para a textura ser apresentada de forma continua
                    tx = x % 2;
                    ty = z % 2;

                    // no array dos vertices é guardado em cada posição um vertice com as coordenadas 3D, o vetor normal e o vertice da textura
                    vertices[i] = new VertexPositionNormalTexture(new Vector3(x, h, z), Vector3.Up, new Vector2(tx, ty));
                }

            // vertex buffer para diminuir o tráfego entre o CPU e a GPU
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertexCount, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);

            // calculo do numero de indices 
            indexCount = (groundWidht - 1) * 2 * groundHeight;
            indexes = new short[indexCount];

            // definir a ordem a percorrer nos indices
            for (int strip = 0; strip < (groundWidht - 1); strip++)
                for (int line = 0; line < groundHeight; line++)
                {
                    indexes[(strip * 2) * (groundHeight) + 2 * line] = (short)(strip + line * groundWidht);
                    indexes[(strip * 2) * (groundHeight) + 2 * line + 1] = (short)(strip + line * groundWidht + 1);
                }

            // index buffer para diminuir o trafego entre a CPU e a GPU
            indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indexCount, BufferUsage.None);
            indexBuffer.SetData<short>(indexes);
        }

        // método para obter a altura do ponto do mapa alturas para a camera não atravessar o terreno
        public float getHeight(float x, float z)
        {
            //variáveis para as coordenadas dos vértices
            int xa, xb, xc, xd;
            int za, zb, zc, zd;

            // calculo das coordenadas x dos 4 vertices em torno do ponto recebido
            xa = (int)x;
            xb = xa + 1;
            xc = xa;
            xd = xb;

            // calculo das coordenadas z dos 4 vertices em torno do ponto recebido
            za = (int)z;
            zb = za;
            zc = za + 1;
            zd = zc;

            // variáveis para calculo da altura de cada vertice
            float da, db, dc, dd;
            float ya, yb, yc, yd;

            // distancias entre as coordenadas x dos vertices e a coordenada x recebida
            da = x - xa;
            db = xb - x;
            dc = x - xc;
            dd = xd - x;

            // alturas dos vertices em volta do ponto recebido
            ya = heightMapPositions[za * groundWidht + xa];
            yb = heightMapPositions[zb * groundWidht + xb];
            yc = heightMapPositions[zc * groundWidht + xc];
            yd = heightMapPositions[zd * groundWidht + xd];

            //altura entre os vertices que rodeiam o ponto recebido
            float yab = ya * db + yb * da;
            float ycd = yc * dd + yd * dc;

            // distancia entre o z recebido e os z dos vertices
            float dab = z - za;
            float dcd = zc - z;

            // calculo da altura do ponto no momento
            float y = yab * dcd + ycd * dab;

            // return a altura
            return y;
        }

        public void Draw(GraphicsDevice graphics, CameraSF cameraSF)
        {
            // efeitos utilizados para o desenho da classe terreno
            effect.World = Matrix.Identity;
            effect.View = cameraSF.viewMatrix;
            effect.CurrentTechnique.Passes[0].Apply();

            // o uso dos buffers dos vertices e dos indices
            graphics.SetVertexBuffer(vertexBuffer);
            graphics.Indices = indexBuffer;

            // com este for é desenhada da stripde forma ordenada 
            for (int strip = 0; strip < groundWidht - 1; strip++)
                graphics.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, strip * (2 * groundHeight), groundHeight * 2 - 2);
        }
    }
}
