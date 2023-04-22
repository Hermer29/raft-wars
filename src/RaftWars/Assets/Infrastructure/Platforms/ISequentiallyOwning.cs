using DefaultNamespace.Skins;

namespace Infrastructure.Platforms
{
    public interface ISequentiallyOwning : IAcquirable
    {
        public int Serial { get; }
    }
}