using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Main.BLL_HUY
{
    public struct Vector2
    {
        public float X;
        public float Y;
        public Vector2(float x, float y) { X = x; Y = y; }

        public static float Distance(Vector2 a, Vector2 b) { return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)); }
        public static Vector2 Normalize(Vector2 v) { float len = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y); return new Vector2(v.X / len, v.Y / len); }

        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.X - b.X, a.Y - b.Y);
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator *(Vector2 v, float scalar) => new Vector2(v.X * scalar, v.Y * scalar);
    }

    public abstract class Character
    {
        public int Health { get; protected set; }
        public int AttackPower { get; protected set; }
        public Vector2 Position { get; set; }
        public bool IsAlive => Health > 0;

        public Character(int health, int attack, Vector2 position)
        {
            Health = health;
            AttackPower = attack;
            Position = position;
        }
        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
            if (!IsAlive) { Die(); }
        }
        public virtual void Attack(Character target)
        {
            target.TakeDamage(AttackPower);
        }
        protected abstract void Die();
    }
}