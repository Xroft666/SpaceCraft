using UnityEngine;
using System.Collections;

namespace Voxel2D
{
    [System.Serializable]
	public struct IntVector2
    {

        public static readonly IntVector2 ZERO = new IntVector2(0, 0);
		public static readonly IntVector2 UP = new IntVector2(0, 1);
		public static readonly IntVector2 RIGHT = new IntVector2(1, 0);
		public static readonly IntVector2 ONE = new IntVector2(1, 1);

		public int x;
		public int y;

		public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static IntVector2 operator +(IntVector2 left, IntVector2 right)
        {
            return new IntVector2(left.x + right.x, left.y + right.y);
        }

		public static IntVector2 operator -(IntVector2 left, IntVector2 right)
		{
			return new IntVector2(left.x - right.x, left.y - right.y);
		}

		public static IntVector2 operator *(IntVector2 left, IntVector2 right)
		{
			return new IntVector2(left.x * right.x, left.y * right.y);
		}

		public static IntVector2 operator /(IntVector2 left, IntVector2 right)
		{
			return new IntVector2(left.x / right.x, left.y / right.y);
		}


        public static implicit operator Vector2(IntVector2 value)
        {
            return new Vector2(value.x, value.y);
        }

		public static implicit operator IntVector2(Vector2 value)
		{
			return new IntVector2((int)value.x, (int)value.y);
		}

		public string ToString(){
			return x+","+y;
		}

    }
}