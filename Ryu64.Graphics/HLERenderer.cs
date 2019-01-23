using OpenTK.Graphics.OpenGL;

namespace Ryu64.Graphics
{
    class HLERenderer : IRenderer
    {
        public void Init()
        {
        }

        public void Render()
        {
            GL.ClearColor(0, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void Update()
        {
        }

        public void Cleanup()
        {
        }
    }
}