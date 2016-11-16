namespace XenoCore.Engine.Services.Graphics
{
    public struct Texture
    {
        internal int id;

        public bool IsEmpty
        {
            get { return id == 0; }
        }

        public Texture(int id)
        {
            this.id = id;
        }
      
    }

}
