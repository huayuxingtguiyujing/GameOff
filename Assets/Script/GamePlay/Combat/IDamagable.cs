

namespace GameOff2023.GamePlay.Character {
    public interface IDamagable {
        public virtual void Hurt(float addExtent, float damage) { }

        public virtual void Death() { }
    }
}